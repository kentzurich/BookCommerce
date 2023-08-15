using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.ApplicationUser;
using BookCommerce_WEB.Services.Base;

namespace BookCommerce_WEB.Services.UserAuth
{
    public class UserAuthService : IUserAuthService
    {
        private readonly string bookCommerceUrl;
        private readonly string APIName;
        private readonly IBaseService _baseService;

        public UserAuthService(IConfiguration config,
                               IBaseService baseService)
        {
            bookCommerceUrl = config.GetValue<string>("ServiceUrls:BookCommerce_API");
            _baseService = baseService;
            APIName = "UserAuthAPI";
        }

        public async Task<T> GetAllRoles<T>()
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.GET,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/Roles"
                }, withBearer: false);
        }

		public async Task<T> GetAsync<T>(string id)
		{
			return await _baseService.SendAsync<T>(
			   new APIRequest()
			   {
				   APIType = StaticDetails.APIType.GET,
				   URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/GetUser/{id}"
			   });
		}

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(
               new APIRequest()
               {
                   APIType = StaticDetails.APIType.GET,
                   URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/GetAllUser"
               });
        }

        public async Task<T> GetUserId<T>(string userName)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.GET,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/GetUserId/{userName}"
                }, withBearer: false);
        }

		public async Task<T> IsInRole<T>(string role)
        {
            return await _baseService.SendAsync<T>(
               new APIRequest()
               {
                   APIType = StaticDetails.APIType.GET,
                   URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/IsInRole/{role}"
               });
        }

        public async Task<T> LoginAsync<T>(LoginRequestDTO model)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = model,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/Login"
                }, withBearer: false);
        }

        public async Task<T> LogoutAsync<T>(TokenDTO model)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = model,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/Revoke"
                }, withBearer: false);
        }

        public async Task<T> RegisterAsync<T>(RegistrationRequestDTO model)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = model,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/Register"
                }, withBearer: false);
        }

        public async Task<T> UpdateUserRoleAsync<T>(string oldRole, ApplicationUserDTO model)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.PUT,
                    Data = model,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/UpdateApplicationUserRole/{oldRole}"
                });
        }

        public async Task<T> UpdateApplicationUserAsync<T>(ApplicationUserDTO model)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.PUT,
                    Data = model,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/UpdateApplicationUser"
                });
        }
	}
}
