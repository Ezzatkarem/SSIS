// SSIS.PLL/Extensions/ServiceExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using SSIS.PLL.Services;
using SSIS.PLL.Services.Interfaces;
using SSIS.PLL.Interfaces;        // ← add this

namespace SSIS.PLL.Extentions
{
    public static class ServiceExtentions
    {
        public static IServiceCollection AddBLLServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>(); // ← ADD THIS
            return services;
        }
    }
}