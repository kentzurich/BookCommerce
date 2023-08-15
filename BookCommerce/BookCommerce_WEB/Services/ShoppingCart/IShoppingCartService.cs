using BookCommerce_WEB.Models.DTO.ShoppingCart;

namespace BookCommerce_WEB.Services.ShoppingCart
{
    public interface IShoppingCartService
    {
        Task<T> GetAsync<T>(int id);
        Task<T> AddToCart<T>(CreateShoppingCartDTO model);
        Task<T> CountShoppingCart<T>(string id);
        Task<T> GetAllAsync<T>(string id);
        Task<T> DecrementCount<T>(int id);
        Task<T> IncrementCount<T>(int id);
        Task<T> DeleteAsync<T>(int id);
        Task<T> DeleteRangeAsync<T>(string id);
    }
}
