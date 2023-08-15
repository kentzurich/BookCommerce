using BookCommerce_API.DataAccess;
using BookCommerce_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookCommerce_API.Repository.ShoppingCart
{
    public class ShoppingCartRepository : Repository<ShoppingCartModel>, IShoppingCartRepository
    {
        private ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<int> DecrementCount(ShoppingCartModel model, int count)
        {
            model.Count -= count;
            return model.Count;
        }

        public async Task<int> IncrementCount(ShoppingCartModel model, int count)
        {
            model.Count += count;
            return model.Count;
        }

        public async Task<List<ShoppingCartModel>> GetAllAsync(Expression<Func<ShoppingCartModel, bool>> filter = null, 
            bool isTracked = true)
        {
            IQueryable<ShoppingCartModel> cartList;

            if (!isTracked)
                cartList = _db.ShoppingCarts.AsNoTracking();

            cartList = _db.ShoppingCarts
                .Include(x => x.Product)
                    .ThenInclude(prod => prod.Category)
                .Include(x => x.Product)
                    .ThenInclude(prod => prod.CoverType)
                .Include(x => x.Product)
                    .ThenInclude(prod => prod.ProductImage)
                .Include(x => x.ApplicationUser)
                    .ThenInclude(user => user.Company);

            if (filter != null)
                cartList = _db.ShoppingCarts.Where(filter);

            return await cartList.ToListAsync();
        }

        public async Task<ShoppingCartModel> GetAsync(Expression<Func<ShoppingCartModel, bool>> filter = null, 
            bool isTracked = true)
        {
            IQueryable<ShoppingCartModel> cartList;

            if (!isTracked)
                cartList = _db.ShoppingCarts.AsNoTracking();

            cartList = _db.ShoppingCarts
                .Include(x => x.Product)
                    .ThenInclude(prod => prod.Category)
                .Include(x => x.Product)
                    .ThenInclude(prod => prod.CoverType)
                .Include(x => x.Product)
                    .ThenInclude(prod => prod.ProductImage)
                .Include(x => x.ApplicationUser)
                    .ThenInclude(user => user.Company);

            if (filter != null)
                cartList = _db.ShoppingCarts.Where(filter);

            return await cartList.FirstOrDefaultAsync();
        }
    }
}
