// SSIS.PLL/Extensions/ServiceExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using SSIS.BLL.Services;
using SSIS.BLL.Services.Interfaces;
using SSIS.BLL.Interfaces;        // ← add this

namespace SSIS.BLL.Extentions
{
    public static class ServiceExtentions
    {
        public static IServiceCollection AddBLLServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();
            
            // Phase 2 Services
            services.AddScoped<ICourseService, SSIS.BLL.Services.Implementation.CourseService>();
            services.AddScoped<IEnrollmentService, SSIS.BLL.Services.Implementation.EnrollmentService>();
            services.AddScoped<IScheduleService, SSIS.BLL.Services.Implementation.ScheduleService>();
            
            return services;
        }
    }
}