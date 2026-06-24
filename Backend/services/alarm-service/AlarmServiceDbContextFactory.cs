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
        // If env/connection string isn't available for migration generation,
        // this will fall back to appsettings.json.
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? "Host=localhost;Database=alarm_inms;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new AlarmDbContext(optionsBuilder.Options);
    }
}


