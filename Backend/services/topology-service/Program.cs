using DotNetEnv;
using Npgsql;
using System.Text.Json.Serialization;
using topology_service.Enums;
using Microsoft.EntityFrameworkCore;
using topology_service.Data;
using topology_service.Repositories;
using topology_service.Services;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);
var useHttpsRedirection = builder.Configuration.GetValue("Security:UseHttpsRedirection", !builder.Environment.IsDevelopment());

builder.Configuration.AddEnvironmentVariables();

// Origins are configuration-driven so this service does not need a code
// change when the frontend host changes between environments.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
var allowCredentials = builder.Configuration.GetValue("Cors:AllowCredentials", false);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .AllowAnyHeader()
            .SetPreflightMaxAge(TimeSpan.FromMinutes(10));

        if (allowCredentials)
        {
            policy.AllowCredentials();
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<DeviceType>("device_type");
dataSourceBuilder.MapEnum<DeviceStatus>("device_status");
dataSourceBuilder.MapEnum<PriorityLevel>("priority_level");
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<TopologyDbContext>(options =>
{
    options.UseNpgsql(dataSource);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IDeviceLinkRepository, DeviceLinkRepository>();
builder.Services.AddScoped<IDeviceLinkService, DeviceLinkService>();
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
builder.Services.AddScoped<ILEARepository, LEARepository>();
builder.Services.AddScoped<ILEAService, LEAService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (useHttpsRedirection)
{
    app.UseHttpsRedirection();
}
app.UseCors("Frontend");

app.MapControllers();

app.Run();
