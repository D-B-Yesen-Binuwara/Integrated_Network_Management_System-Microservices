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
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<AlarmDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AlarmDbContext(optionsBuilder.Options);
    }
}

