using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using SSIS.DAL.Identity;
using SSIS.DAL.Repositories;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
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
        private readonly IEmailService emailService;
        private readonly int MaxEmailCodeAttempts = 5;


        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserRepo userRepository,
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IHostEnvironment hostEnvironment,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _hostEnvironment = hostEnvironment;
            this.emailService = emailService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return null;

            var domainUser = await _userRepository.GetByIdentityUserIdAsync(user.Id.ToString());
            if (domainUser == null || !domainUser.IsActive || domainUser.IsDeleted)
                return null;

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

        public async Task<(UserResponseDto? Data, string[] Errors)> RegisterAsync(RegisterRequestDto request)
        {
            string? nationalIdPath = null;
            string? secondaryCertificatePath = null;
            string? universityDegreePath = null;
            string? cvPath = null;

            if (request.NationalIdImage != null)
                nationalIdPath = await SaveFileAsync(request.NationalIdImage, "national-ids");

            if (request.Role == UserRole.Student && request.SecondarySchoolCertificate != null)
                secondaryCertificatePath = await SaveFileAsync(request.SecondarySchoolCertificate, "certificates");

            if (request.Role == UserRole.Doctor)
            {
                if (request.UniversityDegree != null)
                    universityDegreePath = await SaveFileAsync(request.UniversityDegree, "degrees");
                if (request.Cv != null)
                    cvPath = await SaveFileAsync(request.Cv, "cvs");
            }

            var identityUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber
            };
            if (request.Password != request.ConfirmPassword)
                return (null, new[] { "Passwords do not match" });
            var result = await _userManager.CreateAsync(identityUser, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return (null, errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(identityUser, request.Role.ToString());
            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.Select(e => e.Description).ToArray();
                await _userManager.DeleteAsync(identityUser);
                return (null, errors);
            }

            var domainUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                Role = request.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IdentityUserId = identityUser.Id.ToString(),
                PhoneNumber = request.PhoneNumber,
                NationalIdImagePath = nationalIdPath,
                SecondarySchoolCertificatePath = secondaryCertificatePath,
                Title = request.Role == UserRole.Doctor ? request.Title : null,
                Specialization = request.Role == UserRole.Doctor ? request.Specialization : null,
                YearsOfExperience = request.Role == UserRole.Doctor ? request.YearsOfExperience : null,
                UniversityDegreePath = universityDegreePath,
                CvPath = cvPath,
                AdminCodeUsed = request.Role == UserRole.Admin ? request.AdminCode : null,
                IsEmailConfirmed = false
            };

            await _userRepository.AddAsync(domainUser);
            await _unitOfWork.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = domainUser.Id,
                FullName = domainUser.FullName,
                Email = domainUser.Email,
                Role = domainUser.Role,
                IsActive = domainUser.IsActive,
                CreatedAt = domainUser.CreatedAt
            };

            return (response, Array.Empty<string>());
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

            await _userRepository.SoftDeleteAsync(id);

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
        public async Task<(bool seccess,string message )> SendEmailVerificationCodeAsync(string email)
        {
            var user=await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return (false, "User not found");

            if (user.IsEmailConfirmed)
                return (false, "Email already verified");

            var code = GenerateVerificationCode();
                user.EmailVerificationAttempts = 0;
            user.EmailVerificationCode = code;
            user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddMinutes(5);
            user.LastEmailVerificationAttempt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            var body = $"<h1>Your code: {code}</h1><p>Valid for 15 min</p>";
            await emailService.SendEmailASync(email, "Verification Code", body);
            return (true, "Code sent");
        }

       public async Task<(bool seccess,string message)> VerifyEmailCodeAsync(string email,string code)
        {
            var user =await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return (false, "User not found");

            if (user.IsEmailConfirmed)
                return (false, "Email already verified");
            if (user.EmailVerificationCodeExpiry < DateTime.UtcNow)
                return (false, "Code expired");
            if (user.EmailVerificationAttempts >= MaxEmailCodeAttempts)
                return (false, "Too many failed attempts");
            if(user.EmailVerificationCode!=code)
            {
                user.EmailVerificationAttempts++;
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                return (false, "Invalid Code");

            }
            user.IsEmailConfirmed = true;
            user.EmailVerificationCode = null;
            user.EmailVerificationAttempts = 0;
            user.EmailVerificationCodeExpiry = null;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser != null)
            {
                identityUser.EmailConfirmed = true;
                await _userManager.UpdateAsync(identityUser);
            }
            return (true, "Email verified");



        }
        public async Task<(bool seccess,string message)> ResendEmailVerificationCodeAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return (false, "User not found");

            if (user.IsEmailConfirmed)
                return (false, "Email already verified");
            if (user.LastEmailVerificationAttempt?.AddMinutes(10) > DateTime.UtcNow)
                return (false, "Please wait 10 minute");
            return await SendEmailVerificationCodeAsync(email);


        }
        private string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}