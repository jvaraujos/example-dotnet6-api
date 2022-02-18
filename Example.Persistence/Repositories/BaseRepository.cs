using Microsoft.EntityFrameworkCore;
using Example.Application.Contracts.Persistence;
using Example.Persistence.DbContexts;
using System.Linq.Expressions;

namespace Example.Persistence.Repositories
{
    public abstract class BaseRepository<T> : IAsyncRepository<T> where T : class
    {
        protected readonly DbContexts.ExampleDbContext _dbContext;

        public BaseRepository(DbContexts.ExampleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entity)
        {
            await _dbContext.Set<T>().AddRangeAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> DeleteRangeAsync(IEnumerable<T> entity)
        {
            _dbContext.Set<T>().RemoveRange(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size)
        {
            return await _dbContext.Set<T>()
                .AsNoTracking()
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public IQueryable<T> Select()
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();
            return query;
        }

        public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entity)
        {
            _dbContext.Set<T>().UpdateRange(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            var affectedRows = await _dbContext.SaveChangesAsync();
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            var affectedRows = await _dbContext.SaveChangesAsync();
            return affectedRows > 0;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            var affectedRows = await _dbContext.SaveChangesAsync();
            return affectedRows > 0;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
    }
}
