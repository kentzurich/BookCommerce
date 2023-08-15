using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Services.Base;

namespace BookCommerce_WEB.Services.ProductImage
{
    public class ProductImageService : IProductImageService
    {
        private readonly string bookCommerceUrl;
        private readonly string APIName;
        private readonly IBaseService _baseService;

        public ProductImageService(IConfiguration config,
                              IBaseService baseService)
        {
            bookCommerceUrl = config.GetValue<string>("ServiceUrls:BookCommerce_API");
            _baseService = baseService;
            APIName = "ProductImageAPI";
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.DELETE,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/{id}"
                });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.GET,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}"
                });
        }
    }
}
