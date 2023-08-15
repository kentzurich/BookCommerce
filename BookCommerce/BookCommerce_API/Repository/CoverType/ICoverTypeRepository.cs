using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.CoverType
{
    public interface ICoverTypeRepository : IRepository<CoverTypeModel>
    {
        Task UpdateAsync(CoverTypeModel model);
    }
}
