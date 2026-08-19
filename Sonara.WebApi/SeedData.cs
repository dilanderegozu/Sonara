using Microsoft.AspNetCore.Identity;
using Sonara.CoreLayer.Entities;

namespace Sonara.WebApi
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if(!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminMail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminMail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminMail,
                    Email = adminMail,
                    RegisteredAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}