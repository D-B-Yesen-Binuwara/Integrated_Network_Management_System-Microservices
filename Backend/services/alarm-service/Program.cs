using DotNetEnv;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using alarm_service.Data;
using alarm_service.Repositories;
using alarm_service.Repositories.Interfaces;
using alarm_service.Services.Implement;
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
builder.Services.AddSingleton<RootCauseEngine>();
builder.Services.AddScoped<ImpactAnalysisEngine>();

builder.Services.AddScoped<IRootCauseRepository, RootCauseRepository>();
builder.Services.AddScoped<IImpactedDeviceRepository, ImpactedDeviceRepository>();

builder.Services.AddHttpClient<ITopologyClient, TopologyClient>(client =>
{
    var baseUrl = builder.Configuration["TopologyService:BaseUrl"] ?? "http://localhost:5102";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IImpactAnalysisService, ImpactAnalysisService>();

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

