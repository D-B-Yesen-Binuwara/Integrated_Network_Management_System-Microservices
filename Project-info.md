# Integrated Network Management System (INMS)

# Project Overview

The Integrated Network Management System (INMS) is a microservice-based telecommunications fault monitoring and alarm correlation platform.

The system simulates how a telecommunications Network Operations Center (NOC) monitors network infrastructure, receives alarms from external systems, correlates faults using topology intelligence, identifies root causes, and analyzes cascading network impacts.

Unlike traditional monitoring systems that generate alarms internally, INMS operates as a fault correlation and analysis platform.

The system:

* receives externally generated alarms
* stores alarms
* analyzes network topology
* correlates related faults
* identifies probable root causes
* determines downstream impact chains
* visualizes network failures in realtime
* generates AI-powered summaries and analytics

---

# Core Concept

The system does NOT generate alarms internally.

Instead:

* alarms are inserted as external/dummy inputs
* the platform monitors and correlates these alarms
* topology relationships are used to infer network failures and root causes

Example:

```text
SLBN-001 -> LINK_DOWN
CEAN-003 -> NODE_UNREACHABLE
MSAN-010 -> POWER_FAILURE
```

The system analyzes:

* device relationships
* upstream/downstream links
* alarm timestamps
* geographic placement
* network hierarchy

to determine:

* probable root cause
* impacted devices
* cascading failures

---

# Main Objectives

The platform is designed to:

* simulate realistic telecom NOC operations
* demonstrate intelligent alarm correlation
* analyze cascading failures
* reduce false-positive alarms
* identify root causes using topology intelligence
* provide realtime monitoring dashboards
* support AI-assisted event analysis

---

# Technology Stack

## Frontend

* React
* Tailwind CSS
* Axios
* SignalR

## Backend

* ASP.NET Core (.NET 9)
* REST APIs
* Microservice Architecture

## Databases

* PostgreSQL
* MongoDB

## Event Communication

* RabbitMQ (future integration)

## Realtime Communication

* SignalR

## Containerization

* Docker
* Docker Compose

---

# High-Level System Architecture

```text
Frontend (React)
       |
       v
API Gateway
       |
-----------------------------------------------
|             |              |                |
v             v              v                v
Identity   Topology   Alarm & Correlation   AI & Analytics
Service     Service         Service              Service
```

---

# Core Functional Areas

# 1. Authentication & RBAC

Handles:

* user authentication
* JWT token generation
* role management
* hierarchical access control
* account approvals
* user provisioning

Supported Roles:

* Super Admin
* Platform Admin
* Regional Officer
* Province Officer
* LEA Officer

---

# 2. Device & Topology Management

Handles:

* device CRUD operations
* device-to-device links
* geographic hierarchy
* vendor management
* topology mapping

Supported Device Types:

* SLBN
* CEAN
* MSAN

Geographic Structure:

* Region
* Province
* LEA
* Device

---

# 3. Alarm Ingestion & Monitoring

Handles:

* external alarm ingestion
* alarm storage
* active alarm tracking
* realtime alarm visualization

The system assumes alarms are generated externally and inserted into the platform.

Example Alarm Types:

* NODE_DOWN
* LINK_DOWN
* POWER_FAILURE
* UNREACHABLE
* HIGH_LATENCY

---

# 4. Alarm Correlation Engine

Handles:

* fault correlation
* topology traversal
* root cause detection
* downstream impact analysis
* duplicate suppression
* cascade detection

The engine uses:

* device links
* device hierarchy
* alarm locations
* timestamps
* network topology

to determine:

* primary failures
* secondary impacts
* probable root causes

---

# 5. Impact Analysis

Handles:

* propagation analysis
* affected device tracking
* upstream/downstream analysis
* network impact visibility

Example Logic:

If:

* all parent devices are DOWN
* and child device raises connectivity alarms

Then:

* child device becomes IMPACTED
* parent becomes probable root cause

---

# 6. AI Analytics & Summarization

Handles:

* AI-generated summaries
* historical analytics
* natural language querying
* event trend analysis

Example Queries:

* "Summarize alarms in Region North"
* "Show root causes for last night's outage"
* "Display MSAN reliability statistics"

---

# Microservices

# 1. Gateway Service

## Purpose

Acts as the centralized entry point for all frontend requests.

## Responsibilities

