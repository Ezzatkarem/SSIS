using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SSIS.DAL.Extensions;
using FluentValidation.AspNetCore; 

using SSIS.DAL.SeedData;
using SSIS.PL.Extensions;

using System.Text;
using SSIS.BLL.Extentions;
using SSIS.BLL.Validators;

namespace SSIS.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDALServices(builder.Configuration);

            builder.Services.AddBLLServices ();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            builder.Services.AddSwaggerServices();

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            builder.Services.AddFluentValidationAutoValidation();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                await SeedData.InitializeAsync(scope.ServiceProvider);
            }

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

            await app.RunAsync();
        }
    }
}