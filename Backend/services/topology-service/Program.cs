using DotNetEnv;
using Npgsql;
using topology_service.Enums;
using Microsoft.EntityFrameworkCore;
using topology_service.Data;
using topology_service.Repositories;
using topology_service.Services;

Env.TraversePath().Load();

NpgsqlConnection.GlobalTypeMapper.MapEnum<DeviceType>("device_type");
NpgsqlConnection.GlobalTypeMapper.MapEnum<DeviceStatus>("device_status");
NpgsqlConnection.GlobalTypeMapper.MapEnum<PriorityLevel>("priority_level");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TopologyDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IDeviceService, DeviceService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();