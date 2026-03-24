using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using System.Threading.Tasks;

namespace SSIS.Domain.Interfaces
{
    public interface IUserRepo : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailWithIncludesAsync(string email);
        Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role);
        Task SoftDeleteAsync(Guid id);
        Task ActivateAsync(Guid id);
        Task DeactivateAsync(Guid id);
    }
}