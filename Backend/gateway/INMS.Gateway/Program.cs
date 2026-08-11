using INMS.Gateway.Configuration;

var builder = WebApplication.CreateBuilder(args);

var gatewayOptions = builder.Configuration
    .GetSection(GatewayOptions.SectionName)
    .Get<GatewayOptions>()
    ?? throw new InvalidOperationException("Gateway configuration is required.");

var proxyConfiguration = GatewayProxyConfigurationBuilder.Build(gatewayOptions);

builder.Services
    .AddReverseProxy()
    .LoadFromMemory(proxyConfiguration.Routes, proxyConfiguration.Clusters);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapReverseProxy();

app.Run();
