# Integrated Network Management System - Microservices

A microservices-based network management system built with .NET 9, React, and modern cloud-native technologies. This system provides identity management, API gateway routing, and a responsive frontend for managing network infrastructure.

## 🏗️ Architecture Overview

This project follows a microservices architecture designed for a telecom NOC workflow (external alarms → topology-aware correlation → impact analysis → realtime visualization):

- **API Gateway** (YARP) - single entry point for the frontend; forwards requests to the correct microservice
- **Identity Service** - authentication/authorization and RBAC (users, roles, account requests, area assignments)
- **Topology Service** - manages network inventory (devices + device links) and geographic hierarchy (Region/Province/LEA)
- **Alarm & Correlation Service** - ingests SLBN/CEAN/MSAN alarms, runs correlation rules, and produces root-cause + impacted-device results
- **AI & Analytics Service** (planned) - generates analytics/summaries from correlated events (MongoDB-backed)
- **Frontend** (React) - dashboard UI that consumes gateway APIs and displays live alarm, topology, correlation, and map views


## 📁 Project Structure

```
Integrated_Network_Management_System-Microservices/
├── Backend/
│   ├── gateway/                    # API Gateway (YARP)
│   │   └── INMS.Gateway/
│   └── services/                   # Microservices
│       ├── alarm-service/           # Alarm ingestion and correlation
│       ├── topology-service/        # Devices, links, and geographic coordinates
│       └── identity-service/        # Identity & User Management
│           ├── INMS.Identity.API/
│           ├── INMS.Identity.Application/
│           ├── INMS.Identity.Domain/
│           └── INMS.Identity.Infrastructure/
├── frontend/                   # React Frontend
└── Alarm_Management_System/    # Legacy monolith (excluded)
```

## 🧭 Local Development (HTTP)

The current local setup uses HTTP so a trusted ASP.NET HTTPS developer certificate is not required. The frontend calls the gateway at `http://localhost:5253`; it must not call individual microservices directly.

Start each process in a separate terminal from the repository root:

```powershell
dotnet run --project Backend/services/topology-service/topology-service.csproj --launch-profile http
dotnet run --project Backend/services/alarm-service/alarm-service.csproj --launch-profile alarm-service --urls http://localhost:5101
dotnet run --project Backend/services/identity-service/INMS.Identity.API/INMS.Identity.API.csproj --launch-profile http
dotnet run --project Backend/gateway/INMS.Gateway/INMS.Gateway.csproj --launch-profile http
cd frontend
npm run dev
```

The frontend reads `frontend/.env.local`:

```env
VITE_API_BASE_URL=http://localhost:5253
VITE_API_USE_COOKIES=false
```

The alarm service depends on the topology service for topology-aware impact analysis. PostgreSQL must be running and populated using the manual SQL files in `database-scripts/` before service-backed pages can return records. The employee, account-request, and device-registration screens also require the identity service.

## 🚀 Services

### API Gateway
**Location:** `Backend/gateway/INMS.Gateway/`
**Technology:** ASP.NET Core 9.0 + YARP Reverse Proxy
**Port:** 5253 (HTTP) / 7030 (HTTPS)

**Features:**
- Route-based request forwarding
- Path prefix transformation
- Load balancing capabilities
- CORS support

**Routes:**
- `/identity/**` → Identity Service (localhost:5017 in local HTTP development / 7001 HTTPS)
- `/topology/**` → Topology Service (localhost:5102 in local HTTP development)
- `/alarm/**` → Alarm & Correlation Service (localhost:5101 in local HTTP development)

The gateway removes the public service prefix before forwarding. For example, `/topology/api/device` is forwarded to `http://localhost:5102/api/device`.

### Identity Service
**Location:** `Backend/services/identity-service/`
**Technology:** ASP.NET Core 9.0 + Entity Framework Core
**Port:** 5017 (HTTP) / 7001 (HTTPS)
**Database:** PostgreSQL (development/production)

**Features:**
- User management (CRUD operations)
- Regional, provincial, and LEA engineer/officer assignments
- Role-based access control
- Account request processing
- Area-based user assignments
- Password hashing and security
- RESTful API with Swagger documentation

**Entities:**
- `User` - System users with roles and area assignments
- `Role` - User roles and permissions
- `AccountRequest` - New account registration requests
- `UserAreaAssignment` - Geographic area assignments

