using Microsoft.EntityFrameworkCore;
using SSIS.Domain.Entities;
using SSIS.Domain.Interfaces;
using SSIS.DAL.Data;
using SSIS.DAL.Repositories;
using SSIS.Domain.Enum;

namespace SSIS.DAL.Repositories
{
    public class UserRepo : Repository<User>, IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdentityUserIdAsync(string identityUserId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;
                await UpdateAsync(user);
            }
        }

        public async Task ActivateAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                user.IsActive = true;
                await UpdateAsync(user);
            }
        }

        public async Task DeactivateAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                await UpdateAsync(user);
            }
        }

        public async Task<List<User?>> GetByRoleAsync(UserRole role)
        {
            return await _context.Users.Where(p => p.Role == role).ToListAsync();
        }
    }
}