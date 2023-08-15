using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.Product
{
    public interface IProductRepository : IRepository<ProductModel>
    {
        Task UpdateAsync(ProductModel model);
    }
}
