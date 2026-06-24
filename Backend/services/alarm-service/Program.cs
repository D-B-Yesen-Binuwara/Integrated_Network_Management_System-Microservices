using Npgsql;
using Microsoft.EntityFrameworkCore;
using alarm_service.Data;
using alarm_service.Repositories;
using alarm_service.Services;
using alarm_service.Interfaces;
using alarm_service.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AlarmDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMSANAlarmRepository, MSANAlarmRepository>();
builder.Services.AddScoped<IMSANAlarmService, MSANAlarmService>();

builder.Services.AddScoped<ISLBNAlarmRepository, SLBNAlarmRepository>();
builder.Services.AddScoped<ISLBNAlarmService, SLBNAlarmService>();

builder.Services.AddScoped<ICEAAlarmRepository, CEAAlarmRepository>();
builder.Services.AddScoped<ICEAAlarmService, CEAAlarmService>();

builder.Services.AddScoped<IRootCauseRepository, RootCauseRepository>();
builder.Services.AddScoped<IImpactedDeviceRepository, ImpactedDeviceRepository>();
builder.Services.AddScoped<IImpactAnalysisService, ImpactAnalysisService>();

builder.Services.AddHttpClient<ITopologyClient, TopologyClient>(client =>
{
    var topologyUrl = builder.Configuration["TopologyServiceUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(topologyUrl);
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AlarmDbContext>();
    context.Database.Migrate();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

