using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using SSIS.DAL.Identity;
using SSIS.Domain.Entities;
using SSIS.Domain.Interfaces;
using SSIS.PLL.DTOs.Login;
using SSIS.PLL.DTOs.Users;
using SSIS.PLL.Interfaces;
using SSIS.PLL.Services.Interfaces;

namespace SSIS.PLL.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserRepo _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IHostEnvironment _hostEnvironment;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserRepo userRepository,
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            // 1. تسجيل الدخول باستخدام Identity
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return null;

            // 2. التحقق من كلمة المرور
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return null;

            // 3. جلب Domain User
            var domainUser = await _userRepository.GetByIdentityUserIdAsync(user.Id.ToString());
            if (domainUser == null || !domainUser.IsActive || domainUser.IsDeleted)
                return null;

            // 4. توليد Token
            var token = _jwtService.GenerateToken(domainUser);

            return new LoginResponseDto
            {
                Token = token,
                FullName = domainUser.FullName,
                Email = domainUser.Email,
                Role = domainUser.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(8)
            };
        }

        public async Task<UserResponseDto?> RegisterAsync(RegisterRequestDto request)
        {
            // 1. حفظ الملف
            string? documentsPath = null;
            if (request.DocumentsFile != null)
            {
                documentsPath = await SaveFileAsync(request.DocumentsFile, "student-documents");
            }

            // 2. إنشاء ApplicationUser (Identity)
            var identityUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                DocumentsFilePath = documentsPath,
                IsVerified = false
            };

            var createResult = await _userManager.CreateAsync(identityUser, request.Password);
            if (!createResult.Succeeded)
                return null;

            // 3. إضافة Role
            await _userManager.AddToRoleAsync(identityUser, request.Role.ToString());

            // 4. إنشاء Domain User
            var domainUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                Role = request.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                DocumentsFilePath = documentsPath,
                IsVerified = false,
                IdentityUserId = identityUser.Id.ToString(),
            };

            await _userRepository.AddAsync(domainUser);
            await _unitOfWork.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = domainUser.Id,
                FullName = domainUser.FullName,
                Email = domainUser.Email,
                Role = domainUser.Role,
                IsActive = domainUser.IsActive,
                IsVerified = domainUser.IsVerified,
                CreatedAt = domainUser.CreatedAt
            };
        }

        public async Task<UserResponseDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                return null;

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<IReadOnlyList<UserResponseDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users
                .Where(u => !u.IsDeleted)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    IsVerified = u.IsVerified,
                    CreatedAt = u.CreatedAt
                })
                .ToList();
        }

        public async Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                return null;

            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // تحديث ApplicationUser أيضًا
            if (!string.IsNullOrEmpty(user.IdentityUserId))
            {
                var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
                if (identityUser != null)
                {
                    identityUser.FullName = user.FullName;
                    await _userManager.UpdateAsync(identityUser);
                }
            }

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // Soft Delete في Domain User
            await _userRepository.SoftDeleteAsync(id);

            // Soft Delete في ApplicationUser (Identity)
            if (!string.IsNullOrEmpty(user.IdentityUserId))
            {
                var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
                if (identityUser != null)
                {
                    identityUser.IsDeleted = true;
                    identityUser.DeletedAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(identityUser);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                return false;

            if (string.IsNullOrEmpty(user.IdentityUserId))
                return false;

            var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
            if (identityUser == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(identityUser, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                return false;

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<string?> GetDocumentsUrlAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                return null;

            return user.DocumentsFilePath;
        }

        public async Task<bool> VerifyUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                return false;

            user.IsVerified = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            // تحديث ApplicationUser
            if (!string.IsNullOrEmpty(user.IdentityUserId))
            {
                var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
                if (identityUser != null)
                {
                    identityUser.IsVerified = true;
                    await _userManager.UpdateAsync(identityUser);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectUserAsync(Guid id, string? rejectionReason = null)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                return false;

            // مسح الملف
            if (!string.IsNullOrEmpty(user.DocumentsFilePath))
            {
                string fullPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", user.DocumentsFilePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                user.DocumentsFilePath = null;
            }

            user.IsVerified = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            // تحديث ApplicationUser
            if (!string.IsNullOrEmpty(user.IdentityUserId))
            {
                var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);
                if (identityUser != null)
                {
                    identityUser.IsVerified = false;
                    identityUser.DocumentsFilePath = null;
                    await _userManager.UpdateAsync(identityUser);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            string uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream); 
            }

            return Path.Combine("uploads", folderName, uniqueFileName).Replace("\\", "/");
        }
    }
}