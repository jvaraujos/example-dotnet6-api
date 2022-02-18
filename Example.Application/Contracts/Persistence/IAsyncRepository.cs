using System.Linq.Expressions;

namespace Example.Application.Contracts.Persistence
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        IQueryable<T> Select();
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression);
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entity);
        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entity);
        Task<IEnumerable<T>> DeleteRangeAsync(IEnumerable<T> entity);
        Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size);
    }
}
