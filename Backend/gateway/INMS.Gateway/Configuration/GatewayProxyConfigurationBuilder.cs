using Yarp.ReverseProxy.Configuration;

namespace INMS.Gateway.Configuration;

// Builds validated YARP proxy definitions from configured services.
internal static class GatewayProxyConfigurationBuilder
{
    // Converts service config entries into YARP routes and clusters.
    public static ProxyConfiguration Build(GatewayOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Services.Count == 0)
        {
            throw new InvalidOperationException("At least one gateway service must be configured.");
        }

        var routes = new List<RouteConfig>();
        var clusters = new List<ClusterConfig>();
        var registeredPrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (serviceKey, serviceOptions) in options.Services.OrderBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(serviceKey))
            {
                throw new InvalidOperationException("Gateway service keys cannot be empty.");
            }

            var routePrefix = NormalizeRoutePrefix(serviceOptions.RoutePrefix, serviceKey);

            if (!registeredPrefixes.Add(routePrefix))
            {
                throw new InvalidOperationException($"Duplicate gateway route prefix '{routePrefix}' is not allowed.");
            }

            routes.Add(new RouteConfig
            {
                RouteId = $"{serviceKey}-route",
                ClusterId = $"{serviceKey}-cluster",
                Match = new RouteMatch
                {
                    Path = $"{routePrefix}/{{**catch-all}}"
                },
                Transforms =
                [
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["PathRemovePrefix"] = routePrefix
                    }
                ]
            });

            clusters.Add(new ClusterConfig
            {
                ClusterId = $"{serviceKey}-cluster",
                Destinations = BuildDestinations(serviceKey, serviceOptions.Destinations)
            });
        }

        return new ProxyConfiguration(routes, clusters);
    }

    // Validates and normalizes the public route prefix for a service.
    private static string NormalizeRoutePrefix(string routePrefix, string serviceKey)
    {
        if (string.IsNullOrWhiteSpace(routePrefix))
        {
            throw new InvalidOperationException($"Gateway route prefix is required for service '{serviceKey}'.");
        }

        if (!routePrefix.StartsWith('/'))
        {
            throw new InvalidOperationException($"Gateway route prefix '{routePrefix}' for service '{serviceKey}' must start with '/'.");
        }

        var normalizedPrefix = routePrefix.TrimEnd('/');

        if (string.IsNullOrWhiteSpace(normalizedPrefix) || normalizedPrefix == "/")
        {
            throw new InvalidOperationException($"Gateway route prefix for service '{serviceKey}' must be a non-root path.");
        }

        return normalizedPrefix;
    }

    // Converts configured service URLs into YARP destination objects.
    private static IReadOnlyDictionary<string, DestinationConfig> BuildDestinations(
        string serviceKey,
        IReadOnlyDictionary<string, string>? destinations)
    {
        if (destinations is null || destinations.Count == 0)
        {
            throw new InvalidOperationException($"At least one destination must be configured for service '{serviceKey}'.");
        }

        var configuredDestinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase);

        foreach (var (destinationKey, address) in destinations)
        {
            if (string.IsNullOrWhiteSpace(destinationKey))
            {
                throw new InvalidOperationException($"Destination keys cannot be empty for service '{serviceKey}'.");
            }

            if (!Uri.TryCreate(EnsureTrailingSlash(address), UriKind.Absolute, out var destinationUri))
            {
                throw new InvalidOperationException(
                    $"Destination '{destinationKey}' for service '{serviceKey}' must be a valid absolute URI.");
            }

            configuredDestinations[destinationKey] = new DestinationConfig
            {
                Address = destinationUri.ToString()
            };
        }

        return configuredDestinations;
    }

    // Ensures base addresses are safe for downstream path composition.
    private static string EnsureTrailingSlash(string address) =>
        string.IsNullOrWhiteSpace(address)
            ? string.Empty
            : address.EndsWith('/') ? address : $"{address}/";

    // Keeps generated routes and clusters together as one result.
    internal sealed record ProxyConfiguration(
        IReadOnlyList<RouteConfig> Routes,
        IReadOnlyList<ClusterConfig> Clusters);
}
