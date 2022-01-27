using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Models;
using Microsoft.AspNetCore.Identity;
using static Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions;

namespace DiAnterExpress.Data
{
    public class RoleInitializer
    {
        public async static Task Seed(IServiceProvider service)
        {
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            var userManager = service.GetService<UserManager<ApplicationUser>>();

            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));

                await roleManager.CreateAsync(new IdentityRole("Branch"));

                var newAdmin = new ApplicationUser
                {
                    UserName = "admin",
                };

                await userManager.CreateAsync(newAdmin, "password");

                await userManager.AddToRoleAsync(newAdmin, "Admin");

                Console.WriteLine("Role Seeded");
            }
        }
    }
}