using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SSIS.DAL.Data;
using SSIS.DAL.Identity;
using SSIS.Domain.Entities;
using SSIS.Domain.Enum;
using System;
using System.Threading.Tasks;

namespace SSIS.DAL.SeedData
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            await context.Database.EnsureCreatedAsync();

            string[] roles = { "Admin", "Doctor", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            var adminEmail = "admin@system.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");

                    var domainAdmin = new User
                    {
                        Id = Guid.NewGuid(),
                        FullName = adminUser.FullName,
                        Email = adminUser.Email,
                        Role = UserRole.Admin,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IdentityUserId = adminUser.Id.ToString()
                    };
                    await context.Users.AddAsync(domainAdmin);
                }
            }

            var doctorEmail = "doctor@system.com";
            var doctorUser = await userManager.FindByEmailAsync(doctorEmail);
            if (doctorUser == null)
            {
                doctorUser = new ApplicationUser
                {
                    UserName = doctorEmail,
                    Email = doctorEmail,
                    FullName = "Test Doctor",
                    IsActive = true
                };

                var result = await userManager.CreateAsync(doctorUser, "Doctor@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(doctorUser, "Doctor");

                    var domainDoctor = new User
                    {
                        Id = Guid.NewGuid(),
                        FullName = doctorUser.FullName,
                        Email = doctorUser.Email,
                        Role = UserRole.Doctor,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IdentityUserId = doctorUser.Id.ToString()
                    };
                    await context.Users.AddAsync(domainDoctor);
                }
            }

            var studentEmail = "student@system.com";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);
            if (studentUser == null)
            {
                studentUser = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FullName = "Test Student",
                    IsActive = true
                };

                var result = await userManager.CreateAsync(studentUser, "Student@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(studentUser, "Student");

                    var domainStudent = new User
                    {
                        Id = Guid.NewGuid(),
                        FullName = studentUser.FullName,
                        Email = studentUser.Email,
                        Role = UserRole.Student,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IdentityUserId = studentUser.Id.ToString()
                    };
                    await context.Users.AddAsync(domainStudent);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}