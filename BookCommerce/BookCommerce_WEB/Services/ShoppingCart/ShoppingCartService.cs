using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.ShoppingCart;
using BookCommerce_WEB.Services.Base;

namespace BookCommerce_WEB.Services.ShoppingCart
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly string bookCommerceUrl;
        private readonly string APIName;
        private readonly IBaseService _baseService;

        public ShoppingCartService(IConfiguration config,
                                   IBaseService baseService)
        {
            bookCommerceUrl = config.GetValue<string>("ServiceUrls:BookCommerce_API");
            _baseService = baseService;
            APIName = "ShoppingCartAPI";
        }
        public async Task<T> AddToCart<T>(CreateShoppingCartDTO model)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = model,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}"
                });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.GET,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/GetShoppingCart/{id}"
                });
        }

        public async Task<T> CountShoppingCart<T>(string id)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.GET,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/CountShoppingCart/{id}"
                });
        }

        public async Task<T> GetAllAsync<T>(string id)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.GET,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/GetAllShoppingCart/{id}"
                });
        }

		public async Task<T> IncrementCount<T>(int id)
		{
			return await _baseService.SendAsync<T>(
				new APIRequest()
				{
					APIType = StaticDetails.APIType.PUT,
					URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/IncrementCount/{id}"
				});
		}

		public async Task<T> DecrementCount<T>(int id)
		{
			return await _baseService.SendAsync<T>(
				new APIRequest()
				{
					APIType = StaticDetails.APIType.PUT,
					URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/DecrementCount/{id}"
				});
		}

		public async Task<T> DeleteAsync<T>(int id)
		{
			return await _baseService.SendAsync<T>(
				new APIRequest()
				{
					APIType = StaticDetails.APIType.DELETE,
					URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/DeleteCartItem/{id}"
				});
		}
		public async Task<T> DeleteRangeAsync<T>(string id)
		{
			return await _baseService.SendAsync<T>(
				new APIRequest()
				{
					APIType = StaticDetails.APIType.DELETE,
					URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/DeleteAllCart/{id}"
				});
		}
	}
}
