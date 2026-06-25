using DotNetEnv;
using alarm_service.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace alarm_service;

public class AlarmServiceDbContextFactory : IDesignTimeDbContextFactory<AlarmDbContext>
{
    public AlarmDbContext CreateDbContext(string[] args)
    {
        Env.TraversePath().Load();

        var optionsBuilder = new DbContextOptionsBuilder<AlarmDbContext>();

        // Use same connection-string source as runtime: DefaultConnection from env/appsettings.
        // IMPORTANT: no hard-coded fallback; migrations should fail fast if configuration is missing.
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Database connection string 'DefaultConnection' is missing. " +
                "Ensure it is provided via .env (loaded above) or environment variables for migrations/runtime.");
        }


        optionsBuilder.UseNpgsql(connectionString);
        return new AlarmDbContext(optionsBuilder.Options);
    }
}


