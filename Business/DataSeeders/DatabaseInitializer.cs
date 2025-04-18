using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Business.DataSeeders
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            await ApplyMigrationsAsync(serviceProvider);
            await CreateRolesAsync(serviceProvider);
            await AssignAdminRoleAsync(serviceProvider, "kim.hammerstad@gmail.com"); // Assign Admin role

            await CreateDefaultStatusesAsync(serviceProvider);
        }

        private static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    if (dbContext.Database.GetPendingMigrations().Any())
                    {
                        Console.WriteLine("Applying pending database migrations...");
                        await dbContext.Database.MigrateAsync();
                        Console.WriteLine("Migrations applied successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
                // You might want to throw the exception here depending on your error handling strategy
            }
        }

        private static async Task CreateRolesAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<
                    RoleManager<IdentityRole>
                >();
                var roles = new[] { "Admin", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }
        }

        private static async Task AssignAdminRoleAsync(
            IServiceProvider serviceProvider,
            string adminEmail
        )
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<
                    UserManager<MemberEntity>
                >();
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser != null)
                {
                    // Check if user is already in Admin role
                    var isAdmin = await userManager.IsInRoleAsync(adminUser, "Admin");
                    if (!isAdmin)
                    {
                        // Add user to Admin role
                        var result = await userManager.AddToRoleAsync(adminUser, "Admin");
                        if (result.Succeeded)
                        {
                            Console.WriteLine($"Successfully assigned Admin role to {adminEmail}");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"Error assigning Admin role to {adminEmail}: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                            );
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{adminEmail} is already in the Admin role.");
                    }
                }
                else
                {
                    Console.WriteLine(
                        $"Admin user with email {adminEmail} not found. Cannot assign Admin role."
                    );
                }
            }
        }

        private static async Task CreateDefaultStatusesAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var statusRepository =
                    scope.ServiceProvider.GetRequiredService<IStatusRepository>();
                var defaultStatuses = new[]
                {
                    "Not Started",
                    "In Progress",
                    "Completed",
                    "On Hold",
                    "Cancelled",
                };

                foreach (var statusName in defaultStatuses)
                {
                    // Check if status with this NAME already exists first
                    Console.WriteLine($"Checking for status: {statusName}");
                    var findResult = await statusRepository.FindAsync(s => s.Name == statusName);

                    if (!findResult.Succeeded)
                    {
                        Console.WriteLine(
                            $"Error checking existence for status '{statusName}': {findResult.Error}"
                        );
                    }
                    else if (findResult.Result == null) // Status name not found, proceed to add
                    {
                        Console.WriteLine($"Status '{statusName}' not found, attempting to add...");
                        var addResult = await statusRepository.AddAsync(
                            new StatusEntity { Name = statusName, Color = "#808080" } // Add default color
                        );

                        if (!addResult.Succeeded)
                        {
                            Console.WriteLine(
                                $"Error adding status '{statusName}': {addResult.Error}"
                            );
                        }
                        else
                        {
                            Console.WriteLine(
                                $"Successfully added status: {statusName} with ID {addResult.Result?.Id}"
                            );
                        }
                    }
                    else // Status already exists
                    {
                        Console.WriteLine(
                            $"Status '{statusName}' already exists with ID {findResult.Result.Id}. Skipping add."
                        );
                    }
                }
            }
        }
    }
}
