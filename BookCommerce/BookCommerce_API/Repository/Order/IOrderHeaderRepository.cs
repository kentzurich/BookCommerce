using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.Order
{
    public interface IOrderHeaderRepository : IRepository<OrderHeaderModel>
    {
        Task UpdateAsync(OrderHeaderModel model);
        Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        Task UpdateStripePayment(int id, string sessionId, string paymentIntentId);
    }
}
