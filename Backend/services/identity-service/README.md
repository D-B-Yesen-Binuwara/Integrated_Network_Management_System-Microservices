# Identity Service — INMS.Identity

The Identity Service is the first extracted microservice of the Integrated Network Management System. It manages users, roles, account requests, and geographic area assignments. It is independently runnable with its own database and exposes a RESTful API consumed by the API Gateway and frontend.

## Architecture

This service follows Clean Architecture with four projects:

```
services/identity-service/
├── INMS.Identity.Domain/           # Entities, repository interfaces
├── INMS.Identity.Application/      # DTOs, service interfaces, business logic
├── INMS.Identity.Infrastructure/   # EF Core DbContext, repository implementations, migrations
└── INMS.Identity.API/              # ASP.NET Core controllers, DI wiring, startup
```

**Technology:** ASP.NET Core 9.0 · Entity Framework Core 7 · SQLite (dev) / SQL Server (prod)

**Ports:** `https://localhost:7001` · `http://localhost:5001` · `http://localhost:5017`

---

## Domain Layer — `INMS.Identity.Domain`

### Entities

| Entity | Key Fields |
|---|---|
| `User` | `UserId`, `Username`, `PasswordHash`, `FullName`, `ServiceId`, `Email`, `RoleId` → `Role` |
| `Role` | `RoleId`, `RoleName`, `Description` |
| `AccountRequest` | `RequestId`, `FullName`, `Email`, `ServiceId`, `RoleId` → `Role`, `RegionId` (Guid), `ProvinceId` (Guid?), `LEAId` (Guid?), `RequestedAt`, `Status` |
| `UserAreaAssignment` | `AssignmentId`, `UserId` → `User`, `AreaType` (`Region`/`Province`/`LEA`), `AreaId` (Guid) |

Geography references (`RegionId`, `ProvinceId`, `LEAId`, `AreaId`) are stored as `Guid` identifiers — no navigation properties to geography entities — to avoid cross-service coupling.

`AccountRequest.Status` values: `PENDING` · `APPROVED` · `REJECTED`

### Repository Interfaces

- `IUserRepository` — `GetAll`, `GetById`, `Create`, `Update`, `Delete`
- `IRoleRepository` — `GetAll`, `GetById`, `Create`, `Update`, `Delete`
- `IAccountRequestRepository` — `GetAll`, `GetById`, `Create`, `Update`, `Delete`
- `IUserAreaAssignmentRepository` — `GetAllByUserId`, `AssignArea`, `RemoveAssignmentsByUserId`

---

## Application Layer — `INMS.Identity.Application`

### DTOs

**User:**
- `CreateUserDto` — `FirstName`, `LastName`, `RoleId`, `ServiceId?`, `Email?`, `RegionId?`, `ProvinceId?`, `LEAId?`
- `UpdateUserDto` — `Username`, `FullName`, `RoleId`, `ServiceId?`, `Email?`
- `UserResponseDto` — `UserId`, `Username`, `FullName`, `RoleId`, `RoleName`, `ServiceId`, `Email`, `Region`, `Province`, `LEA`

**Role:**
- `RoleDto` — `RoleId`, `RoleName`, `Description`

**AccountRequest:**
- `CreateAccountRequestDto` — `FullName`, `Email`, `ServiceId`, `RoleId`, `RegionId`, `ProvinceId?`, `LEAId?`
- `UpdateAccountRequestStatusDto` — `Status`

### Services

**`UserService`** (`IUserService`)
- `GetAll()` — returns all users with role name included
- `GetById(id)` — returns a single user with role
- `Create(username, password, roleId)` — basic user creation with SHA-256 password hashing
- `CreateFromDto(dto)` — creates user from `CreateUserDto`; auto-generates username as `firstname.lastname` (with numeric suffix on collision); sets default password `DefaultPassword123!`; creates `UserAreaAssignment` records for any provided `RegionId`, `ProvinceId`, `LEAId`
- `Update(id, username, roleId)` — updates username and role
- `Delete(id)` — removes area assignments then deletes user

**`RoleService`** (`IRoleService`)
- `GetAllAsync()`, `GetByIdAsync(id)`, `CreateAsync(role)`, `UpdateAsync(id, role)`, `DeleteAsync(id)`

**`AccountRequestService`** (`IAccountRequestService`)
- `Submit(dto)` — creates a new `AccountRequest` with status `PENDING`
- `GetAll()` — returns all requests with role included
- `Approve(requestId)` — validates request is `PENDING`, checks no existing user with same email, creates a `User` (username = email, password = SHA-256 of `ServiceId`), sets status to `APPROVED`
- `Reject(requestId)` — sets status to `REJECTED`

