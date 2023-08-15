using System.Linq.Expressions;

namespace BookCommerce_API.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null,
            string? includeProperties = null,
            bool isTracked = true);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
            string? includeProperties = null,
            bool isTracked = true);
        Task CreateAsync(TEntity entity);
        Task RemoveAsync(TEntity entity);
        Task RemoveRangeAsync(List<TEntity> entity);
        Task SaveAsync();
    }
}
