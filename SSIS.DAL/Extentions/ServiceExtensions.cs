using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSIS.DAL.Data;
using SSIS.DAL.Reposatory;
using SSIS.DAL.UnitOfWork;
using SSIS.Domain.Interfaces;

namespace SSIS.DAL.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IUserRepo, UserRepo>();

            // UnitOfWork
            services.AddScoped<IUnitOfWork, SSIS.DAL.UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}