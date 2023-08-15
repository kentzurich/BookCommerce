namespace BookCommerce_WEB.Services.ProductImage
{
    public interface IProductImageService
    {
        Task<T> DeleteAsync<T>(int id);
        Task<T> GetAllAsync<T>();
    }
}
