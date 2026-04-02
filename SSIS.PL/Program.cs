using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SSIS.DAL.Extensions;
using SSIS.DAL.SeedData;
using SSIS.PL.Extensions;
using SSIS.BLL.Extentions;
using System.Text;

namespace SSIS.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. إضافة خدمات طبقة DAL (DbContext, Repositories, UnitOfWork)
            builder.Services.AddDALServices(builder.Configuration);

            // 2. إضافة خدمات طبقة BLL (UserService, JwtService, Identity)
            builder.Services.AddBLLServices ();  // يجب أن تحتوي هذه الطريقة على AddIdentity + ConfigureApplicationCookie

            // 3. إضافة خدمات Swagger
            builder.Services.AddSwaggerServices();

            // 4. إضافة JWT Authentication (تجاوز إعدادات Cookies)
            builder.Services.AddJwtAuthentication(builder.Configuration);

            // 5. إضافة Controllers
            builder.Services.AddControllers();

            var app = builder.Build();

            // 6. تهيئة البيانات الأولية (Seed Data)
            using (var scope = app.Services.CreateScope())
            {
                await SeedData.InitializeAsync(scope.ServiceProvider);
            }

            // 7. تكوين الـ Middleware
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

            // ترتيب مهم: UseAuthentication قبل UseAuthorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}