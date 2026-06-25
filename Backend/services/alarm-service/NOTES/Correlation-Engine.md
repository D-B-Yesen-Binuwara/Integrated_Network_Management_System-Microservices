# Alarm Service Correlation Engine Design

## Overview

This document describes the design and implementation approach for the fault correlation engine inside the `alarm-service` microservice.

The project is a telecommunications fault monitoring and analysis platform.

The system DOES NOT generate alarms.

Alarms are assumed to originate from external systems or dummy data sources and are stored in alarm tables.

The responsibility of the correlation engine is to:

* analyze alarms
* traverse topology relationships
* identify probable root causes
* determine impacted devices
* suppress secondary alarms
* generate correlation results

---

# Architectural Goal

The correlation engine must be:

* configurable
* extensible
* maintainable
* vendor-independent
* easy to add new alarm types
* independent from recompilation for rule changes

The correlation logic should NOT be hardcoded.

Topology algorithms remain in C#.

Business correlation rules are externalized into JSON files.

---

# Chosen Approach

Hybrid Rule Engine

## Topology Algorithms

Implemented in C#

Contains:

* graph traversal
* DFS/BFS
* parent lookup
* child lookup
* impact propagation
* multi-parent analysis
* chain depth calculation

These algorithms are structural and should remain hardcoded.

---

## Correlation Rules

Stored in JSON files.

Contains:

* alarm relationships
* suppression rules
* root cause conditions
* device type relationships

Rules can be changed without recompiling.

---

# Folder Structure

```text
alarm-service/

Controllers/

Services/

Repositories/

Entities/

DTOs/

Data/

Correlation/

    Engine/

        CorrelationEngine.cs

        RootCauseEngine.cs

        ImpactAnalysisEngine.cs

        RuleLoader.cs

    Models/

        CorrelationRule.cs

        CorrelationResult.cs

        CorrelationContext.cs

    Rules/

        slbn-rules.json

        cean-rules.json

        msan-rules.json

Program.cs

appsettings.json
```

---

# Alarm Tables

Separate tables are used for each device type.

```text
SLBN_Alarms

CEAN_Alarms

MSAN_Alarms
```

Additional tables:

```text
RootCauses

ImpactedDevices

CorrelatedFaults
```

---

# Topology Source

Topology information comes from topology-service.

The correlation engine must never maintain topology data itself.

Required information:

* Device
* DeviceType
* Parent devices
* Child devices
* Device links

Topology service remains the source of truth.

---

# Rule File Example

slbn-rules.json

```json
[
  {
    "RuleName": "SLBN Link Failure",

    "Enabled": true,

    "Priority": 1,

    "SourceAlarmType": "LINK_DOWN",

    "SourceDeviceType": "SLBN",

    "TargetAlarmType": "NODE_UNREACHABLE",

    "TargetDeviceType": "CEAN",

    "MarkSourceAsRootCause": true,

    "SuppressTargetAlarm": true
  }
]
```

---

# Correlation Flow

Step 1

Receive alarm.

Example:

```text
SLBN-001 LINK_DOWN
```

---

Step 2

Load topology information.

Determine:

* parents
* children

---

Step 3

Load correlation rules.

Rules are loaded from JSON files.

---

Step 4

Find matching rule.

Example:

```text
LINK_DOWN
SLBN
↓
NODE_UNREACHABLE
CEAN
```

---

Step 5

Traverse children.

Determine affected devices.

---

Step 6

Verify conditions.

Example:

All parents down?

If yes:

Child becomes impacted.

If no:

Ignore child.

---

Step 7

Create correlation result.

Root cause:

```text
SLBN-001
```

Impacted:

```text
CEAN-003

MSAN-010
```

---

Step 8

Store results.

Tables:

```text
RootCauses

ImpactedDevices

CorrelatedFaults
```

---

# Core Components

## RuleLoader

Responsibilities:

