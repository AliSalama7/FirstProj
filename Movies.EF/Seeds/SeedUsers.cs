using Microsoft.AspNetCore.Identity;
using Movies.Domain.Constants;
using Movies.Domain.Models;
using System.Security.Claims;
namespace Movies.EF.Seeds
{
    public static class SeedUsers
    {
        public static async Task SeedBasicUser(UserManager<ApplicationUser> userManager)
        {
            var User = new ApplicationUser()
            {
                UserName = "basicuser",
                Email = "basicuser@lol.com",
                EmailConfirmed = true,
                FirstName = "Ali",
                LastName = "Salama"
            };
            var xyz = await userManager.FindByEmailAsync(User.Email);
            if (xyz == null)
            {
                await userManager.CreateAsync(User, "123Ali");
                await userManager.AddToRoleAsync(User, Roles.Basic.ToString());
            };
        }
        public static async Task SeedModerator(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var User = new ApplicationUser()
            {
                UserName = "admin",
                Email = "admin@lol.com",
                EmailConfirmed = true,
                FirstName = "Ali",
                LastName = "Salama"
            };
            var xyz = await userManager.FindByEmailAsync(User.Email);
            if (xyz == null)
            {
                await userManager.CreateAsync(User, "123Ali");
                await userManager.AddToRoleAsync(User, Roles.Moderator.ToString());
            };
            await roleManager.SeedClaimsForModerator();
        }

        private static async Task SeedClaimsForModerator(this RoleManager<IdentityRole> roleManager)
        {
            var ModRole = await roleManager.FindByNameAsync(Roles.Moderator.ToString());
            await roleManager.AddPermissionClaims(ModRole, "Movies");
        }
        public static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allclaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionList(module);
            foreach (var permission in allPermissions)
            {
                if (!allclaims.Any(m => m.Type == "Permission" && m.Value == permission))
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }
    }
}
