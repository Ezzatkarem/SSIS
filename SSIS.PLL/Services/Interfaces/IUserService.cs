using SSIS.PLL.DTOs.Login;
using SSIS.PLL.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.PLL.Services.Interfaces
{
    public interface  IUserService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<UserResponseDto?> RegisterAsync(RegisterRequestDto request);  // ← أضف هذا




        Task<IReadOnlyList<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid Id);
        Task<bool> ChangePasswordAsync(Guid id, ChangePasswordRequestDto request);
        Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserRequestDto request);
        Task<string?> GetDocumentsUrlAsync(Guid id);
        Task<bool> VerifyUserAsync(Guid id);
        Task<bool> RejectUserAsync(Guid id, string? rejectionReason = null);



    }
}
