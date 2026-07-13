using Microsoft.AspNetCore.Identity;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public static class AdminSeedService
    {
        public static async Task SeedAdminAsync(UserManager<AppUser> userManager)
        {
            var adminEmail = "admin@gmail.com";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    FullName = "Admin User",
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    IsActive = true
                };

                await userManager.CreateAsync(adminUser, "123456");
            }

            var roles = await userManager.GetRolesAsync(adminUser);

            if (!roles.Contains("Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}