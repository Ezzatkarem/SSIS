using SSIS.Domain.Entities;

namespace SSIS.PLL.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}