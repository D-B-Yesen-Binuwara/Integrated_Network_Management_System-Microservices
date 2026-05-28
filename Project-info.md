# Integrated Network Management System (INMS)

## Project Overview

The Integrated Network Management System (INMS) is a web-based telecommunications network monitoring and fault analysis platform designed using a microservice architecture.

The system simulates how a real-world Network Operations Center (NOC) monitors network infrastructure, detects failures, analyzes cascading impacts, identifies root causes, and provides real-time operational visibility.

The platform supports multiple network device layers including:

* SLBN (Backbone Layer)
* CEAN (Exchange Layer)
* MSAN (Access Layer)

The system is designed for:

* real-time monitoring
* intelligent fault propagation
* root cause analysis
* alarm management
* role-based administration
* AI-powered event summarization

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

* RabbitMQ
* MassTransit

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
------------------------------------------------
|         |            |           |            |
v         v            v           v            v
Identity  Topology   Monitoring  Analysis   AI/Logging
Service    Service     Service    Service     Service
```

---

# Core Functional Areas

## 1. Authentication & RBAC

Handles:

* user authentication
* JWT token generation
* role-based access control
* account approvals
* user provisioning

Supported Roles:

* Super Admin
* Platform Admin
* Regional Officer
* Province Officer
* LEA Officer

---

## 2. Device & Topology Management

Handles:

* device CRUD operations
* device-to-device links
* vendor management
* geographic hierarchy
* network topology management

Geographic Structure:

* Region
* Province
* LEA
* Device

Supported Device Types:

* SLBN
* CEAN
* MSAN

---

## 3. Real-Time Monitoring

Handles:

* heartbeat generation
* heartbeat monitoring
* device timeout detection
* alarm generation
* realtime device status updates

Device States:

* UP
* DOWN
* UNREACHABLE
* IMPACTED

---

## 4. Root Cause & Impact Analysis

Handles:

* topology traversal
* multi-parent route validation
* cascading failure analysis
* root cause detection
* impact propagation

Analysis Logic:

* If all upstream parents fail:

  * downstream device becomes UNREACHABLE
* If at least one parent path is alive:

  * downstream device remains UP

---

## 5. Alarm Management

Handles:

* alarm creation
* alarm clearing
* severity classification
* active alarm tracking
* alarm history

Alarm Severities:

* Critical
* High
* Medium
* Low

NOTE:
alarms from each device type is recorded in their own databse / table [which ever is the most effiient way]
---

## 6. Simulation Engine

Handles:

* controlled failure injection
* deterministic testing scenarios
* simulated device shutdowns
* impact propagation testing

Purpose:

* testing
* demonstrations
* training
* validation

---

## 7. AI Analytics & Logging

Handles:

* event logging
* AI-generated summaries
* historical analysis
* natural language querying

Example Queries:

* "Summarize alarms in Region North"
* "Show root causes for last night's outage"
* "Display MSAN reliability statistics"

---

# Microservices

## 1. Gateway Service

### Responsibilities

* API routing
* authentication middleware
* request forwarding
* centralized entry point

### Port

5000

---

## 2. Identity Service

### Responsibilities

* authentication
* JWT management
* RBAC
* user management

### Database

PostgreSQL

### Port

5001

---

## 3. Topology Service

### Responsibilities

* devices
* device links
* geographic hierarchy
* vendors

### Database

PostgreSQL

### Port

5002

---

## 4. Monitoring Service

### Responsibilities

* heartbeat monitoring
* timeout detection
* alarm generation
* realtime updates

### Database

PostgreSQL

### Port

5003

---

## 5. Analysis Service

### Responsibilities

* impact analysis
* root cause detection
* propagation algorithms
* simulation analysis

### Database

PostgreSQL

### Port

5004

---

## 6. AI & Logging Service

### Responsibilities

* event logging
* AI summarization
* analytics
* query processing

### Database

MongoDB

### Port

5005

---

# Event-Driven Communication

RabbitMQ is used for asynchronous communication between services.

Example Event Flow:

```text
Monitoring Service
    ->
DeviceTimedOutEvent
    ->
RabbitMQ
    ->
Analysis Service
    ->
RootCauseDetectedEvent
    ->
RabbitMQ
    ->
Monitoring Service
    ->
Alarm Created
```

---

# Realtime Communication

SignalR is used for:

* live dashboard updates
* realtime alarm notifications
* device status updates

---

# Current Scope

## Included in MVP

* device management
* topology management
* heartbeat monitoring
* alarm generation
* impact analysis
* root cause detection
* role-based access
* AI summaries
* realtime dashboard

## Excluded from MVP

* Kubernetes deployment
* distributed tracing
* advanced event sourcing
* multi-region scaling
* enterprise observability stack
* high-availability clustering

---

# Development Principles

The project prioritizes:

* modularity
* simplicity
* maintainability
* fast iteration
* realistic telecom simulation

The system intentionally avoids unnecessary enterprise-level complexity during the MVP phase.

---

# Backend Folder Structure

```text
backend/
│
├── gateway/
├── services/
│   ├── identity-service/
│   ├── topology-service/
│   ├── monitoring-service/
│   ├── analysis-service/
│   └── ai-logging-service/
```

---

# Main Technical Challenges

The primary complexity of the project lies in:

* network topology traversal
* cascading failure detection
* realtime synchronization
* fault propagation logic
* multi-parent route validation

The project is NOT primarily CRUD-focused.

---

# Future Improvements

Potential future enhancements:

* Kubernetes deployment
* Neo4j graph database
* advanced analytics
* predictive failure analysis
* distributed caching
* advanced observability
* service scaling
* mobile monitoring app

---

# Project Goal

The goal of INMS is to simulate a realistic telecommunications network management platform capable of:

* monitoring infrastructure in realtime
* detecting failures intelligently
* analyzing impact chains
* assisting operators through AI-powered insights
* demonstrating scalable distributed system design
