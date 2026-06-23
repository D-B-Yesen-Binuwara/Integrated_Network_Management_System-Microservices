using DotNetEnv;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using alarm_service.Data;
using alarm_service.Repositories;
using alarm_service.Services;
using alarm_service.Correlation.Engine;

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

builder.Services.AddSingleton<RuleLoader>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

var ruleLoader = app.Services.GetRequiredService<RuleLoader>();
Console.WriteLine($"SLBN Rules Loaded : {ruleLoader.SlbnRules.Count}");
Console.WriteLine($"CEAN Rules Loaded : {ruleLoader.CeanRules.Count}");
Console.WriteLine($"MSAN Rules Loaded : {ruleLoader.MsanRules.Count}");

app.Run();

