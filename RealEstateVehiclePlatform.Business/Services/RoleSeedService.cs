using Microsoft.AspNetCore.Identity;

namespace RealEstateVehiclePlatform.Business.Services
{
    public static class RoleSeedService
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                var exists = await roleManager.RoleExistsAsync(role);

                if (!exists)
                {
                    await roleManager.CreateAsync(new IdentityRole<int>
                    {
                        Name = role
                    });
                }
            }
        }
    }
}