**API Endpoints:**
- `GET /api/user` - List all users
- `GET /api/user/{id}` - Get user by ID
- `POST /api/user` - Create new user
- `DELETE /api/user/{id}` - Delete user
- `GET /api/role` - List all roles
- `POST /api/accountrequest` - Submit account request
- `GET /api/accountrequest` - List account requests
- `PATCH /api/accountrequest/{id}/status` - Approve or reject an account request

### Topology Service
**Location:** `Backend/services/topology-service/`
**Port:** 5102 (HTTP) / 7248 (HTTPS)
**Database:** PostgreSQL

**API Endpoints:**
- `GET /api/region` - List regions with region codes
- `GET /api/province` - List provinces with province codes and their region
- `GET /api/lea` - List LEAs with LEA codes and their province/region
- `GET /api/device` - List devices with IP, status, priority, coordinates, hierarchy codes, and assigned engineer
- `GET /api/device/{id}` - Get one device
- `GET /api/device-link` - List parent-child device links
- `POST /api/device-link` - Create a device link
- `DELETE /api/device-link/{id}` - Delete a device link

### Alarm & Correlation Service
**Location:** `Backend/services/alarm-service/`
**Port:** 5101 (HTTP) / 7101 (HTTPS)
**Database:** PostgreSQL

**API Endpoints:**
- `GET /api/slbn-alarms/active` - List active SLBN alarms
- `GET /api/cea-alarms/active` - List active CEAN alarms
- `GET /api/msan-alarms/active` - List active MSAN alarms
- `GET /api/correlation/faults` - List persisted correlated faults
- `GET /api/correlation/impacted-devices` - List impacted devices

### Frontend Application
**Location:** `frontend/`
**Technology:** React 19 + TypeScript + Vite
**Port:** 5173 (development)

**Features:**
- Modern React with hooks and functional components
- TypeScript for type safety
- Redux Toolkit for state management
- React Router for navigation
- Axios for HTTP requests
- Tailwind CSS for styling
- SignalR for real-time communication
- Leaflet map with device-type markers, topology links, alarm/status colors, and device popups
- Persistent light mode and blue-purple gradient dark mode
- ESLint for code quality

The topology map uses live `latitude` and `longitude` values from the topology service. Alarm table IP addresses and priority levels are joined from topology device metadata using `deviceId`; no frontend fixture data is used.

Region management verifies LEA engineer coverage, and the inventory form requires an LEA plus a matching assigned LEA engineer; region and province values are derived by the topology service from the LEA code.

The pinned Leaflet JavaScript runtime is loaded from the CDN in `frontend/index.html`; the required CSS surface is local in `frontend/src/leaflet.css`. The browser still needs internet access for the runtime and OpenStreetMap tiles.

**Dependencies:**
- React 19.2.6
- TypeScript 6.0.2
- Redux Toolkit 2.12.0
- React Router DOM 7.15.1
- Axios 1.16.1
- Tailwind CSS 4.3.0
- SignalR 2.4.3
- Leaflet 1.9.4 via CDN

## 🛠️ Development Setup

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+ and npm
- PostgreSQL 14+
- Visual Studio 2022 or VS Code

### Backend Setup

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd Integrated_Network_Management_System-Microservices
   ```

2. **Build Identity Service:**
   ```bash
   dotnet restore Backend/services/identity-service/INMS.Identity.sln
   dotnet build Backend/services/identity-service/INMS.Identity.sln
   ```

3. **Setup Database (Identity Service):**
   ```bash
   # Create migration (if needed)
   dotnet ef migrations add InitialCreate --project Backend/services/identity-service/INMS.Identity.Infrastructure --startup-project Backend/services/identity-service/INMS.Identity.API
   
   # Apply migration
   dotnet ef database update --project Backend/services/identity-service/INMS.Identity.Infrastructure --startup-project Backend/services/identity-service/INMS.Identity.API
   ```

4. **Build Gateway:**
   ```bash
   dotnet restore Backend/gateway/INMS.Gateway/INMS.Gateway.csproj
   dotnet build Backend/gateway/INMS.Gateway/INMS.Gateway.csproj
   ```

### Frontend Setup

1. **Install dependencies:**
   ```bash
   cd frontend
   npm install
   ```

2. **Start development server:**
   ```bash
   npm run dev
   ```

### Running the Application

For the current dashboard pages, start the topology service, alarm service, identity service, gateway, and frontend using the commands in [Local Development (HTTP)](#-local-development-http). Login is intentionally not implemented yet, but the identity service is needed by employee/account management and device assignment screens.

If using HTTPS instead, start the services with their HTTPS profiles, set `VITE_API_BASE_URL=https://localhost:7030`, and trust the local development certificate with `dotnet dev-certs https --trust`.

