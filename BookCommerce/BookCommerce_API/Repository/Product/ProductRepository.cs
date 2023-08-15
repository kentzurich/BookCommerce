using BookCommerce_API.DataAccess;
using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.Product
{
    public class ProductRepository : Repository<ProductModel>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(ProductModel model)
        {
            _db.Products.Update(model);
            await  _db.SaveChangesAsync();
        }
    }
}
