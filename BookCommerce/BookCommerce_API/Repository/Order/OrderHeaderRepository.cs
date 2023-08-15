using BookCommerce_API.DataAccess;
using BookCommerce_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCommerce_API.Repository.Order
{
    public class OrderHeaderRepository : Repository<OrderHeaderModel>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(OrderHeaderModel model)
        {
            _db.OrderHeaders.Update(model);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateStatus(int id, string orderStatus, string paymentStatus = null)
        {
            var order = await _db.OrderHeaders.FirstOrDefaultAsync(x => x.OrderHeaderId == id);
            if(order != null)
            {
                order.OrderStatus = orderStatus;
                if(paymentStatus != null)
                    order.PaymentStatus = paymentStatus;
            }
        }

        public async Task UpdateStripePayment(int id, string sessionId, string paymentIntentId)
        {
            var order = await _db.OrderHeaders.FirstOrDefaultAsync(x => x.OrderHeaderId == id);

            if (sessionId != null)
                order.SessionId = sessionId;

            if (paymentIntentId != null)
            {
                order.PaymentDate = DateTime.Now;
                order.PaymentIntentId = paymentIntentId;
            }
        }
    }
}
