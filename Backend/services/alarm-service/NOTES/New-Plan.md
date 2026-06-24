# Revised Correlation Engine Plan

## Current State

### What exists

**topology-service** (port 7248 / 5102)

- `Device` entity: `DeviceId`, `DeviceName`, `DeviceType` (SLBN=0, CEAN=1, MSAN=2, Customer=3), `IP`, `Status`, `PriorityLevel`, `Latitude`, `Longitude`
- `DeviceLink` entity: `LinkId`, `ParentDeviceId`, `ChildDeviceId`, `LinkStatus`
- `DeviceLinkService` already has `WouldCreateCycleAsync` using recursive CTE — cycle logic lives here
- `IsValidTopology` enforces SLBN→CEAN, CEAN→MSAN/Customer, MSAN→Customer hierarchy
- Existing endpoints:
  - `GET /api/device` — all devices
  - `GET /api/device/{id}` — single device
  - `GET /api/device-link` — all links (flat list, no graph traversal)
  - `POST /api/device-link` — create link
  - `DELETE /api/device-link/{id}` — delete link
- Missing: no parent/child/ancestor/descendant traversal endpoints

**alarm-service** (Day 1 + Day 2 done)

- `CorrelationRule`, `CorrelationResult`, `CorrelationContext` models
- `RuleLoader` — loads and caches JSON rules at startup
- `RootCauseEngine` — matches alarm context against rules, returns root cause
- Correlation/Rules JSON files with SLBN, CEAN, MSAN rules
- Missing: no topology traversal, no `ImpactAnalysisEngine`, no `TopologyClient`

---

## Architecture Decision

Topology traversal (parents, children, ancestors, descendants) belongs in **topology-service**.

The alarm-service calls topology-service APIs over HTTP. It never traverses the graph itself.

The `DeviceLink` table already stores the parent→child relationships needed for all traversal.

---

## What Needs to Be Built

---

### Part A — topology-service (must be done first)

#### A1. Repository layer — add traversal queries to `IDeviceLinkRepository`

Add to `IDeviceLinkRepository`:

```csharp
Task<List<DeviceLink>> GetChildLinksAsync(int parentDeviceId);
Task<List<DeviceLink>> GetParentLinksAsync(int childDeviceId);
```

Implement in `DeviceLinkRepository` using EF — filter `DeviceLinks` by `ParentDeviceId` or `ChildDeviceId` respectively. Include the related `Device` navigation property so callers get `DeviceType`.

---

#### A2. Service layer — add traversal methods to `IDeviceLinkService`

Add to `IDeviceLinkService`:

```csharp
Task<List<DeviceDto>> GetChildrenAsync(int deviceId);
Task<List<DeviceDto>> GetParentsAsync(int deviceId);
Task<List<DeviceDto>> GetDescendantsAsync(int deviceId);
Task<List<DeviceDto>> GetAncestorsAsync(int deviceId);
```

Implement in `DeviceLinkService`:

- `GetChildrenAsync` — one-level: return child devices of the given device
- `GetParentsAsync` — one-level: return parent devices of the given device (supports multi-parent)
- `GetDescendantsAsync` — recursive DFS downward using `GetChildLinksAsync`, visited `HashSet<int>` to prevent cycles
- `GetAncestorsAsync` — recursive DFS upward using `GetParentLinksAsync`, visited `HashSet<int>` to prevent cycles

The recursive traversal algorithms live here, inside topology-service. The alarm-service never traverses — it just calls these endpoints.

---

#### A3. Controller layer — expose traversal endpoints on `DeviceController`

Add to `DeviceController`:

```
GET /api/device/{id}/children     → one-level children
GET /api/device/{id}/parents      → one-level parents (multi-parent supported)
GET /api/device/{id}/descendants  → all downstream devices recursively
GET /api/device/{id}/ancestors    → all upstream devices recursively
```

