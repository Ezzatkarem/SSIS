using SSIS.Domain.Common;
using SSIS.Domain.Enum;

namespace SSIS.Domain.Entities
{
    public class User : BaseEntity, ISoftDelete
    {
        //General
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public decimal? ComulativeGpa { get; set; }
        public int TotalCompletedCredits { get; set; }
        public string IdentityUserId { get; set; } = string.Empty;

        //Student
        public string? DocumentsFilePath { get; set; }      
        public string NationalIdImagePath { get; set; }    
        public bool IsVerified { get; set; } = false;       
        public string? SecondarySchoolCertificatePath { get; set; }  
        public int? Level { get; set; }
         

       // Doctor
        public string? Title { get; set; }                  
        public string? Specialization { get; set; }         
        public string? UniversityDegreePath { get; set; }   
        public string? CvPath { get; set; }             
        

        // Admin
        public string? AdminCodeUsed { get; set; }    
        // Conferm Email

        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailVerificationCode { get; set; }
        public DateTime? EmailVerificationCodeExpiry { get; set; }
       
        public int EmailVerificationAttempts { get; set; }
        public DateTime? LastEmailVerificationAttempt { get; set; }
        // Navigation properties for Phase 2
        public ICollection<Course> TaughtCourses { get; set; } = new List<Course>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        // Grade navigation property
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        // Payment Navigation Property
        public ICollection<Fee>? Fees { get; set; }= new List<Fee>();
        public ICollection<Payment>? Payments { get; set; }=new List<Payment>();


        public ICollection<Notification>? Notefications { get; set; } = new List<Notification>();
    }
}