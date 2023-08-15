using BookCommerce_API.DataAccess;
using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.ProductImage
{
    public class ProductImageRepository : Repository<ProductImageModel>, IProductImageRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
