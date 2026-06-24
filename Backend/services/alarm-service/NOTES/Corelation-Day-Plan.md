# Correlation Engine Implementation Plan

## Objective

Implement a JSON-driven alarm correlation engine inside the `alarm-service`.

The implementation should be completed incrementally, where each day produces a usable and testable result.

---

# Day 1 — Foundation and Rule Loading

## Goal

Create the correlation module structure and successfully load rules from JSON files.

## Deliverables

Working folder structure:

```text
Correlation/

    Engine/

    Models/

    Rules/
```

Create:

### Models

* CorrelationRule.cs
* CorrelationResult.cs
* CorrelationContext.cs

### Engine

* RuleLoader.cs

### Rule Files

* slbn-rules.json
* cean-rules.json
* msan-rules.json

---

## Tasks

### Create model classes

Implement:

```csharp
CorrelationRule

CorrelationResult

CorrelationContext
```

---

### Create RuleLoader

Responsibilities:

* read JSON files
* deserialize rules
* cache rules

---

### Load rules during startup

Verify:

Rules are successfully loaded.

---

## Test

Print:

```text
SLBN Rules Loaded : X

CEAN Rules Loaded : X

MSAN Rules Loaded : X
```

---

## End Result

A functioning rule-loading subsystem.

Nothing else should be attempted on Day 1.

---

# Day 2 — Root Cause Engine

## Goal

Determine probable root causes from alarm data.

No impact propagation yet.

---

## Deliverables

Working:

```text
RootCauseEngine
```

that returns:

```text
Root Cause Device

Root Cause Alarm
```

---

## Tasks

Create:

```text
Engine/

    RootCauseEngine.cs
```

Implement:

### Rule matching

Find matching rule.

### Priority handling

Sort by:

```text
Priority ASC
```

### Disabled rules

Ignore:

```text
Enabled = false
```

### Root cause creation

Populate:

```csharp
CorrelationResult.RootCauseDeviceId
```

---

## Test Cases

### LINK_DOWN alarm

Should return root cause.

---

### Unknown alarm type

Should return empty result.

---

### Disabled rule

Should return no match.

---

## End Result

Root cause identification works.

Impact analysis not implemented yet.

---

# Day 3 — Topology Traversal and Impact Analysis

## Goal

Determine impacted devices.

---

## Deliverables

Working:

```text
ImpactAnalysisEngine
```

that recursively traverses topology.

---

## Tasks

Create:

```text
ImpactAnalysisEngine.cs
```

Implement:

### DFS traversal

Recursive traversal.

---

### Cycle detection

Maintain:

```csharp
HashSet<int> visitedDevices
```

Prevent:

```text
A → B → C → A
```

---

### Leaf detection

Leaf devices terminate recursion.

---

### Populate

```csharp
CorrelationResult.ImpactedDevices
```

---

## Test Cases

Topology:

```text
SLBN-1

↓

CEAN-1

↓

MSAN-1
```

Result:

```text
Root Cause

SLBN-1

Impacted

CEAN-1

MSAN-1
```

---

## End Result

Impact propagation works.

---

# Day 4 — Multi-Parent Logic and Correlation Engine

## Goal

Complete the actual correlation engine.

---

## Deliverables

Working:

```text
CorrelationEngine
```

which orchestrates everything.

---

## Tasks

Create:

```text
CorrelationEngine.cs
```

Pipeline:

```text
Alarm

↓

Load Rule

↓

RootCauseEngine

↓

ImpactAnalysisEngine

↓

CorrelationResult
```

---

### Multi-parent support

Example:

```text
SLBN-1
     \
      CEAN-1
     /
SLBN-2
```

Rules:

If one parent remains healthy:

Do NOT mark CEAN-1 as impacted.

Only mark impacted if:

```text
ALL parents satisfy failure conditions
```

---

### Suppressed alarms

Populate:

```csharp
SuppressedAlarms
```

---

## Test Cases

Single parent topology.

Multi-parent topology.

Unknown alarms.

Disabled rules.

---

## End Result

Complete correlation engine working in memory.

---

# Day 5 — Persistence and API Integration

## Goal

Integrate with the service.

Persist results.

Expose APIs.

---

## Deliverables

End-to-end correlation flow.

---

## Tasks

Persist:

### RootCauses table

### ImpactedDevices table

### CorrelatedFaults table

---

Create APIs:

```text
POST /correlate

GET /root-causes

GET /impacted-devices

GET /correlated-faults
```

---

## Integration

Pipeline:

```text
Alarm

↓

CorrelationEngine

↓

CorrelationResult

↓

Database

↓

API Response
```

---

## Test Cases

Insert alarm.

Call:

```text
POST /correlate
```

Verify:

Root cause stored.

Impacted devices stored.

Correlation result returned.

---

## End Result

Fully functioning JSON-driven alarm correlation engine.

---

# Important Rules

Never:

### Hardcode alarm relationships

Bad:

```csharp
if(alarmType=="LINK_DOWN")
```

Relationships belong in JSON.

---

Never:

### Read JSON every request

Load once.

Cache in memory.

---

Never:

### Duplicate topology data

Topology-service remains source of truth.

---

Always:

### Maintain visited set

Prevent infinite recursion.

---

Always:

### Support multi-parent devices

Do not assume one parent.

---

Always:

### Sort rules by priority

Priority ASC.

---

Always:

### Ignore disabled rules

Enabled=false means skip.

---

# Final Result After Day 5

Working features:

✔ JSON-based rules

✔ Root cause detection

✔ Impact propagation

✔ Multi-parent support

✔ Cycle detection

✔ Recursive traversal

✔ Rule priorities

✔ Suppressed alarms

✔ Database persistence

✔ REST APIs

✔ Extensible architecture

No redesign should be required for future enhancements.
