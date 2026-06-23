# Corelation (Alarm Correlation) Engine - Design Notes

> Note: The repository currently has a split alarm domain for **CEA**, **MSAN**, and **SLBN** (controllers + services per domain). There is currently no correlation engine implementation in the alarm-service code shown so far; this document proposes how to add one in a way that supports **dynamic rule sets**.

## 1) What “correlation engine” should do

When a new alarm is created (e.g., `POST api/cea-alarms`), the system should:

1. **Ingest** the alarm event (type, deviceId, raisedTime/clearedTime, extra attributes).
2. **Evaluate correlation rules** against the current alarm context (e.g., existing active alarms for the same device and/or hierarchy).
3. **Produce outputs**, such as:
   - Create a correlated alarm (a derived/aggregate alarm)
   - Clear/resolve correlated alarms based on new input
   - Mark the alarm as having a correlated root cause
   - Emit events to other services

A minimal v1 output might just create an extra record/table like `CorrelatedAlarm`.

## 2) Can rules be dynamic?
Yes—this is strongly recommended.

Dynamic rules means you can change correlation behavior **without redeploying** the service.

You have three practical approaches (you can even combine them):

### Option A — File-based DSL / JSON rule definitions (importable)
- Keep correlation rules in a JSON/YAML file (or multiple files) under the service.
- Parse/compile them at startup.
- Optionally watch the file for changes and hot-reload.

**Pros**
- Simple to implement.
- Version control via Git.
- Easy for rule authors to review changes.

**Cons**
- Updating rules in production requires distributing files (or mounting volumes).
- Multiple deployments may drift if not managed.

### Option B — Database-driven rules (UI + CRUD endpoints)
- Store correlation rules in DB tables.
- Expose endpoints to manage rules (create/update/enable/disable).
- Cache rules in memory and refresh on change.

**Pros**
- Highest flexibility.
- Rule updates become a UI workflow.
- Auditability: who changed what and when.

**Cons**
- More backend work.
- Need a rule evaluation model that is safe and predictable.

### Option C — UI-managed rules that compile to an internal expression engine
- UI edits rules using a controlled builder.
- Backend stores compiled representation (or stores JSON + compile on load).

**Pros**
- Prevents invalid rules.
- You can validate early.

**Cons**
- Requires building the UI rule editor or at least a schema-driven form.

## 3) Recommended architecture (hybrid)
Start with **Option A** for rapid iteration, then move to **Option B** for long-term maintainability.

At design time, make the correlation engine consume rules from an `IRuleProvider` interface:

- `FileRuleProvider` (reads JSON/YAML)
- `DbRuleProvider` (reads from DB)

The evaluator stays the same.

## 4) Proposed rule model (dynamic + explainable)
Correlation rules can be modeled as:

- **Scope**: what alarm types are relevant, and what entity level is correlated (device, region, vendor, etc.).
- **Conditions**: boolean logic over facts.
- **Window**: time window (e.g., “within 10 minutes”).
- **Actions**: what to do when matched.
- **Priority**: order of evaluation.

### Example rule (JSON DSL concept)

```json
{
  "ruleId": "CEA-001",
  "name": "Link Down -> Performance Degraded",
  "enabled": true,
  "priority": 10,
  "scope": { "level": "device", "deviceIdFromEvent": true },
  "window": { "type": "raisedTime", "minutes": 10 },
  "trigger": {
    "onAlarmType": "LINK_DOWN"
  },
  "conditions": {
    "all": [
      { "fact": "activeAlarmExists", "alarmType": "PERF_DEGRADED" },
      { "fact": "alarmCountWithinWindow", "alarmType": "CRC_ERROR", "op": ">=", "value": 3 }
    ]
  },
  "actions": [
    { "type": "createCorrelatedAlarm", "correlatedAlarmType": "ROOT_CAUSE_LINK" }
  ]
}
```

You can extend this DSL later.

## 5) “Correlation/equation” vs “rule evaluation”
There are two common ways people implement correlation:

### A) Equation-style (math)
- Example: compute a score from multiple alarms.
- If score > threshold, create correlated alarm.

Pros: simple numeric tuning.
Cons: less explainable (“why did this match?”).

### B) Logic-rule style (recommended)
- Example: AND/OR conditions over alarm presence/count/state.
- If condition graph matches, fire actions.

