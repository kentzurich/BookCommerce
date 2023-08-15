using BookCommerce_WEB.Models.DTO.CoverType;

namespace BookCommerce_WEB.Services.CoverType
{
    public interface ICoverTypeService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(CreateCoverTypeDTO model);
        Task<T> UpdateAsync<T>(UpdateCoverTypeDTO model);
        Task<T> DeleteAsync<T>(int id);
    }
}
