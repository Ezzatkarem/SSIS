using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using System.Threading.Tasks;

namespace SSIS.Domain.Interfaces
{
    public interface IUserRepo : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdentityUserIdAsync(string identityUserId);
        Task SoftDeleteAsync(Guid id);
        Task ActivateAsync(Guid id);
        Task DeactivateAsync(Guid id);
    }
}