# INMS API Gateway – Next Tasks (YARP)

## Current status (what exists)
- `Program.cs`: YARP reverse proxy is configured.
- `appsettings.json`: **only** one route/cluster is configured:
  - `/identity/{**catch-all}` → forwards to `https://localhost:7001/`
  - removes the `/identity` prefix before forwarding.

## Missing (what to implement next)
1. **Add routes + clusters for other services**
   - Topology service
   - Alarm/Correlation service
   - AI/Analytics service

2. **Verify destination base paths**
   - Ensure each service exposes endpoints that match its controllers’ routing (e.g., `Route("api/[controller]")`).
   - Confirm the gateway path prefixes align with each service path expectations.

3. **(Optional) Add request-id / correlation header propagation**
   - Standardize `X-Request-Id` (or similar) so logs across gateway + services can be correlated.

## Ordered task list
1. Add gateway route+cluster for **topology-service**
2. Add gateway route+cluster for **alarm-correlation-service**
3. Add gateway route+cluster for **ai-logging-service / AI & Analytics service**
4. Manually verify each service works via gateway (simple smoke tests)
5. Add optional request-id header propagation for tracing
6. Auth later (JWT/login) once you decide to re-enable security scope

