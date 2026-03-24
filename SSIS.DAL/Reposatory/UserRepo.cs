using SSIS.DAL.Data;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using SSIS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;  // ✅ أضف هذا السطر


namespace SSIS.DAL.Reposatory
{
    public class UserRepo : Reposatory<User>, IUserRepo
    {
        private readonly AppDbContext context;

        public UserRepo(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task ActivateAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            user.IsActive = true;
            await UpdateAsync(user);

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

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User?> GetByEmailWithIncludesAsync(string email)
        {
            throw new NotImplementedException();
            //return await _context.Users
            //    .Include(u => u.Enrollments)
            //    .ThenInclude(e => e.Course)
            //    .FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role)
        {
            throw new NotImplementedException();

            //return await _context.Users
            //   .Where(u => u.Role == role && !u.IsDeleted)
            //   .ToListAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            user.IsDeleted = true;
            user.DeletedAt = DateTime.Now;
            await UpdateAsync(user);
        }
    }
}
