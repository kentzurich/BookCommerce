
using BookCommerce_API.DataAccess;
using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.Category
{
    public class CategoryRepository : Repository<CategoryModel>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(CategoryModel model)
        {
            _db.Categories.Update(model);
            await _db.SaveChangesAsync();
        }
    }
}
