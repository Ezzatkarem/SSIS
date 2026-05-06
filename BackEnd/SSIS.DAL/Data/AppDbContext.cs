using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SSIS.DAL.Identity;
using SSIS.Domain.Entities;
using System.Reflection.Emit;

namespace SSIS.DAL.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public IEnumerable<object> Grade { get; internal set; }
        public DbSet<Payment> Payments  { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<CoursePrerequesite> coursePrerequesites { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne<Domain.Entities.User>()
                .WithOne()
                .HasForeignKey<ApplicationUser>(x => x.DomainUserId);


            builder.Entity<Payment>()
    .HasOne(p => p.Student)
    .WithMany()
    .HasForeignKey(p => p.StudentId)
    .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CoursePrerequesite>()
                .HasKey(c => c.Id);

            builder.Entity<CoursePrerequesite>()
                .HasOne(p => p.Course)
                .WithMany(p => p.Prerequesites)
                .HasForeignKey(p => p.Courseid)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CoursePrerequesite>()
                           .HasOne(p => p.PrerequesiteCourse)
                           .WithMany(p => p.RequiredFor)
                           .HasForeignKey(p => p.PrerequesiteCourseid)
                           .OnDelete(DeleteBehavior.Restrict);                
        }
    }
}