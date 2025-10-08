using System.Linq.Expressions;

namespace Readify_Library.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string[] includes = null);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByCriteriaAsync(Expression<Func<T, bool>> criteria);
        Task<T> GetOneRecordWithIncludesAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task AddAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(int id);
    }
}
