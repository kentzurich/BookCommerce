using BookCommerce_API.DataAccess;
using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.Company
{
    public class CompanyRepository : Repository<CompanyModel>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(CompanyModel model)
        {
            _db.Companies.Update(model);
            await _db.SaveChangesAsync();
        }
    }
}
