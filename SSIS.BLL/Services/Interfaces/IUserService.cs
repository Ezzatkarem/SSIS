using SSIS.BLL.DTOs.Login;
using SSIS.BLL.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSIS.BLL.Services.Interfaces
{
    public interface  IUserService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<(UserResponseDto? Data, string[] Errors)> RegisterAsync(RegisterRequestDto request);




        Task<IReadOnlyList<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid Id);
        Task<bool> ChangePasswordAsync(Guid id, ChangePasswordRequestDto request);
        Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserRequestDto request);
        Task<string?> GetDocumentsUrlAsync(Guid id);
        Task<bool> VerifyUserAsync(Guid id);
        Task<bool> RejectUserAsync(Guid id, string? rejectionReason = null);

        Task<(bool seccess, string message)> SendEmailVerificationCodeAsync(string email);
            Task<(bool seccess, string message)> VerifyEmailCodeAsync(string email, string code);
        Task<(bool seccess, string message)> ResendEmailVerificationCodeAsync(string email);

    }
}
