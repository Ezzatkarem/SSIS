using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Paymob.Net.Extensions;
using SSIS.BLL.Extentions;
using SSIS.BLL.Services.Implementaion;
using SSIS.BLL.Validators;
using SSIS.DAL.Extensions;
using SSIS.DAL.SeedData;
using SSIS.PL.Extensions;
using System.Text;

namespace SSIS.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDALServices(builder.Configuration);
            builder.Services.AddBLLServices();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            builder.Services.AddSwaggerServices();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddAutoMapper(cfg => { }, typeof(GradeService).Assembly);
            builder.Services.AddPaymob(builder.Configuration["Paymob:ApiKey"]);
            builder.Services.AddHttpClient<PaymentService>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                await SeedData.InitializeAsync(scope.ServiceProvider);
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Student System API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseCors("AllowAll"); 
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}