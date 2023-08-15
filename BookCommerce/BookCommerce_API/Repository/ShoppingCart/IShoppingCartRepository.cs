using BookCommerce_API.Models;
using System.Linq.Expressions;

namespace BookCommerce_API.Repository.ShoppingCart
{
    public interface IShoppingCartRepository : IRepository<ShoppingCartModel>
    {
        Task<int> IncrementCount(ShoppingCartModel model, int count);
        Task<int> DecrementCount(ShoppingCartModel model, int count);
        Task<List<ShoppingCartModel>> GetAllAsync(Expression<Func<ShoppingCartModel, bool>>? filter = null,
           bool isTracked = true);
        Task<ShoppingCartModel> GetAsync(Expression<Func<ShoppingCartModel, bool>>? filter = null,
            string? includeProperties = null,
            bool isTracked = true);
    }
}
