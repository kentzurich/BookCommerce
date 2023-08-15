using BookCommerce_WEB.Models.DTO.Company;

namespace BookCommerce_WEB.Services.Company
{
    public interface ICompanyService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(CreateCompanyDTO model);
        Task<T> UpdateAsync<T>(UpdateCompanyDTO model);
        Task<T> DeleteAsync<T>(int id);
    }
}
