using Microsoft.AspNetCore.Identity;
using Movies.Domain.Constants;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.EF.Seeds
{
    public static  class SeedRoles
    {
        public static async Task  SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            if(!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Moderator.ToString()));
            }
        }
    }
}
