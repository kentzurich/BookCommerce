using BookCommerce_WEB.Models.DTO.Product;

namespace BookCommerce_WEB.Services.Product
{
    public interface IProductService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(CreateProductDTO model);
        Task<T> UpdateAsync<T>(UpdateProductDTO model);
        Task<T> DeleteAsync<T>(int id);
    }
}
