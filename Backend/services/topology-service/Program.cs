using System.IO;
using topology_service.Repositories;
using topology_service.Services;

LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection in configuration.");
}

builder.Services.AddSingleton(new DatabaseSettings(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDeviceRepository, DeviceRepository>();
builder.Services.AddSingleton<IDeviceService, DeviceService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

static void LoadDotEnv()
{
    var envFile = Path.Combine(AppContext.BaseDirectory, ".env");
    if (!File.Exists(envFile))
    {
        envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    }

    if (!File.Exists(envFile))
    {
        return;
    }

    foreach (var line in File.ReadAllLines(envFile))
    {
        var trimmed = line.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#") || !trimmed.Contains('='))
        {
            continue;
        }

        var index = trimmed.IndexOf('=');
        var key = trimmed[..index].Trim();
        var value = trimmed[(index + 1)..].Trim().Trim('"');
        Environment.SetEnvironmentVariable(key, value);
    }
}

internal sealed record DatabaseSettings(string DefaultConnection);
