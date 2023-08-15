using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.Category;
using BookCommerce_WEB.Services.Base;

namespace BookCommerce_WEB.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly string bookCommerceUrl;
        private readonly string APIName;
        private readonly IBaseService _baseService;

        public CategoryService(IConfiguration config, IBaseService baseService)
        {
            _baseService = baseService;
            bookCommerceUrl = config.GetValue<string>("ServiceUrls:BookCommerce_API");
            APIName = "CategoryAPI";
        }
        public async Task<T> CreateAsync<T>(CreateCategoryDTO model)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.POST,
                    Data = model,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}"
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

        public async Task<T> UpdateAsync<T>(UpdateCategoryDTO model)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.PUT,
                    Data = model,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/{model.CategoryId}"
                });
        }
    }
}
