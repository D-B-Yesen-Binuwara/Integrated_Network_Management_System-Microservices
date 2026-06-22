# INMS – Alarm Correlation & Fault Localization Flow (Microservices)

## 1) Core idea (telco NOC workflow)
The platform **does not generate alarms internally**. External systems produce alarms, and the INMS stack:
1. **Ingests** alarms (per device type)
2. **Correlates** them using topology intelligence + correlation rules to find probable root causes
3. **Localizes faults** by traversing the topology (device links / hierarchy) to identify impacted downstream/upstream devices
4. Produces results for visualization + summaries

The intended microservice separation from `README.md`:
- **Alarm & Correlation Service**: ingestion → correlation → root-cause + impacted-device results
- **Topology Service**: owns network inventory (Region/Province/LEA/Devices/Links)
- **AI & Analytics Service**: event logging + summarization
- **Identity Service**: access control (later via JWT/auth)
- **Gateway**: routing

---

## 2) Data model assumption: separate alarm tables per device type
You stated:
> “alarms will be separate tables for each device type to store the alarms.”

Typical tables/collections (examples):
- `SLBN_Alarms` (for SLBN device type)
- `CEAN_Alarms` (for CEAN device type)
- `MSAN_Alarms` (for MSAN device type)

Why this matters for correlation:
- Alarm payloads/fields can differ by device type.
- Correlation rules can be specialized per device-type combination.

Implementation approach:
- The Alarm & Correlation service exposes a unified ingestion endpoint but writes to the appropriate table/collection based on `deviceType`.
- Correlation reads from these tables using time windows and correlation keys.

---

## 3) Topology inputs required for correlation & localization
To correlate and localize, the correlation engine needs:
1. **Device inventory**
2. **Device links** (graph edges)
3. **Hierarchy** (Region → Province → LEA; also device placement if applicable)

Topology Service owns:
- Regions/Provinces/LEA entities
- Devices (SLBN/CEAN/MSAN)
- Device links (relationships)

Correlation engine uses these links to:
- traverse upstream/downstream
- infer likely propagation paths
- map alarms on one device type to related devices in the graph

---

## 4) Step-by-step correlation + fault localization flow

### Step A — Alarm ingestion (per device type)
**Trigger:** external system sends an alarm event.

**Alarm & Correlation Service responsibilities:**
1. Determine `deviceType` (SLBN/CEAN/MSAN)
2. Normalize/validate payload
3. Store it in the corresponding alarm table/collection
4. Emit a domain event internally (or publish to a queue) like:
   - `AlarmReceived` (includes alarmId, deviceId, deviceType, timestamp, area context, severity)

Key design point:
- Store first (persistence) then correlate.

---

### Step B — Correlation windowing (select relevant alarms)
**Goal:** correlation should not be “alarm-by-alarm only”. It typically uses a time window.

**Mechanism:**
- For each new alarm, find related alarms within a configurable window, e.g. last N minutes.
- Optionally, deduplicate by correlation keys:
  - `(rootCandidateType, areaId, timeBucket, alarmTypeSet)`

Selection process:
- Query across the relevant alarm tables:
  - Read `SLBN_Alarms` + `CEAN_Alarms` + `MSAN_Alarms`
  - Filter by `area` (region/province/LEA) and time range

---

### Step C — Correlation rules (graph + rule-based inference)
**Goal:** infer probable root causes from sets/patterns of alarms.

**Inputs to rules:**
1. The alarm set within the time window
2. Topology context (device links / upstream/downstream relationships)
3. Optional metadata:
   - severity
   - alarm timestamps alignment
   - area placement

**How rules are applied (typical patterns):**
- Pattern 1: If device X is DOWN/unreachable AND its neighbors Y are also DOWN/unreachable within window → candidate root is the highest parent in topology.
- Pattern 2: If CEAN_POWER_FAILURE + MSAN_UNREACHABLE shortly after → probable root cause is CEAN power event.
- Pattern 3: Suppress duplicates (same alarm type on adjacent devices with same timestamp bucket)

