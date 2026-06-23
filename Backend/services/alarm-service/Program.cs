using DotNetEnv;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using alarm_service.Data;
using alarm_service.Repositories;
using alarm_service.Services;

Env.TraversePath().Load();


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AlarmDbContext>(options =>
{
    options.UseNpgsql(dataSource);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMSANAlarmRepository, MSANAlarmRepository>();
builder.Services.AddScoped<IMSANAlarmService, MSANAlarmService>();

builder.Services.AddScoped<ISLBNAlarmRepository, SLBNAlarmRepository>();
builder.Services.AddScoped<ISLBNAlarmService, SLBNAlarmService>();

builder.Services.AddScoped<ICEAAlarmRepository, CEAAlarmRepository>();
builder.Services.AddScoped<ICEAAlarmService, CEAAlarmService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

