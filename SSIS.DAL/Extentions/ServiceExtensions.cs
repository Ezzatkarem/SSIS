using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSIS.DAL.Data;
using SSIS.DAL.Identity;        // ← ApplicationUser

using SSIS.DAL.Repositories;
using SSIS.DAL.UnitOfWork;
using SSIS.Domain.Interfaces;

namespace SSIS.DAL.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ✅ 1. Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // ✅ 2. Identity — THIS WAS MISSING
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ✅ 3. Repositories
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

            // ✅ 4. UnitOfWork
            services.AddScoped<IUnitOfWork, SSIS.DAL.UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}