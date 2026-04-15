// SSIS.PLL/Extensions/ServiceExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using SSIS.BLL.Services;
using SSIS.BLL.Services.Interfaces;
using SSIS.BLL.Interfaces;
using SSIS.PLL.Services.Interfaces;        // ← add this

namespace SSIS.BLL.Extentions
{
    public static class ServiceExtentions
    {
        public static IServiceCollection AddBLLServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, SSIS.PLL.Services.Implementaion.EmailService>();
            
            // Phase 2 Services
            services.AddScoped<ICourseService, SSIS.BLL.Services.Implementation.CourseService>();
            services.AddScoped<IEnrollmentService, SSIS.BLL.Services.Implementation.EnrollmentService>();
            services.AddScoped<IScheduleService, SSIS.BLL.Services.Implementation.ScheduleService>();
            services .AddScoped<IGradeService, SSIS.BLL.Services.Implementaion.GradeService>();

            return services;
        }
    }
}