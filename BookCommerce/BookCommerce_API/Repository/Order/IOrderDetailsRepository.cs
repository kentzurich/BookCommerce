using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.Order
{
    public interface IOrderDetailsRepository : IRepository<OrderDetailsModel>
    {
        Task UpdateAsync(OrderDetailsModel model);
    }
}