**`UserAreaAssignmentService`** (no interface, registered directly)
- `AssignArea(userId, areaType, areaId)` — validates `areaType` is `Region`/`Province`/`LEA`, then persists assignment

---

## Infrastructure Layer — `INMS.Identity.Infrastructure`

### `IdentityDbContext`

EF Core `DbContext` with four `DbSet`s mapped to explicit table names:

| DbSet | Table |
|---|---|
| `Users` | `User` |
| `Roles` | `Role` |
| `UserAreaAssignments` | `UserAreaAssignment` |
| `AccountRequests` | `AccountRequest` |

### Repository Implementations

All repositories use `IdentityDbContext` and follow the same pattern: EF Core async operations with `Include` for navigation properties where needed.

- `UserRepository` — `GetAll`/`GetById` include `Role`; `Delete` also removes related `UserAreaAssignment` rows
- `RoleRepository` — standard CRUD
- `AccountRequestRepository` — `GetAll`/`GetById` include `Role`
- `UserAreaAssignmentRepository` — filters by `UserId`; `RemoveAssignmentsByUserId` bulk-removes

### Migrations

| Migration | Description |
|---|---|
| `20260526083307_InitialCreate` | Creates `Role`, `User`, `AccountRequest`, `UserAreaAssignment` tables with FK constraints and indexes |
| `20260526091243_ChangeGeographyIdsToGuid` | Changes geography ID columns (`RegionId`, `ProvinceId`, `LEAId`, `AreaId`) from `int` to `Guid` |

---

## API Layer — `INMS.Identity.API`

### Controllers & Endpoints

**`UserController`** — `api/user`

| Method | Route | Description |
|---|---|---|
| GET | `api/user` | List all users (with role name) |
| GET | `api/user/{id}` | Get user by ID |
| POST | `api/user` | Create user from `CreateUserDto` |
| DELETE | `api/user/{id}` | Delete user and their area assignments |

**`RoleController`** — `api/role`

| Method | Route | Description |
|---|---|---|
| GET | `api/role` | List all roles |
| POST | `api/role` | Create a role |

**`AccountRequestController`** — `api/accountrequest`

| Method | Route | Description |
|---|---|---|
| POST | `api/accountrequest` | Submit a new account request |
| GET | `api/accountrequest` | List all account requests |
| PATCH | `api/accountrequest/{id}/status` | Approve or reject a request (`{ "status": "APPROVED" \| "REJECTED" }`) |

### Startup (`Program.cs`)

- Registers `IdentityDbContext` with SQL Server if `IdentityConnection` is set in config, otherwise falls back to SQLite (`identity.db`)
- Registers all repositories and services as `Scoped`
- Enables Swagger UI (always on, including production)
- CORS policy `AllowFrontend` allows `http://localhost:5173`
- `UseHttpsRedirection`, `MapControllers`

---

## Database Configuration

### Development (SQLite — default)
No configuration needed. The service automatically creates `identity.db` in the API project directory.

### Production (SQL Server)
Add to `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "IdentityConnection": "Server=localhost;Database=INMS_Identity;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

---

## Running the Service

```bash
# Restore and build
dotnet restore services/identity-service/INMS.Identity.sln
dotnet build services/identity-service/INMS.Identity.sln

# Apply migrations (SQLite dev DB)
dotnet ef database update \
  --project services/identity-service/INMS.Identity.Infrastructure \
  --startup-project services/identity-service/INMS.Identity.API

# Run
dotnet run --project services/identity-service/INMS.Identity.API
```

- API: `https://localhost:7001`
- Swagger: `https://localhost:7001/swagger`

---

## Known Limitations / Future Work

- No authentication or authorization middleware — endpoints are currently open. Azure Entra ID (Microsoft Entra ID) is planned for authentication/SSO/MFA/token issuance.
- `UserResponseDto.Region/Province/LEA` fields are always `null` — geography name resolution requires a future Geography service.
- `UserService.GenerateUniqueUsername` uses a blocking `.Result` call inside an async context — should be refactored to fully async.
- `AccountRequestService.Approve` uses `ServiceId` as the initial password — should be replaced with a proper temporary password or invite flow.
- `IAccountRequestService.GetAll` returns `List<object>` — should be typed to a proper response DTO.
- No `PUT /api/user/{id}` endpoint exposed yet (service method exists but no controller action).
- Swagger is enabled unconditionally — should be gated behind the `Development` environment check in production.
