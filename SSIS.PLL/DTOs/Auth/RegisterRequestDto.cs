using SSIS.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;


namespace SSIS.PLL.DTOs.Login
{
    public class RegisterRequestDto
    {
        // General
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;   
        public UserRole Role { get; set; }
        public IFormFile NationalIdImage { get; set; }

        public string PhoneNumber { get; set; }

        // Student
        public IFormFile? SecondarySchoolCertificate { get; set; }

        // Doctor
        public string? Title { get; set; }
        public string? Specialization { get; set; }
        public int? YearsOfExperience { get; set; }
        public IFormFile? UniversityDegree { get; set; }
        public IFormFile? Cv { get; set; }

        // Admin
        public string? AdminCode { get; set; }

        // General
    }
}
