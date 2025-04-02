using Business.Factories;
using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(x =>
    x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register Repositories
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Factories
builder.Services.AddScoped<IClientFactory, ClientFactory>();
builder.Services.AddScoped<IProjectFactory, ProjectFactory>();
builder.Services.AddScoped<IStatusFactory, StatusFactory>();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IStatusService, StatusService>();

builder
    .Services.AddIdentity<MemberEntity, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Admin}/{action=Index}/{id?}")
    .WithStaticAssets();

// Create roles if they don't exist
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Create default statuses if they don't exist
    var statusRepository = scope.ServiceProvider.GetRequiredService<IStatusRepository>();
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
        var existsResult = await statusRepository.ExistsAsync(s => s.Name == statusName);
        if (!existsResult.Succeeded || !existsResult.Result)
        {
            // Handle potential error from ExistsAsync or if status doesn't exist
            if (!existsResult.Succeeded)
            {
                Console.WriteLine(
                    $"Error checking existence for status '{statusName}': {existsResult.Error}"
                );
                // Decide if you want to continue or stop seeding based on the error
            }
            else // Status does not exist, so add it
            {
                var addResult = await statusRepository.AddAsync(
                    new StatusEntity { Name = statusName, Color = "#808080" } // Add default color
                );
                if (!addResult.Succeeded)
                {
                    Console.WriteLine($"Error adding status '{statusName}': {addResult.Error}");
                    // Decide if you want to continue or stop seeding based on the error
                }
                else
                {
                    Console.WriteLine($"Added default status: {statusName}");
                }
            }
        }
    }
}

app.Run();
