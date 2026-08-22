using INMS.Gateway.Configuration;

var builder = WebApplication.CreateBuilder(args);
var useHttpsRedirection = builder.Configuration.GetValue("Security:UseHttpsRedirection", !builder.Environment.IsDevelopment());

// The gateway is the only backend origin the browser should call directly.
// Keep allowed origins in configuration so development and production can use
// different frontend hosts without changing application code.
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

        // Credentials remain disabled by default. Enable them only when the
        // deployed authentication flow explicitly requires cookie credentials.
        if (allowCredentials)
        {
            policy.AllowCredentials();
        }
    });
});

var gatewayOptions = builder.Configuration
    .GetSection(GatewayOptions.SectionName)
    .Get<GatewayOptions>()
    ?? throw new InvalidOperationException("Gateway configuration is required.");

var proxyConfiguration = GatewayProxyConfigurationBuilder.Build(gatewayOptions);

builder.Services
    .AddReverseProxy()
    .LoadFromMemory(proxyConfiguration.Routes, proxyConfiguration.Clusters);

var app = builder.Build();

// Minimal security headers for API responses. The frontend owns the document
// policy; the gateway protects the API surface and proxy responses.
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
        context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
        context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
        context.Response.Headers.TryAdd("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
        return Task.CompletedTask;
    });

    await next();
});

if (useHttpsRedirection)
{
    app.UseHttpsRedirection();
}
app.UseCors("Frontend");

app.MapReverseProxy();

app.Run();
