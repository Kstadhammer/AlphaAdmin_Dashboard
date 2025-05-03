// Program.cs - Main application configuration and startup.
// Configures services, middleware, authentication, and database.
// Had help from AI to fix errors and refactor the code.

using AspNet.Security.OAuth.GitHub;
using Business.DataSeeders;
using Business.Factories;
using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure the database context to use SQLite
builder.Services.AddDbContext<AppDbContext>(x =>
    x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register Repositories for dependency injection
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Factories for dependency injection
builder.Services.AddScoped<IClientFactory, ClientFactory>();
builder.Services.AddScoped<IProjectFactory, ProjectFactory>();
builder.Services.AddScoped<IStatusFactory, StatusFactory>();

// Register Services for dependency injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IStatusService, StatusService>();

// Configure ASP.NET Core Identity
builder
    .Services.AddIdentity<MemberEntity, IdentityRole>(options =>
    {
        // User settings
        options.SignIn.RequireConfirmedAccount = false; // Email confirmation not required for login
        options.User.RequireUniqueEmail = true;

        // Password settings
        options.Password.RequiredLength = 8;
        // Add other password requirements if needed (e.g., require digit, non-alphanumeric, etc.)
    })
    .AddEntityFrameworkStores<AppDbContext>() // Use EF Core for storing Identity data
    .AddDefaultTokenProviders(); // Add providers for generating tokens (e.g., password reset)

// Configure Authentication services
builder
    .Services.AddAuthentication(options =>
    {
        // Set the default scheme to cookies
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie() // Add cookie-based authentication
    .AddGoogle(options => // Add Google OAuth provider
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    })
    .AddGitHub(options => // Add GitHub OAuth provider
    {
        options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;
    });

// Configure Application Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login"; // Redirect path if user is not authenticated
    options.AccessDeniedPath = "/Auth/AccessDenied"; // Redirect path if user is denied access
    options.SlidingExpiration = true; // Reset expiration time on each request
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie expiration time
});

// Configure Cookie Policy Options
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true; // Always require consent for non-essential cookies
    options.MinimumSameSitePolicy = SameSiteMode.None; // Adjust based on needs, None might be required for OAuth redirects
    // options.ConsentCookie.SecurePolicy = CookieSecurePolicy.Always; // Recommended for production (HTTPS)
});

// Add MVC services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseRouting(); // Adds route matching to the middleware pipeline

app.UseCookiePolicy(); // Add cookie policy middleware HERE - must be before Authentication/Authorization

app.UseAuthentication(); // Adds authentication middleware
app.UseAuthorization(); // Adds authorization middleware

app.MapStaticAssets(); // Placeholder for potential static asset mapping helper

// Configure default MVC route
app.MapControllerRoute(name: "default", pattern: "{controller=Admin}/{action=Index}/{id?}")
    .WithStaticAssets(); // Placeholder

// Initialize database: Apply migrations, create roles, and seed initial data
await DatabaseInitializer.InitializeAsync(app.Services);

// Run the application
app.Run();
