using BookCommerce_API.DataAccess;
using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.CoverType
{
    public class CoverTypeRepository : Repository<CoverTypeModel>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public CoverTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(CoverTypeModel model)
        {
            _db.CoverTypes.Update(model);
            await _db.SaveChangesAsync();
        }
    }
}
