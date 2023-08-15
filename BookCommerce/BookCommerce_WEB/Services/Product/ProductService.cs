using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.Product;
using BookCommerce_WEB.Services.Base;
using static BookCommerce_Utility.StaticDetails;

namespace BookCommerce_WEB.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly string bookCommerceUrl;
        private readonly string APIName;
        private readonly IBaseService _baseService;

        public ProductService(IConfiguration config,
                              IBaseService baseService)
        {
            bookCommerceUrl = config.GetValue<string>("ServiceUrls:BookCommerce_API");
            _baseService = baseService;
            APIName = "ProductAPI";
        }

        public async Task<T> CreateAsync<T>(CreateProductDTO model)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = model,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}",
                    ContentType = ContentType.MultipartFormData
                });
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
                }, withBearer: false);
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.GET,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/{id}"
                }, withBearer: false);
        }

        public async Task<T> UpdateAsync<T>(UpdateProductDTO model)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.PUT,
                    Data = model,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/{model.ProductId}",
                    ContentType = ContentType.MultipartFormData
                });
        }
    }
}
