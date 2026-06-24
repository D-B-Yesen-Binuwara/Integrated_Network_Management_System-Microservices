<<<<<<< HEAD
using System.IO;
using alarm_service.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
=======
using DotNetEnv;
using alarm_service.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
>>>>>>> origin/main

namespace alarm_service;

public class AlarmServiceDbContextFactory : IDesignTimeDbContextFactory<AlarmDbContext>
{
    public AlarmDbContext CreateDbContext(string[] args)
    {
<<<<<<< HEAD
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<AlarmDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

=======
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
>>>>>>> origin/main
        return new AlarmDbContext(optionsBuilder.Options);
    }
}

<<<<<<< HEAD
=======

>>>>>>> origin/main
