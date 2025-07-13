using Infrastructure;
using Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Migrations;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                const string connectionStringEnvVar = "ATMOS_DB_CONNECTION";
                var connectionString = Environment.GetEnvironmentVariable(connectionStringEnvVar) ??
                                       "Host=localhost;Database=postgres;Username=postgres;Password=postgres";
                services.UseAtmosInfrastructure(hostContext.Configuration, connectionString);
                services.AddLogging(configure => configure.AddConsole());
            });

        try
        {
            // Build and run the migration logic
            await RunMigration(builder.Build());
            return 0; // Success
        }
        catch (Exception ex)
        {
            // If anything goes wrong, log it and return a failure code
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occurred during database migration: {ex.Message}");
            Console.ResetColor();
            return 1; // Failure
        }
    }
    
    private static async Task RunMigration(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        var dbContext = services.GetRequiredService<AtmosContext>();
        
        logger.LogInformation("Starting database migration...");
        
        var dbConnection = dbContext.Database.GetDbConnection();
        logger.LogInformation("Connecting to database: {Server}/{Database}", dbConnection.DataSource, dbConnection.Database);

        await dbContext.Database.MigrateAsync();

        logger.LogInformation("Database migration finished successfully.");
    }
}