* read JSON files
* deserialize rules
* cache rules in memory

Should load once at startup.

Do not read files for every request.

---

## CorrelationEngine

Main orchestrator.

Responsibilities:

* load rules
* match rules
* invoke root cause analysis
* invoke impact analysis
* build results

---

## RootCauseEngine

Responsibilities:

* determine probable root cause
* avoid duplicate root causes
* prioritize root alarms

---

## ImpactAnalysisEngine

Responsibilities:

* traverse topology
* determine affected devices
* support recursive traversal
* support multi-level impact chains

---

# Correlation Models

## CorrelationRule

```csharp
RuleName

Enabled

Priority

SourceAlarmType

SourceDeviceType

TargetAlarmType

TargetDeviceType

MarkSourceAsRootCause

SuppressTargetAlarm
```

---

## CorrelationResult

```csharp
RootCauseDeviceId

RootCauseAlarmId

ImpactedDevices

SuppressedAlarms

CorrelationTime
```

---

# Implementation Order

## Phase 1

Create folder structure.

Create:

```text
Correlation/

Engine/

Models/

Rules/
```

---

## Phase 2

Create models.

Implement:

```text
CorrelationRule

CorrelationResult

CorrelationContext
```

---

## Phase 3

Implement RuleLoader.

Responsibilities:

* load JSON files
* deserialize
* cache rules

Verify:

Rules are successfully loaded at startup.

---

## Phase 4

Implement topology provider.

Must obtain:

```text
Device

Parent devices

Child devices
```

from topology-service.

Use REST API.

Do not duplicate topology data.

---

## Phase 5

Implement RootCauseEngine.

Goal:

Determine probable root cause.

Avoid duplicates.

---

## Phase 6

Implement ImpactAnalysisEngine.

Use DFS recursion.

Support:

* multiple levels
* recursive traversal
* cycle detection

Maintain:

```csharp
HashSet<int> visitedDevices
```

to prevent infinite loops.

---

## Phase 7

Implement CorrelationEngine.

Pipeline:

```text
Alarm

↓

Load rules

↓

Load topology

↓

RootCauseEngine

↓

ImpactAnalysisEngine

↓

CorrelationResult

↓

Persist result
```

---

# Important Checks

Always verify:

## Rule exists

No rule:

Return empty result.

Never throw exceptions.

---

## Device exists

Topology lookup failure:

Return graceful error.

---

## Parent devices exist

Handle root nodes.

---

## Child devices exist

Leaf nodes are valid.

---

## Cycle detection

Prevent:

A → B → C → A

Infinite recursion.

Must maintain:

```csharp
HashSet<int> visitedDevices
```

---

## Multi-parent devices

Example:

```text
SLBN-1
    \
     CEAN-1
    /
SLBN-2
```

If one parent is healthy:

CEAN-1 should remain UP.

Do not mark impacted unless ALL parents satisfy failure conditions.

---

## Rule priority

Sort rules by:

```text
Priority ASC
```

Apply highest priority rule first.

---

## Disabled rules

Ignore:

```json
Enabled = false
```

---

# Pitfalls

## Never hardcode alarm relationships

Bad:

```csharp
if(alarmType=="LINK_DOWN")
```

Relationship logic belongs in JSON.



## Never read JSON on every request

Load once.

Cache in memory.



## Never duplicate topology data

Topology-service remains source of truth.



## Never assume one parent

Support multi-parent relationships.



## Never trust topology to be acyclic

Implement cycle detection.



## Never use recursive traversal without visited set

Infinite recursion is possible.



## Never mix traversal and rules

Topology algorithms belong in C#.

Business rules belong in JSON.

Keep them separated.



# Future Extensions

Possible additions:

* time windows
* alarm count thresholds
* rule expressions
* vendor-specific rule files
* rule versioning
* database rule storage
* UI rule editor

These future enhancements must not require redesigning the correlation engine.

Current design must remain compatible with future improvements.
