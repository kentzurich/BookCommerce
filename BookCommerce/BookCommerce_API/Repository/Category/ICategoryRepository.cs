using BookCommerce_API.Models;

namespace BookCommerce_API.Repository.Category
{
    public interface ICategoryRepository : IRepository<CategoryModel>
    {
        Task UpdateAsync(CategoryModel model);
    }
}
