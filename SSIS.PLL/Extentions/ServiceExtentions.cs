using Microsoft.Extensions.DependencyInjection;
using SSIS.PLL.Services;
using SSIS.PLL.Services.Interfaces;
using SSIS.PLL.Interfaces;      

namespace SSIS.PLL.Extentions
{
    public static class ServiceExtentions
    {
        public static IServiceCollection AddBLLServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, SSIS.PLL.Services.Implementaion.EmailService>();
            return services;
        }
    }
}