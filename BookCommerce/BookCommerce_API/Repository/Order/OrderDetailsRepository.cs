using BookCommerce_API.DataAccess;
using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.Order
{
    public class OrderDetailsRepository : Repository<OrderDetailsModel>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(OrderDetailsModel model)
        {
            _db.OrderDetails.Update(model);
            await _db.SaveChangesAsync();
        }
    }
}
