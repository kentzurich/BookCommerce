using BookCommerce_WEB.Models.DTO.ApplicationUser;

namespace BookCommerce_WEB.Services.UserAuth
{
    public interface IUserAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestDTO model);
        Task<T> RegisterAsync<T>(RegistrationRequestDTO model);
        Task<T> LogoutAsync<T>(TokenDTO model);
        Task<T> GetAllRoles<T>();
        Task<T> GetUserId<T>(string userName);
        Task<T> GetAsync<T>(string id);
        Task<T> GetAllAsync<T>();
        Task<T> IsInRole<T>(string role);
        Task<T> UpdateUserRoleAsync<T>(string oldRole, ApplicationUserDTO model);
        Task<T> UpdateApplicationUserAsync<T>(ApplicationUserDTO model);
    }
}
