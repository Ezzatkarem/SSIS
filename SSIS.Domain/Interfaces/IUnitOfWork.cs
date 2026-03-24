using System;
using System.Threading;
using System.Threading.Tasks;

namespace SSIS.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IUserRepo Users { get; }
        // IRepository<Course> Courses { get; }
        // IRepository<Grade> Grades { get; }
        // IRepository<Attendance> Attendances { get; }
        // IRepository<Fee> Fees { get; }

        // Save
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // Transactions
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        // Check
        bool HasChanges();
    }
}