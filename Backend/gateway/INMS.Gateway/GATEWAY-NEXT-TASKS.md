# INMS API Gateway - Current State and Next Tasks

## Current status
- `Program.cs` loads YARP from an in-memory proxy configuration built from the `Gateway` configuration section.
- Route and cluster generation is centralized in `Configuration/GatewayProxyConfigurationBuilder.cs`.
- The gateway currently exposes:
  - `/identity/{**catch-all}`
  - `/topology/{**catch-all}`
  - `/alarm/{**catch-all}`
- Each public prefix is removed before forwarding requests to the target service.
- Shared gateway structure lives in `appsettings.json`.
- Environment-specific service destination URLs live in `appsettings.Development.json`.
- Service destinations are configuration-driven, not hard-coded in gateway code.

## Current development mappings
- `/identity` -> `https://localhost:7001/`
- `/topology` -> `https://localhost:7248/`
- `/alarm` -> `https://localhost:7101/`

## Validation already in place
- Missing service definitions fail fast at startup.
- Empty service keys fail fast at startup.
- Invalid or duplicate route prefixes fail fast at startup.
- Missing or invalid destination URLs fail fast at startup.

## Verified
- `INMS.Gateway` builds successfully.
- `alarm-service` builds successfully after removing the fallback topology URL from code.

## Next tasks
1. Add new service entries under `Gateway:Services` when more backend services are introduced.
2. Add smoke tests for gateway route forwarding.
3. Add correlation or request-id header propagation for tracing across services.
4. Add production environment destination configuration.
5. Add gateway-level authentication and authorization when the security scope is finalized.
