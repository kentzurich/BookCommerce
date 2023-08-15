using BookCommerce_API.Models;
using BookCommerce_API.Models.DTO.ApplicationUser;

namespace BookCommerce_API.Repository.User
{
    public interface IUserRepository : IRepository<ApplicationUserModel>
	{
        Task<bool> IsUniqueUser(string userName);
		Task<string> GetUserId(string userName);
        Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO);
        Task RevokeRefreshToken(TokenDTO tokenDTO);
        Task UpdateAsync(ApplicationUserModel model);
    }
}
