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

            var context = service.GetService<ApplicationDbContext>();

            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));

                await roleManager.CreateAsync(new IdentityRole("Branch"));

                await roleManager.CreateAsync(new IdentityRole("Tokopodia"));

                var newAdmin = new ApplicationUser
                {
                    UserName = "admin",
                };

                await userManager.CreateAsync(newAdmin, "password");

                await userManager.AddToRoleAsync(newAdmin, "Admin");

                Console.WriteLine("Role Seeded");
            }

            if (!context.ShipmentTypes.Any())
            {
                context.ShipmentTypes.Add(
                    new ShipmentType
                    {
                        Name = "Di Entar Aja",
                        CostPerKg = 5000,
                        CostPerKm = 100,
                    }
                );
                context.ShipmentTypes.Add(
                    new ShipmentType
                    {
                        Name = "Di Anter",
                        CostPerKg = 5000,
                        CostPerKm = 200,
                    }
                );
                context.ShipmentTypes.Add(
                    new ShipmentType
                    {
                        Name = "Di Anter Super",
                        CostPerKg = 5000,
                        CostPerKm = 500,
                    }
                );

                context.SaveChanges();
            }
        }
    }
}