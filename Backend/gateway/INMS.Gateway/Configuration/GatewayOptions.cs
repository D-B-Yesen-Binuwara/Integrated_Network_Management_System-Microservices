namespace INMS.Gateway.Configuration;

// Binds gateway service definitions from configuration.
public sealed class GatewayOptions
{
    public const string SectionName = "Gateway";

    public Dictionary<string, GatewayServiceOptions> Services { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

// Holds route and destination settings for one downstream service.
public sealed class GatewayServiceOptions
{
    public string RoutePrefix { get; set; } = string.Empty;

    public Dictionary<string, string> Destinations { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
