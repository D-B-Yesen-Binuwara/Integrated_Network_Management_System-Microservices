# Integrated Network Management System - Microservices

A microservices-based network management system built with .NET 9, React, and modern cloud-native technologies. This system provides identity management, API gateway routing, and a responsive frontend for managing network infrastructure.

## 🏗️ Architecture Overview

This project follows a microservices architecture designed for a telecom NOC workflow (external alarms → topology-aware correlation → impact analysis → realtime visualization):

- **API Gateway** (YARP) - single entry point for the frontend; forwards requests to the correct microservice
- **Identity Service** - authentication/authorization and RBAC (users, roles, account requests, area assignments)
- **Topology Service** - manages network inventory (devices + device links) and geographic hierarchy (Region/Province/LEA)
- **Alarm & Correlation Service** (next milestone) - ingests external alarms, runs correlation rules, and produces root-cause + impacted-device results
- **AI & Analytics Service** (next milestone) - generates analytics/summaries from correlated events (MongoDB-backed)
- **Frontend** (React) - dashboard UI that consumes the gateway APIs and receives realtime updates (SignalR)


## 📁 Project Structure

```
Integrated_Network_Management_System-Microservices/
├── Backend/
│   ├── gateway/                    # API Gateway (YARP)
│   │   └── INMS.Gateway/
│   └── services/                   # Microservices
│       └── identity-service/       # Identity & User Management
│           ├── INMS.Identity.API/
│           ├── INMS.Identity.Application/
│           ├── INMS.Identity.Domain/
│           └── INMS.Identity.Infrastructure/
├── frontend/                   # React Frontend
└── Alarm_Management_System/    # Legacy monolith (excluded)
```

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
- `/identity/**` → Identity Service (localhost:7001)

### Identity Service
**Location:** `Backend/services/identity-service/`
**Technology:** ASP.NET Core 9.0 + Entity Framework Core
**Port:** 5017 (HTTP) / 7001 (HTTPS)
**Database:** PostgreSQL (development/production)

**Features:**
- User management (CRUD operations)
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
- ESLint for code quality

**Dependencies:**
- React 19.2.6
- TypeScript 6.0.2
- Redux Toolkit 2.12.0
- React Router DOM 7.15.1
- Axios 1.16.1
- Tailwind CSS 4.3.0
- SignalR 2.4.3

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