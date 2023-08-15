using BookCommerce_API.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookCommerce_API.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _db;
        private DbSet<TEntity> _dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null,
            bool isTracked = true)
        {
            IQueryable<TEntity> query = _dbSet;

            if (!isTracked)
                query = query.AsNoTracking();

            if (includeProperties != null)
                query = IncludeProperties(includeProperties);

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null, 
            bool isTracked = true)
        {
            IQueryable<TEntity> query = _dbSet;

            if (!isTracked)
                query = query.AsNoTracking();

            if (includeProperties != null)
                query = IncludeProperties(includeProperties);

            if (filter != null)
                query = query.Where(filter);

            return await query.FirstOrDefaultAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            _db.Remove(entity);
            await SaveAsync();
        }

		public async Task RemoveRangeAsync(List<TEntity> entity)
		{
			_db.RemoveRange(entity);
			await SaveAsync();
		}

		private IQueryable<TEntity> IncludeProperties(string? includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }

            return query;
        }
    }
}
