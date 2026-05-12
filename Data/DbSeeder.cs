using CS395SI_Spring2023_Group1.Constants;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Net.NetworkInformation;

namespace CS395SI_Spring2023_Group1.Data
{
    public class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<IdentityUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            // Use constants to ensure consistent capitalization
            await EnsureRoleAsync(roleManager, MyConstants.ROLE_ADMIN);
            await EnsureRoleAsync(roleManager, MyConstants.ROLE_USER);
            await EnsureRoleAsync(roleManager, MyConstants.ROLE_MANAGER);
            await EnsureRoleAsync(roleManager, MyConstants.ROLE_INSTRUCTOR);
            await EnsureRoleAsync(roleManager, MyConstants.ROLE_STUDENT);

            var user = new IdentityUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var userInDb = await userManager.FindByEmailAsync(user.Email);

            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "Admin@123");
                await userManager.AddToRoleAsync(user, MyConstants.ROLE_ADMIN);
            }
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
