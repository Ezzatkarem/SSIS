using SSIS.DAL.Extensions;
using SSIS.PL.Extensions;  // ✅ مهم جدًا

namespace SSIS.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
                var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // DAL Services
            builder.Services.AddDALServices(builder.Configuration);

            // BLL Services

            // API Services
            builder.Services.AddSwaggerServices();
            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Student System API v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}