using DotNetEnv;
using Npgsql;
using topology_service.Enums;
using Microsoft.EntityFrameworkCore;
using topology_service.Data;
using topology_service.Repositories;
using topology_service.Services;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IDeviceLinkRepository, DeviceLinkRepository>();
builder.Services.AddScoped<IDeviceLinkService, DeviceLinkService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();