**Output:**
- Create a `CorrelatedFault` (root-cause candidate) object
- Include:
  - `rootCauseDeviceId`
  - `rootCauseType`
  - `confidence/score`
  - `supportingAlarms[]`
  - `area context`

---

### Step D — Fault localization (impacted devices / cascade)
**Goal:** given the root cause device (or candidate), traverse the topology to find impacted downstream/upstream devices.

How to localize using device links:
1. Load neighbors/edges for the root-cause device from Topology Service
2. Traverse:
   - **Downstream traversal**: follow outgoing links to impacted devices
   - **Upstream traversal**: follow incoming links for likely upstream influence
3. Stop conditions:
   - time boundary
   - device status constraints (only include devices that have relevant alarm evidence)
   - depth limit (avoid graph explosion)

**Output:**
- Store `ImpactedDevice` records or results:
  - `impactedDeviceId`
  - `relationPath` (optional)
  - `impactType` (downstream/upstream)
  - `evidenceAlarms[]`

---

### Step E — Persistence + publication of results
Persist correlation results so other services can query:
- `CorrelatedFault`
- `RootCause`
- `ImpactedDevice`

Then publish events like:
- `FaultCorrelated`
- `FaultLocalized`

These events power:
- realtime visualization (SignalR)
- analytics summaries

---

## 5) Microservice communication: event handler vs event bus/queue

### Option 1 — Direct “event handler” inside the service (simple, synchronous)
Flow:
- Alarm ingested → handler runs immediately → correlation + localization → store results.

Pros:
- easiest to implement and debug
- fewer moving parts

Cons:
- correlation becomes heavy and can slow ingestion
- spikes in alarms can overload the service

Use this if:
- early MVP
- low alarm volume

### Option 2 — Event bus / queue (recommended for scale + resilience)
Flow:
1. Ingestion persists alarm
2. Publish `AlarmReceived` to a queue/topic
3. A separate correlation worker/consumer processes events
4. Correlation writes results and publishes `FaultCorrelated`

Pros:
- decouples ingestion from correlation
- supports retries + backpressure
- enables parallel processing

Cons:
- extra infrastructure (RabbitMQ/Azure Service Bus/etc.)

Given `README.md` says “RabbitMQ (future integration)”, a practical strategy is:
- **MVP now:** implement internal handler (Option 1)
- **Soon:** replace the internal handler with a queue producer/consumer (Option 2)

### Best practical approach for your stage
Implement an **outbox-like pattern mentally**, even if you start simple:
- ensure ingestion stores the alarm first
- run correlation in a separate method that can later be called by a queue consumer

This keeps the code structured so moving to a real queue later is not a rewrite.

---

## 6) Suggested implementation structure (practical)
Within **Alarm & Correlation Service**:
- `IngestionController` (or endpoint)
- `AlarmStore` (writes to the correct alarm table)
- `CorrelationEngine`
  - `LoadAlarms(timeWindow, area)`
  - `ApplyCorrelationRules(alarmSet, topologyContext)`
  - `ScoreAndSelectRootCause()`
- `FaultLocalizationEngine`
  - `TraverseTopology(rootCauseDeviceId, direction, constraints)`
- `ResultStore` (persist correlated faults + impacted devices)
- `EventPublisher` abstraction
  - initially calls handlers directly
  - later can publish to RabbitMQ

Topology access:
- call Topology Service APIs OR maintain cached topology snapshots.

---

## 7) What to decide next (so you can implement consistently)
1. What is the **time window** for correlation?
2. What are the **correlation rule inputs** (alarm types + thresholds)?
3. How will you represent a topology traversal direction (upstream/downstream)?
4. Do you want correlation to be:
   - triggered per alarm
   - triggered per time bucket
   - triggered on a batch scheduler

---

## 8) Minimal MVP loop
To validate end-to-end quickly:
1. Ingest alarm → store in device-type table
2. Trigger correlation for that alarm’s time window
3. Produce a single root-cause candidate
4. Traverse topology to output impacted device list
5. Persist results for UI/analytics

Once validated, you can add:
- duplicate suppression
- confidence scoring improvements
- suppression/silencing rules
- queue-based async processing

