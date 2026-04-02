using Microsoft.EntityFrameworkCore.Storage;
using SSIS.DAL.Data;
using SSIS.DAL.Reposatory;
using SSIS.DAL.Repositories;
using SSIS.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SSIS.DAL.UnitOfWork 
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        private IUserRepo? _userRepository;  

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IUserRepo Users => _userRepository ??= new UserRepo(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
                if (_transaction != null)
                {
                    _transaction.Dispose();
                }
            }
            _disposed = true;
        }
    }
}