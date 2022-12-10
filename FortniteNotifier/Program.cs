using FortniteNotifier;
using FortniteNotifier.Shared.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

// Create the bootstraper logger
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

try
{
    IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        // Get the environment configuration to get connection string and log location
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Add db context to the container
        services.AddDbContext<FortniteContext>(options => options.UseNpgsql(configuration["ConnectionString"]));

        // Add the unit of work to the container
        //services.AddScoped<UnitOfWork>();

        // Create the logger to log to console and rolling file
        string logPath = configuration["LogLocation"] ?? throw new Exception("LogLocation is null");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();
    })
    .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
