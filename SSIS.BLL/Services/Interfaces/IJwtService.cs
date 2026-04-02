using SSIS.Domain.Entities;

namespace SSIS.BLL.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}