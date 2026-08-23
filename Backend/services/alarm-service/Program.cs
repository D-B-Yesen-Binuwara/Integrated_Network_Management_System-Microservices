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

// Keep direct service access safe for local tooling while the browser uses the
// gateway in normal operation. Origins are configured per environment.
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
builder.Services.AddScoped<ICorrelationEngine, CorrelationEngine>();
builder.Services.AddScoped<IAlarmFactsProvider, AlarmFactsProvider>();
builder.Services.AddScoped<ICorrelationResultService, CorrelationResultService>();

builder.Services.AddScoped<IRootCauseRepository, RootCauseRepository>();
builder.Services.AddScoped<IImpactedDeviceRepository, ImpactedDeviceRepository>();

var topologyServiceBaseUrl = builder.Configuration["TopologyService:BaseUrl"];

if (!Uri.TryCreate(EnsureTrailingSlash(topologyServiceBaseUrl), UriKind.Absolute, out var topologyServiceUri))
{
    throw new InvalidOperationException("TopologyService:BaseUrl must be configured with a valid absolute URI.");
}

builder.Services.AddHttpClient<ITopologyClient, TopologyClient>(client =>
{
    client.BaseAddress = topologyServiceUri;
});

builder.Services.AddScoped<IImpactAnalysisService, ImpactAnalysisService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.MapControllers();

var ruleLoader = app.Services.GetRequiredService<RuleLoader>();
Console.WriteLine($"SLBN Rules Loaded : {ruleLoader.SlbnRules.Count}");
Console.WriteLine($"CEAN Rules Loaded : {ruleLoader.CeanRules.Count}");
Console.WriteLine($"MSAN Rules Loaded : {ruleLoader.MsanRules.Count}");

app.Run();

static string EnsureTrailingSlash(string? value) =>
    string.IsNullOrWhiteSpace(value)
        ? string.Empty
        : value.EndsWith('/') ? value : $"{value}/";