All return `List<DeviceDto>` (same shape as existing `GET /api/device/{id}`).

Note: these go on `DeviceController` (not `DeviceLinkController`) because they answer "what devices are related to this device" — a device-centric question.

---

### Part B — alarm-service

#### B1. Add `TopologyDeviceDto` to alarm-service

A local DTO inside alarm-service that mirrors the topology-service `DeviceDto` response shape:

```csharp
// Correlation/Models/TopologyDeviceDto.cs
public class TopologyDeviceDto
{
    public int DeviceId { get; set; }
    public string DeviceName { get; set; }
    public string DeviceType { get; set; }  // string because enum values differ between services
    public string Status { get; set; }
}
```

No project reference to topology-service. Deserialized from HTTP response.

---

#### B2. Add `ITopologyClient` and `TopologyClient`

```csharp
// Correlation/Topology/ITopologyClient.cs
public interface ITopologyClient
{
    Task<TopologyDeviceDto?> GetDeviceAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetChildrenAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetParentsAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetDescendantsAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetAncestorsAsync(int deviceId);
}
```

```csharp
// Correlation/Topology/TopologyClient.cs
```

Uses `HttpClient`. Base URL read from `appsettings.json` under `TopologyService:BaseUrl`.
Registered as typed `HttpClient` in `Program.cs`.
Returns `null` / empty list on failure — never throws.

---

#### B3. Implement `ImpactAnalysisEngine`

```csharp
// Correlation/Engine/ImpactAnalysisEngine.cs
```

Takes a `CorrelationContext` and matched `CorrelationRule`. Calls `_topologyClient.GetDescendantsAsync(context.DeviceId)` to get all downstream devices. Populates `CorrelationResult.ImpactedDevices`.

No local graph traversal. No visited set needed here — traversal and cycle detection are done inside topology-service.

---

#### B4. Add `TopologyService:BaseUrl` to `appsettings.json`

```json
"TopologyService": {
  "BaseUrl": "https://localhost:7248"
}
```

---

## Implementation Order

```
1. A1 — DeviceLinkRepository: GetChildLinksAsync, GetParentLinksAsync
2. A2 — DeviceLinkService: GetChildrenAsync, GetParentsAsync, GetDescendantsAsync, GetAncestorsAsync
3. A3 — DeviceController: 4 new endpoints
4. B4 — alarm-service appsettings: add TopologyService:BaseUrl
5. B1 — TopologyDeviceDto
6. B2 — ITopologyClient + TopologyClient
7. B3 — ImpactAnalysisEngine
8. Register TopologyClient and ImpactAnalysisEngine in alarm-service Program.cs
```

---

## What Does NOT Change

- `RuleLoader`, `RootCauseEngine`, correlation models — no changes needed
- `DeviceLink` entity — no changes needed
- `Device` entity — no changes needed
- Existing topology-service endpoints — no changes, only additions
- Existing alarm-service alarm domain (CEA/MSAN/SLBN services/controllers) — no changes

---

## Open Questions Before Implementing

1. Does the `DeviceLink` navigation property (`ParentDevice`, `ChildDevice`) get eagerly loaded currently, or does the repository need `.Include()` added for traversal queries?
   - Looking at `DeviceLinkRepository.GetAllAsync()` — it uses `AsNoTracking()` with no `.Include()`. The traversal queries will need `.Include(l => l.ChildDevice)` and `.Include(l => l.ParentDevice)` to avoid null navigation properties.

2. `DeviceType` in topology-service is an `int` enum. The alarm-service `CorrelationRule` uses string values like `"SLBN"`, `"CEAN"`, `"MSAN"`. The `TopologyDeviceDto` in alarm-service should deserialize `DeviceType` as an `int` and map it to the string names to match against rule `TargetDeviceType`.

3. The `GET /api/device-link` (all links) endpoint already exists. The new traversal endpoints are additions — `GET /api/device-link` is not changed or replaced.
