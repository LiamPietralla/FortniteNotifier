using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog.Events;
using Serilog;
using FortniteNotifier.Shared.Data;
using Microsoft.EntityFrameworkCore;

// Create the default builder
var builder = WebApplication.CreateBuilder(args);

// Create the bootstraper logger
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

try
{
    // Add db context to the container
    builder.Services.AddDbContext<FortniteContext>(options => options.UseNpgsql(builder.Configuration["ConnectionString"]));

    // Add the unit of work to the container
    builder.Services.AddScoped<UnitOfWork>();
    
    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // Add authentication services
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            options.SlidingExpiration = true;
            options.AccessDeniedPath = "/Account/Forbidden";
        });

    // Get a copy of the configuration
    IConfiguration configuration = builder.Configuration;
    string logLocation = configuration["LogLocation"] ?? throw new Exception("Could not get log location from configuration");
    
    // Setup the application logger
    Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
                .WriteTo.File(logLocation, rollingInterval: RollingInterval.Day)
                .CreateLogger();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}