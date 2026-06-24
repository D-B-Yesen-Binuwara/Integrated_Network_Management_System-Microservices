<<<<<<< HEAD
=======
using DotNetEnv;
>>>>>>> origin/main
using Npgsql;
using Microsoft.EntityFrameworkCore;
using alarm_service.Data;
using alarm_service.Repositories;
using alarm_service.Services;
<<<<<<< HEAD
using alarm_service.Interfaces;
using alarm_service.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AlarmDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

=======
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
>>>>>>> origin/main

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMSANAlarmRepository, MSANAlarmRepository>();
builder.Services.AddScoped<IMSANAlarmService, MSANAlarmService>();

builder.Services.AddScoped<ISLBNAlarmRepository, SLBNAlarmRepository>();
builder.Services.AddScoped<ISLBNAlarmService, SLBNAlarmService>();

builder.Services.AddScoped<ICEAAlarmRepository, CEAAlarmRepository>();
builder.Services.AddScoped<ICEAAlarmService, CEAAlarmService>();

<<<<<<< HEAD
builder.Services.AddScoped<IRootCauseRepository, RootCauseRepository>();
builder.Services.AddScoped<IImpactedDeviceRepository, ImpactedDeviceRepository>();
builder.Services.AddScoped<IImpactAnalysisService, ImpactAnalysisService>();

builder.Services.AddHttpClient<ITopologyClient, TopologyClient>(client =>
{
    var topologyUrl = builder.Configuration["TopologyServiceUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(topologyUrl);
});
=======
builder.Services.AddSingleton<RuleLoader>();
builder.Services.AddSingleton<RootCauseEngine>();
>>>>>>> origin/main


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

<<<<<<< HEAD
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AlarmDbContext>();
    context.Database.Migrate();
}

app.UseHttpsRedirection();
app.MapControllers();

=======
app.UseHttpsRedirection();
app.MapControllers();

var ruleLoader = app.Services.GetRequiredService<RuleLoader>();
Console.WriteLine($"SLBN Rules Loaded : {ruleLoader.SlbnRules.Count}");
Console.WriteLine($"CEAN Rules Loaded : {ruleLoader.CeanRules.Count}");
Console.WriteLine($"MSAN Rules Loaded : {ruleLoader.MsanRules.Count}");

>>>>>>> origin/main
app.Run();

