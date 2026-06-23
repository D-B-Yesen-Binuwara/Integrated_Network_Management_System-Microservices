# alarm-service TODO

## Database isolation + domain split (MSAN/SLBN/CEA)
- [x] Gather authoritative Alarm entity shape from Temp monolithic implementation.
- [x] Confirm current alarm-service implementation (Alarm entity + single Alarm table + CRUD/filtering/active endpoints).
- [ ] Create/point to dedicated database `INMS_Alarm` (connection strings + design-time factory).
- [ ] Replace single `Alarm` entity/table with `MSANAlarm`, `SLBNAlarm`, `CEAAlarm` entities.
- [ ] Update `AlarmDbContext` to expose `DbSet<MSANAlarm>`, `DbSet<SLBNAlarm>`, `DbSet<CEAAlarm>`.
- [ ] Update repository/service/controller layers to be domain-specific:
  - [ ] Repositories: `IMSANAlarmRepository`, `ISLBNAlarmRepository`, `ICEAAlarmRepository` + implementations.
  - [ ] Services: `IMSANAlarmService`, `ISLBNAlarmService`, `ICEAAlarmService` + implementations.
  - [ ] Controllers: `MSANAlarmController`, `SLBNAlarmController`, `CEAAlarmController`.
- [ ] Replace DTOs with MSAN/SLBN/CEA request/response DTOs.
- [ ] Remove old `Alarm` endpoints/routes.

## Migrations
- [ ] Remove old migrations (AlarmInitialCreate / AlarmDomainInit) after confirming no longer needed.
- [ ] Add new migration `InitialAlarmDomainSchema`.
- [ ] Apply `dotnet ef database update` against `INMS_Alarm`.

## Startup migration application
- [ ] Add `context.Database.Migrate()` on startup (or a safe guarded variant).

## Build validation
- [ ] `dotnet build` for alarm-service.
- [ ] Run service and validate Swagger endpoints.