Pros: explainable + supports rule toggling.
Cons: slightly more complex evaluation than pure math.

A combined approach works well:
- Use logic rules to decide applicability
- Inside an action, compute a score if needed

## 6) Where to hook into alarm-service

You typically evaluate correlation **when an alarm is created**:

- In each domain service (`CEAAlarmService`, `MSANAlarmService`, `SLBNAlarmService`) inside `CreateAsync`:
  1. After saving the alarm entity (so it exists in the DB)
  2. Call `ICorrelationEngine.EvaluateAsync(newAlarm)`
  3. Persist outputs (correlated alarms) using correlation repositories

If correlation needs “active alarms” to exist, ensure evaluation is after persistence.

## 7) Rule evaluation dependencies (what facts you may need)
Define a `CorrelationContext` passed to evaluation:

- The newly created alarm
- DeviceId / hierarchy identifiers
- Query accessors (e.g., `IAlarmFactsProvider`)
  - active alarm exists by type
  - count of alarm types within a time window
  - latest alarm timestamp by type

This makes evaluation testable and keeps it independent of EF queries.

## 8) Data needed for outputs (correlated alarms)
You’ll likely need new entities and tables:

- `CorrelatedAlarm`
  - CorrelatedAlarmId
  - DeviceId
  - CorrelatedAlarmType
  - RootCauseAlarmIds (optional)
  - CreatedAt
  - IsActive
  - CorrelationRuleId

Even if you postpone DB schema, the interface should be ready.

## 9) Dynamic updates strategy
To support “rules can be changed when need”:

### UI/endpoints workflow (Option B)
1. UI calls `POST /api/correlation-rules` to add/update rules
2. Backend validates and stores them
3. Backend updates in-memory cache
4. Alarm-service continues using latest rules

### Caching + reload
- Load rules into an in-memory cache at startup.
- Refresh cache when:
  - a rule is modified (event-based), or
  - TTL expires (polling), or
  - a DB “version” changes.

This avoids hitting DB on every alarm event.

## 10) Safety and validation
Dynamic rules must not allow arbitrary code execution.

- Avoid embedding executable expressions (e.g., C# eval).
- Use a controlled DSL (JSON/YAML) with a whitelist of operations.
- Validate:
  - rule schema
  - supported alarm types
  - time windows
  - action parameters

## 11) Implementation steps (practical sequence)

### Step 1 — Define core interfaces
- `ICorrelationEngine`
- `IRuleProvider`
- `IRuleEvaluator` (optional)
- `IAlarmFactsProvider`

### Step 2 — Implement a v1 engine using file-based rules
- Create rule parser/loader for JSON.
- Evaluate a small subset of facts:
  - active alarm exists
  - count within window

### Step 3 — Create endpoints for correlation rule management (optional for v1)
- If UI is planned: add CRUD endpoints for rules.

### Step 4 — Persist correlated alarms
- Add EF entities + migrations.
- Implement `ICorrelatedAlarmRepository`.

### Step 5 — Hook evaluation into each CreateAsync
- After alarm is created, call correlation engine.

### Step 6 — Add rule caching and reload
- Hot reload for file-based rules.
- Or DB caching for DB-driven rules.

### Step 7 — Add “explainability”
- Store which conditions matched and which alarms contributed.
- This makes debugging rule changes much easier.

## 12) Where to place this in the project
Inside `Backend/services/alarm-service/` create folders like:

- `Correlation/`
  - `Core/` (interfaces, DTOs)
  - `Evaluation/` (engine)
  - `Rules/` (DSL model)
  - `Providers/` (file/db providers)

And optionally:
- `Controllers/CorrelationRulesController.cs`
- `Entities/CorrelatedAlarm.cs`
- `Repositories/CorrelatedAlarmRepository.cs`

## 13) Final recommendation
Dynamic correlation is absolutely possible and best implemented via a **controlled DSL** plus a rule provider that can switch between **file** and **DB**.

This yields:
- rule changes without redeploy
- predictable evaluation
- easier debugging + audits

---
If you later share the exact alarm attribute model you want to correlate on (alarm type enums/strings, device hierarchy, additional fields), the DSL schema and supported “facts” can be refined to match your real data model.