* API routing
* request forwarding
* authentication middleware
* centralized access point

## Contains

```text
Controllers/
Program.cs
appsettings.json
```

## Database

None

## Port

5000

---

# 2. Identity Service

## Purpose

Handles authentication and access control.

## Responsibilities

* login
* JWT generation
* RBAC
* account approvals
* user provisioning

## Contains

```text
Controllers/
Services/
Repositories/
Entities/
DTOs/
Data/
Program.cs
appsettings.json
```

## Main Entities

* User
* Role
* Permission
* AccountRequest

## Database

PostgreSQL

## Port

5001

---

# 3. Topology Service

## Purpose

Manages the network topology and device hierarchy.

## Responsibilities

* device management
* device links
* vendor management
* geographic hierarchy
* network topology

## Contains

```text
Controllers/
Services/
Repositories/
Entities/
DTOs/
Data/
Program.cs
appsettings.json
```

## Main Entities

* Device
* DeviceLink
* Vendor
* Region
* Province
* LEA

## Database

PostgreSQL

## Port

5002

---

# 4. Alarm & Correlation Service

## Purpose

Core fault analysis and correlation engine.

## Responsibilities

* alarm ingestion
* alarm storage
* alarm correlation
* root cause analysis
* impact propagation
* fault suppression

## Contains

```text
Controllers/
Services/
Repositories/
Entities/
DTOs/
Data/
Correlation/
Program.cs
appsettings.json
```

## Main Entities

* SLBN_Alarm
* CEAN_Alarm
* MSAN_Alarm
* RootCause
* ImpactedDevice
* CorrelatedFault

## Database

PostgreSQL

## Alarm Table Structure

Separate alarm tables are maintained for each device type:

```text
SLBN_Alarms
CEAN_Alarms
MSAN_Alarms
```

This separation exists because:

* alarm structures may differ
* vendors differ
* priorities differ
* correlation rules differ
* metadata differs

## Port

5003

---

# 5. AI & Analytics Service

## Purpose

Handles analytics and intelligent event summarization.

## Responsibilities

* AI summaries
* historical reporting
* analytics
* event searching
* natural language queries

## Contains

```text
Controllers/
Services/
Repositories/
Entities/
DTOs/
Data/
AI/
Program.cs
appsettings.json
```

## Main Entities

* EventLog
* AISummary
* AnalyticsReport

## Database

MongoDB

## Port

5004

---

# Realtime Communication

SignalR is used for:

* live alarm updates
* realtime dashboard updates
* network status changes
* active fault visualization

---

# Database Architecture

# PostgreSQL

Used for:

* users
* topology
* devices
* links
* alarms
* correlation results
* impacted devices

---

# MongoDB

Used for:

* event logs
* AI summaries
* analytics
* historical searches

---

# Folder Structure

```text
Integrated_Network_Management_System-Microservices/
│
├── frontend/
│
├── backend/
│   │
│   ├── gateway/
│   │
│   ├── services/
│   │   ├── identity-service/
│   │   ├── topology-service/
│   │   ├── alarm-correlation-service/
│   │   └── ai-logging-service/
│   │
│   └── shared/
│
├── docker-compose.yml
└── README.md
```

---

# Current MVP Scope

## Included

* device management
* topology management
* external alarm ingestion
* alarm correlation
* root cause analysis
* impact propagation
* RBAC
* realtime dashboard
* AI summaries

## Excluded

* Kubernetes
* enterprise event streaming
* distributed tracing
* advanced event sourcing
* multi-region scaling
* HA clustering

---

# Main Technical Challenge

The primary complexity of the project lies in:

* topology traversal
* fault correlation
* root cause identification
* cascading impact analysis
* multi-parent relationship validation

The project is NOT primarily CRUD-focused.

---

# Future Enhancements

Potential future improvements:

* RabbitMQ integration
* Neo4j graph database
* predictive failure analysis
* distributed caching
* advanced analytics
* Kubernetes deployment
* telecom protocol integrations
* SNMP trap ingestion

---

# Project Goal

The goal of INMS is to simulate a realistic telecommunications fault correlation and network analysis platform capable of:

* monitoring telecom alarms
* intelligently correlating faults
* identifying probable root causes
* analyzing cascading network impacts
* visualizing network failures in realtime
* demonstrating distributed system architecture principles