The individual HTTPS endpoints are:

- Gateway: `https://localhost:7030`
- Alarm service: `https://localhost:7101`
- Topology service: `https://localhost:7248`
- Identity service: `https://localhost:7001`

The older identity-only example below is retained for reference.

1. **Start Identity Service:**
   ```bash
   dotnet run --project Backend/services/identity-service/INMS.Identity.API
   ```
   - API: https://localhost:7001
   - Swagger: https://localhost:7001/swagger

2. **Start API Gateway:**
   ```bash
   dotnet run --project Backend/gateway/INMS.Gateway
   ```
   - Gateway: https://localhost:7030

3. **Start Frontend:**
   ```bash
   cd frontend
   npm run dev
   ```
   - Frontend: http://localhost:5173

## 🗄️ Database Configuration

### PostgreSQL
Connection string in `Backend/services/identity-service/INMS.Identity.API/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "IdentityConnection": "Host=localhost;Port=5432;Database=INMS_Identity;Username=postgres;Password=<password>"
  }
}
```

## 🔧 Configuration

### Gateway Configuration
**File:** `Backend/gateway/INMS.Gateway/appsettings.json`

- Configure routes and clusters for service discovery
- Add new microservices by updating the ReverseProxy section

### Identity Service Configuration
**File:** `Backend/services/identity-service/INMS.Identity.API/appsettings.json`

- Database connection strings
- CORS policies
- Logging configuration

## 🧪 Testing

### API Testing
- Use Swagger UI for interactive API testing
- HTTP files available in each service for manual testing
- Identity Service: `INMS.Identity.API.http`
- Gateway: `INMS.Gateway.http`

### Frontend Testing
```bash
cd frontend
npm run lint    # ESLint code quality check
npm run build   # Production build test
```

## 🐞 Troubleshooting

### `GET /topology/api/device-link` returns 404

The gateway should forward this request to `http://localhost:5102/api/device-link`. Ensure the topology service is rebuilt/restarted after route changes and that `DeviceLinkController` uses the explicit `api/device-link` route. A healthy gateway log will show a downstream `200` response instead of `Received HTTP/1.1 response 404`.

### Edge reports “Tracking Prevention blocked access to storage”

The Leaflet CSS is local, so this notice should no longer reference the stylesheet. If Edge still reports it, it is for the pinned Leaflet JavaScript runtime or OpenStreetMap tiles; it is separate from the API and does not explain a backend 404. The map requires browser access to those external runtime/tile URLs.

### React DevTools message

`Download the React DevTools...` is an informational development message, not an application error.

### HTTP development warning about HTTPS redirection

The HTTP development configuration disables HTTPS redirection because the local frontend uses `http://localhost:5253`. Production defaults still enable HTTPS redirection. For HTTPS local development, use the HTTPS profiles and configure `VITE_API_BASE_URL=https://localhost:7030`.

## 🚀 Deployment

### Docker Support
*Coming soon - Docker configurations for containerized deployment*

### Production Considerations
- Use SQL Server for production databases
- Configure proper connection strings
- Enable HTTPS certificates
- Set up proper logging and monitoring
- Configure environment-specific settings

## 🔐 Security Features

- Password hashing using secure algorithms
- Role-based access control
- CORS configuration for cross-origin requests
- HTTPS enforcement
- Input validation and sanitization

## 📈 Future Enhancements

- **Alarm & Correlation Service completion** - external alarm ingestion, rule-based correlation, and impact propagation (MVP loop)
- **Notification Service** - real-time alerts and notifications (SignalR)
- **Reporting Service** - analytics and reporting capabilities (time-based insights, outage summaries)
- **Authentication Service** - JWT-based authentication hardening and distributed auth integration
- **Configuration Service** - centralized configuration management

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

*License information to be added*

## 📞 Support

For questions and support, please contact the development team or create an issue in the repository.

---

**Note:** This project is actively under development. The Alarm_Management_System folder contains legacy monolith code and is excluded from the microservices architecture.
