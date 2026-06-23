using System.IO;
using alarm_service.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace alarm_service;



public class AlarmServiceDbContextFactory : IDesignTimeDbContextFactory<AlarmDbContext>
{
    public AlarmDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AlarmDbContext>();

        // Match topology-service style: read from appsettings/env via default configuration.
        // If env/connection string isn't available for migration generation,
        // this will still compile but migrations may fail to apply.
        // Use same connection string as appsettings.json.
        // Read connection string from configuration (appsettings/appsettings.Development + env vars).
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);
        return new AlarmDbContext(optionsBuilder.Options);







    }
}


