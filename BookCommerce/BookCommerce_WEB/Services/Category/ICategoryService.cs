using BookCommerce_WEB.Models.DTO.Category;

namespace BookCommerce_WEB.Services.Category
{
    public interface ICategoryService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(CreateCategoryDTO model);
        Task<T> UpdateAsync<T>(UpdateCategoryDTO model);
        Task<T> DeleteAsync<T>(int id);
    }
}
