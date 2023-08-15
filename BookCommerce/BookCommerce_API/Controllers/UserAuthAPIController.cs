using AutoMapper;
using BookCommerce_API.Models;
using BookCommerce_API.Models.DTO.ApplicationUser;
using BookCommerce_API.Repository.Company;
using BookCommerce_API.Repository.User;
using BookCommerce_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookCommerce_API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    public class UserAuthAPIController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ICompanyRepository _companyRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAuthAPIController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUserModel> _userManager;
        private APIResponse _response;

        public UserAuthAPIController(IUserRepository userRepo, 
                                     ICompanyRepository companyRepo,
                                     IMapper mapper,
                                     ILogger<UserAuthAPIController> logger,
                                     RoleManager<IdentityRole> roleManager,
                                     UserManager<ApplicationUserModel> userManager)
        {
            _userRepo = userRepo;
            _companyRepo = companyRepo;
            _mapper = mapper;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger.LogInformation("UserAuthAPI Controller called.");
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            _logger.LogInformation("Login Method called.");
            var tokenDTO = await _userRepo.Login(model);
            if(tokenDTO == null || string.IsNullOrEmpty(tokenDTO.AccessToken))
            {
                _response = Response(HttpStatusCode.BadRequest, false, new List<string> { "Username or Password is incorrect." });
                return BadRequest(_response);
            }

            _response = Response(HttpStatusCode.OK, Result: tokenDTO);
            _logger.LogInformation("Login Method done.");
            return Ok(_response);
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            _logger.LogInformation("Register Method called.");
            bool isUniqueUser = await _userRepo.IsUniqueUser(model.UserName);
            if(!isUniqueUser)
            {
                _response = Response(HttpStatusCode.BadRequest, false, new List<string> { "Username already exists." });
                return BadRequest(_response);
            }

            var role = await _roleManager.Roles.Where(x => x.Name == model.Role).FirstOrDefaultAsync();
            if(role == null)
            {
                _response = Response(HttpStatusCode.BadRequest, false, new List<string> { "Role does not exists." });
                return BadRequest(_response);
            }

            if (model.Role == StaticDetails.ROLE_USER_COMPANY)
            {
                if (model.CompanyId == 0)
                {
                    _response = Response(HttpStatusCode.BadRequest, false, new List<string> { "Company Id is required." });
                    return BadRequest(_response);
                }

                var company = await _companyRepo.GetAsync(x => x.CompanyId == model.CompanyId);
                if (company == null)
                {
                    _response = Response(HttpStatusCode.BadRequest, false, new List<string> { "Company does not exists." });
                    return BadRequest(_response);
                }
            }

            var user = await _userRepo.Register(model);
            if(user == null || string.IsNullOrEmpty(user.UserName))
            {
                _response = Response(HttpStatusCode.BadRequest, false, new List<string> { "Error while registering the user." });
                return BadRequest(_response);
            }

            _response = Response(HttpStatusCode.NoContent);
            _logger.LogInformation("Register Method done.");
            return Ok(_response);
        }

        [HttpPost("Refresh")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] TokenDTO model)
        {
            if(ModelState.IsValid)
            {
                _logger.LogInformation("GetNewTokenFromRefreshToken Method called.");
                var tokenDTOResponse = await _userRepo.RefreshAccessToken(model);
                if(tokenDTOResponse == null || string.IsNullOrEmpty(tokenDTOResponse.AccessToken))
                {
                    _response = Response(HttpStatusCode.BadRequest, false, new List<string> { "Token invalid." });
                    return BadRequest(_response);
                }

                _response = Response(HttpStatusCode.OK, Result: tokenDTOResponse);
                _logger.LogInformation("GetNewTokenFromRefreshToken Method done.");
                return Ok(_response);
            }
            else
            {
                _response = Response(HttpStatusCode.BadRequest, false, new List<string> { "Invalid input." });
                return BadRequest(_response);
            }
        }

        [HttpPost("Revoke")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] TokenDTO model)
        {
            if(ModelState.IsValid)
            {
                _logger.LogInformation("RevokeRefreshToken Method called.");
                await _userRepo.RevokeRefreshToken(model);
                _response = Response(HttpStatusCode.OK);
                _logger.LogInformation("RevokeRefreshToken Method done.");
                return Ok(_response);
            }

            _response = Response(HttpStatusCode.BadRequest, false, new List<string> { "Invalid input." });
            return BadRequest(_response);
        }

        [HttpGet("Roles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllRoles(string id)
        {
            try
            {
                _logger.LogInformation("GetAllRoles Method called.");
                IEnumerable<UserRoleDTO> roleList = await _roleManager.Roles.Select(x => 
                new UserRoleDTO
                {
                    Name = x.Name,
                }).ToListAsync();
                _response = Response(HttpStatusCode.OK, true, Result: roleList);
                _logger.LogInformation("GetAllRoles Method done.");
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpGet("GetUserId/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetUserId(string userName)
        {
            try
            {
                _logger.LogInformation("GetUserId Method called.");
                var userId = await _userRepo.GetUserId(userName);
                ApplicationUserDTO applicationUserDTO = new()
                {
                    Id = userId,
                    Role = await GetUserRole(userId),
                };
                _response = Response(HttpStatusCode.OK, true, Result: applicationUserDTO);
                _logger.LogInformation("GetUserId Method done.");
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

		[HttpGet("GetUser/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetUser(string id)
		{
			try
			{
				_logger.LogInformation("GetUser Method called.");
                
                var user = await _userRepo.GetAsync(x => x.Id == id, includeProperties: "Company");
                user.Role = await GetUserRole(user.Id);
                if (user == null)
                {
					_logger.LogError("User not found.");
					_response = Response(HttpStatusCode.NotFound, false, new List<string> { "User not found." });
                    return NotFound(_response);
                }

                //            ApplicationUserModel applicationUserDTO = _mapper.Map<ApplicationUserDTO>(user);
                //_response = Response(HttpStatusCode.OK, true, Result: applicationUserDTO);
                _response = Response(HttpStatusCode.OK, Result: _mapper.Map<ApplicationUserDTO>(user));
                _logger.LogInformation("GetUser Method done.");
				return _response;
			}
			catch (Exception ex)
			{
				_logger.LogCritical(ex.Message.ToString());
				throw;
			}
		}

		[HttpGet("GetAllUser")]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetAllUser()
        {
            try
            {
                _logger.LogInformation("GetAllUser Method called.");

                var userList = await _userRepo.GetAllAsync(includeProperties: "Company");
                if (userList == null)
                {
                    _logger.LogError("Users not found.");
                    _response = Response(HttpStatusCode.NotFound, false, new List<string> { "Users not found." });
                    return NotFound(_response);
                }

                foreach (var user in userList)
                {
                    user.Role = await GetUserRole(user.Id);

                    if (user.Company is null)
                        user.Company = new() { Name = string.Empty };
                }

                List<ApplicationUserDTO> applicationUserList = _mapper.Map<List<ApplicationUserDTO>>(userList);
                _response = Response(HttpStatusCode.OK, true, Result: applicationUserList);
                _logger.LogInformation("GetAllUser Method done.");
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpGet("IsInRole/{role}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> IsInRole(string role)
        {
            try
            {
                _logger.LogInformation("IsInRole Method called.");
                var isInRole = User.IsInRole(role);
                if (!isInRole)
                {
                    _logger.LogError("Role not found.");
                    _response = Response(HttpStatusCode.NotFound, false, new List<string> { "Role not found." });
                    return NotFound(_response);
                }

                _response = Response(HttpStatusCode.OK);
                _logger.LogInformation("IsInRole Method done.");
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        //MAY ERROR KAPA DITO.
        [HttpPut("UpdateApplicationUserRole/{oldRole}", Name = "UpdateApplicationUserRole")]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> UpdateApplicationUserRole(string oldRole, [FromBody] ApplicationUserDTO model)
        {
            try
            {
                _logger.LogInformation("UpdateApplicationUser Method called.");

                if (string.IsNullOrEmpty(oldRole))
                {
                    _logger.LogError("Old model is null.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                if (model == null)
                {
                    _logger.LogError("ApplicationUserDTO model is null.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                //Error pa Dito
                ApplicationUserModel applicationUserModel = _mapper.Map<ApplicationUserModel>(model);
                await _userManager.RemoveFromRoleAsync(applicationUserModel, oldRole);
                await _userManager.AddToRoleAsync(applicationUserModel, applicationUserModel.Role);
                _response = Response(HttpStatusCode.NoContent);
                _logger.LogInformation("UpdateApplicationUser Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        [HttpPut("UpdateApplicationUser", Name = "UpdateApplicationUser")]
        [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> UpdateApplicationUser([FromBody] ApplicationUserDTO model)
        {
            try
            {
                _logger.LogInformation("UpdateApplicationUser Method called.");

                if (model == null)
                {
                    _logger.LogError("ApplicationUserDTO model is null.");
                    _response = Response(HttpStatusCode.BadRequest, false);
                    return BadRequest(_response);
                }

                ApplicationUserModel applicationUserModel = _mapper.Map<ApplicationUserModel>(model);
                await _userRepo.UpdateAsync(applicationUserModel);
                _response = Response(HttpStatusCode.NoContent);
                _logger.LogInformation("UpdateApplicationUser Method done.");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message.ToString());
                throw;
            }
        }

        private APIResponse Response(HttpStatusCode statusCode,
                                     bool isSuccess = true,
                                     List<string> ErrorMessages = null,
                                     object Result = null)
        {
            var response = new APIResponse();
            response.IsSuccess = isSuccess;
            response.StatusCode = statusCode;
            response.ErrorMessages = ErrorMessages;
            response.Result = Result;

            return response;
        }

        private async Task<string> GetUserRole(string applicationUserId)
        {
            var userManager = await _userManager.FindByIdAsync(applicationUserId);
            var role = await _userManager.GetRolesAsync(userManager);

            return role.FirstOrDefault();
        }
    }
}
