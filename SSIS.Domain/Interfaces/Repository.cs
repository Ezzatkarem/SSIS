using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SSIS.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // Get
        Task<T?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Add
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        // Update
        Task UpdateAsync(T entity);

        // Delete
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);

        // Check
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync();

        // Queryable
        IQueryable<T> Query();
    }
}