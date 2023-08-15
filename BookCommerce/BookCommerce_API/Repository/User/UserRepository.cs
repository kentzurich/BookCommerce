using AutoMapper;
using BookCommerce_API.DataAccess;
using BookCommerce_API.Models;
using BookCommerce_API.Models.DTO.ApplicationUser;
using BookCommerce_Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookCommerce_API.Repository.User
{
    public class UserRepository : Repository<ApplicationUserModel>, IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly string SecretKey;

        public UserRepository(ApplicationDbContext db,
                              UserManager<ApplicationUserModel> userManager,
                              RoleManager<IdentityRole> roleManager,
                              IMapper mapper,
                              IConfiguration config) : base(db)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            SecretKey = config.GetValue<string>("APISettings:Secret");
        }

        public async Task<bool> IsUniqueUser(string userName)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
                return true;

            return false;
        }

        public async Task<string> GetUserId(string userName)
        {
            var userId = _db.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName == userName).GetAwaiter().GetResult().Id;
            if (userId != null)
                return userId;

            return string.Empty;
        }

        public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _db.ApplicationUsers
                .FirstOrDefaultAsync(x => x.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if(!isValid)
                return new() { AccessToken = string.Empty };

            var jwtTokenId = $"JTI{Guid.NewGuid()}";
            var accessToken = await GetAccessToken(user, jwtTokenId);
            var refreshToken = await CreateNewRefreshToken(user.Id, jwtTokenId);
            return new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ApplicationUserModel applicationUserModel = new()
            {
                UserName = registrationRequestDTO.UserName,
                Email = registrationRequestDTO.UserName,
                NormalizedEmail = registrationRequestDTO.UserName.ToUpper(),
                Name = registrationRequestDTO.Name,
                StreetAddress = registrationRequestDTO.StreetAddress,
                City = registrationRequestDTO.City,
                State = registrationRequestDTO.State,
                PostalCode = registrationRequestDTO.PostalCode,
                PhoneNumber = registrationRequestDTO.PhoneNumber,
                CompanyId = null

            };

            if (registrationRequestDTO.CompanyId != 0 && registrationRequestDTO.Role == StaticDetails.ROLE_USER_COMPANY)
                applicationUserModel.CompanyId = registrationRequestDTO.CompanyId;

            try
            {
                var result = await _userManager.CreateAsync(applicationUserModel, registrationRequestDTO.Password);

                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(StaticDetails.ROLE_ADMIN).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.ROLE_ADMIN));
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.ROLE_EMPLOYEE));
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.ROLE_USER_COMPANY));
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.ROLE_USER_INDIVIDUAL));
                    }

                    await _userManager.AddToRoleAsync(applicationUserModel, registrationRequestDTO.Role);
                    var userToReturn = await _db.ApplicationUsers
                        .FirstOrDefaultAsync(x => x.UserName == registrationRequestDTO.UserName);
                    return _mapper.Map<UserDTO>(applicationUserModel);
                }
            }
            catch 
            {
                throw;
            }

            return new();
        }

        public async Task RevokeRefreshToken(TokenDTO tokenDTO)
        {
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Refresh_Token == tokenDTO.RefreshToken);
            if (existingRefreshToken == null)
                return;

            var isValidToken = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isValidToken)
                return;

            await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
        }

        public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO)
        {
            //find an existing refresh token
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Refresh_Token == tokenDTO.RefreshToken);
            if (existingRefreshToken == null)
                return new();

            //compare data from existing refresh token and access token
            var isValidToken = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isValidToken)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new();
            }

            //when someone tries to use not valid refresh token
            if(!existingRefreshToken.IsValid)
            {
                await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
                return new();
            }

            //if expired mark as invalid and return empty
            if(existingRefreshToken.ExpiresAt < DateTime.Now)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new();
            }

            //refresh old refresh with new one with updated expire date
            var newRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);


            //revoke existing refresh token
            await MarkTokenAsInvalid(existingRefreshToken);

            //generate new access token
            var applicationUser = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == existingRefreshToken.UserId);
            if (applicationUser == null)
                return new();

            var newAccessToken = await GetAccessToken(applicationUser, existingRefreshToken.JwtTokenId);

            return new()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }

        private async Task<string> CreateNewRefreshToken(string userId, string jwtTokenId)
        {
            RefreshToken refreshToken = new()
            {
                IsValid = true,
                UserId = userId,
                JwtTokenId = jwtTokenId,
                ExpiresAt = DateTime.Now.AddDays(1), // this must be same with GetAccessToken
                Refresh_Token = $"{Guid.NewGuid()}-{Guid.NewGuid()}",

            };

            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();
            return refreshToken.Refresh_Token;
        }

        private async Task MarkAllTokenInChainAsInvalid(string userId, string jwtTokenId)
        {
            await _db.RefreshTokens
                .Where(x => x.UserId == userId && x.JwtTokenId == jwtTokenId)
                .ExecuteUpdateAsync(x => x.SetProperty(refreshToken => refreshToken.IsValid, false));
        }

        private async Task MarkTokenAsInvalid(RefreshToken refreshToken)
        {
            refreshToken.IsValid = false;
            await _db.SaveChangesAsync();
        }

        private bool GetAccessTokenData(string accessToken, string expectedUserId, string expectedTokenId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);
                var tokenId = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                var userId = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

                return userId == expectedUserId && tokenId == expectedTokenId;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GetAccessToken(ApplicationUserModel applicationUserModel, string jwtTokenId)
        {
            //if user was found, generate JWT Token
            var roles = await _userManager.GetRolesAsync(applicationUserModel);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, applicationUserModel.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                    new Claim(JwtRegisteredClaimNames.Sub, applicationUserModel.Id),
                    new Claim(JwtRegisteredClaimNames.Iss, "https://bookcommerce-api.com"),
                    new Claim(JwtRegisteredClaimNames.Aud, "bookcommerce.com"),
                }),
                Expires = DateTime.Now.AddDays(1), // this must be same with CreateNewRefreshToken
                Issuer = "https://bookcommerce-api.com",
                Audience = "bookcommerce.com",
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public async Task UpdateAsync(ApplicationUserModel model)
        {
            _db.ApplicationUsers.Update(model);
            await _db.SaveChangesAsync();
        }
    }
}
