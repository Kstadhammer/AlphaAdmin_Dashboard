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
    /// <summary>
    /// Static class responsible for initializing the database on application startup.
    /// Includes applying migrations, seeding default roles, statuses, and an administrator account.
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Initializes the database by applying migrations and seeding essential data.
        /// </summary>
        /// <param name="serviceProvider">The application's service provider.</param>
        /// <returns>A task representing the asynchronous initialization process.</returns>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            await ApplyMigrationsAsync(serviceProvider);
            await CreateRolesAsync(serviceProvider);
            await CreateDefaultAdminAsync(serviceProvider);
            await AssignAdminRoleAsync(serviceProvider, "kim.hammerstad@gmail.com"); // Assign Admin role

            await CreateDefaultStatusesAsync(serviceProvider);
        }

        /// <summary>
        /// Applies any pending Entity Framework Core migrations to the database.
        /// </summary>
        /// <param name="serviceProvider">The application's service provider.</param>
        /// <returns>A task representing the asynchronous migration process.</returns>
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

        /// <summary>
        /// Creates the default roles ("Admin", "User") if they don't already exist.
        /// </summary>
        /// <param name="serviceProvider">The application's service provider.</param>
        /// <returns>A task representing the asynchronous role creation process.</returns>
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

        /// <summary>
        /// Creates a default administrator user if one doesn't exist with the predefined credentials.
        /// Ensures the default admin user is assigned the "Admin" role.
        /// </summary>
        /// <param name="serviceProvider">The application's service provider.</param>
        /// <returns>A task representing the asynchronous admin creation process.</returns>
        private static async Task CreateDefaultAdminAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<
                    UserManager<MemberEntity>
                >();

                const string defaultAdminEmail = "admin@admin.se";
                var adminUser = await userManager.FindByEmailAsync(defaultAdminEmail);

                if (adminUser == null)
                {
                    Console.WriteLine(
                        $"Default admin user not found. Creating admin user: {defaultAdminEmail}"
                    );

                    var defaultAdmin = new MemberEntity
                    {
                        UserName = defaultAdminEmail,
                        Email = defaultAdminEmail,
                        NormalizedUserName = defaultAdminEmail.ToUpper(),
                        NormalizedEmail = defaultAdminEmail.ToUpper(),
                        EmailConfirmed = true,
                        FirstName = "Admin",
                        LastName = "User",
                        JobTitle = "System Administrator",
                        IsActive = true,
                    };

                    var result = await userManager.CreateAsync(defaultAdmin, "Admin123!");
                    if (result.Succeeded)
                    {
                        Console.WriteLine("Default admin user created successfully");
                        await userManager.AddToRoleAsync(defaultAdmin, "Admin");
                        Console.WriteLine("Assigned Admin role to default admin user");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Failed to create default admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                        );
                    }
                }
                else
                {
                    Console.WriteLine("Default admin user already exists");

                    // Ensure admin has the Admin role
                    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    {
                        var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                        if (roleResult.Succeeded)
                        {
                            Console.WriteLine("Assigned Admin role to existing default admin user");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"Failed to assign Admin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}"
                            );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Assigns the "Admin" role to a specific user identified by email, if they exist and are not already an admin.
        /// </summary>
        /// <param name="serviceProvider">The application's service provider.</param>
        /// <param name="adminEmail">The email address of the user to grant admin privileges.</param>
        /// <returns>A task representing the asynchronous role assignment process.</returns>
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

        /// <summary>
        /// Creates default project statuses ("Not Started", "In Progress", etc.) if they don't already exist.
        /// </summary>
        /// <param name="serviceProvider">The application's service provider.</param>
        /// <returns>A task representing the asynchronous status seeding process.</returns>
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
