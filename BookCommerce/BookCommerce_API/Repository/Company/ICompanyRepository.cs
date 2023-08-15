using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.Company
{
    public interface ICompanyRepository : IRepository<CompanyModel>
    {
        Task UpdateAsync(CompanyModel model);
    }
}
