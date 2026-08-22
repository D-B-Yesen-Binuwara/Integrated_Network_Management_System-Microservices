import { fetchJson, requestJson } from './api'

const routes = {
  alarm: '/alarm/api',
  topology: '/topology/api',
  identity: '/identity/api',
} as const

export type AlarmSource = 'all' | 'SLBN' | 'CEAN' | 'MSAN'

export type AlarmRecord = {
  id: number
  source: Exclude<AlarmSource, 'all'>
  deviceId: number
  alarmType: string
  raisedTime: string
  clearedTime: string | null
  isActive: boolean
  regionCode: string | null
  provinceCode: string | null
  leaCode: string | null
}

export type EnrichedAlarmRecord = AlarmRecord & {
  deviceName: string
  ip: string
  priorityLevel: string
  deviceType: string
  deviceStatus: string
  regionCode: string
  provinceCode: string
  leaCode: string
}

type AlarmDto = {
  slbnAlarmId?: number
  ceaAlarmId?: number
  msanAlarmId?: number
  deviceId: number
  alarmType: string
  raisedTime: string
  clearedTime: string | null
  isActive: boolean
  regionCode?: string | null
  provinceCode?: string | null
  leaCode?: string | null
}

export type TopologyDevice = {
  deviceId: number
  deviceName: string
  deviceType: string
  ip: string
  regionCode: string
  provinceCode: string
  leaCode: string
  regionName: string
  provinceName: string
  leaName: string
  assignedEngineerId: number
  status: string
  priorityLevel: string
  latitude: number
  longitude: number
}

export type TopologyLink = {
  linkId: number
  parentDeviceId: number
  childDeviceId: number
  linkStatus: string
}

export type RegionRecord = {
  regionId: number
  regionCode: string
  name: string
  description: string | null
}

export type ProvinceRecord = {
  provinceId: number
  provinceCode: string
  name: string
  regionId: number
  region?: RegionRecord | null
}

export type LeaRecord = {
  leaId: number
  leaCode: string
  name: string
  provinceId: number
  province?: ProvinceRecord | null
}

export type EmployeeRecord = {
  userId: number
  username: string
  fullName: string
  roleId: number
  roleName: string | null
  serviceId: string | null
  email: string | null
  region: string | null
  province: string | null
  lea: string | null
  regionCode: string | null
  provinceCode: string | null
  leaCode: string | null
}

export type RoleRecord = {
  roleId: number
  roleName: string
  description: string | null
}

export type AccountRequestRecord = {
  requestId: number
  fullName: string
  email: string
  serviceId: string
  roleId: number
  regionCode: string | null
  provinceCode: string | null
  leaCode: string | null
  requestedAt: string
  status: string
}

export type CorrelationFault = {
  correlatedFaultId: number
  rootCauseId: number
  correlationRuleName: string
  sourceDeviceId: number
  sourceDeviceType: string
  sourceAlarmId: number
  sourceAlarmType: string
  startedAt: string
  endedAt: string | null
  status: string
  confidenceScore: number
  suppressedAlarms: Array<{ alarmId: number; deviceId: number; deviceType: string; alarmType: string; raisedTime: string }>
}

export type DashboardSnapshot = {
  activeAlarms: number
  devices: number
  correlatedFaults: number
  impactedDevices: number
  recentAlarms: AlarmRecord[]
}

export type AnalyticsSnapshot = {
  generatedAt: string
  activeAlarms: AlarmRecord[]
  faults: CorrelationFault[]
}

const alarmPaths: Record<Exclude<AlarmSource, 'all'>, string> = {
  SLBN: `${routes.alarm}/slbn-alarms/active`,
  CEAN: `${routes.alarm}/cea-alarms/active`,
  MSAN: `${routes.alarm}/msan-alarms/active`,
}

export async function getActiveAlarms(source: AlarmSource, signal?: AbortSignal): Promise<AlarmRecord[]> {
  if (source === 'all') {
    const [slbn, cean, msan] = await Promise.all([
      getActiveAlarms('SLBN', signal),
      getActiveAlarms('CEAN', signal),
      getActiveAlarms('MSAN', signal),
    ])
    return [...slbn, ...cean, ...msan].sort((left, right) => right.raisedTime.localeCompare(left.raisedTime))
  }

  const rows = await fetchJson<AlarmDto[]>(alarmPaths[source], signal)
  return rows.map((row) => normalizeAlarm(row, source))
}

/** Joins alarm records with topology metadata for operational tables and map popups. */
export async function getActiveAlarmsWithDeviceDetails(source: AlarmSource, signal?: AbortSignal): Promise<EnrichedAlarmRecord[]> {
  const [alarms, devices] = await Promise.all([
    getActiveAlarms(source, signal),
    getTopologyDevices(signal),
  ])
  const devicesById = new Map(devices.map((device) => [device.deviceId, device]))

  return alarms.map((alarm) => {
    const device = devicesById.get(alarm.deviceId)
    return {
      ...alarm,
      deviceName: device?.deviceName ?? `Device ${alarm.deviceId}`,
      ip: device?.ip ?? 'Unavailable',
      priorityLevel: device?.priorityLevel ?? 'Unknown',
      deviceType: device?.deviceType ?? 'Unknown',
      deviceStatus: device?.status ?? 'Unknown',
      regionCode: device?.regionCode ?? alarm.regionCode ?? 'Unavailable',
      provinceCode: device?.provinceCode ?? alarm.provinceCode ?? 'Unavailable',
      leaCode: device?.leaCode ?? alarm.leaCode ?? 'Unavailable',
    }
  })
}

export function getTopologyDevices(signal?: AbortSignal) {
  return fetchJson<TopologyDevice[]>(`${routes.topology}/device`, signal)
}

export function getTopologyLinks(signal?: AbortSignal) {
  return fetchJson<TopologyLink[]>(`${routes.topology}/device-link`, signal)
}

export function getRegions(signal?: AbortSignal) {
  return fetchJson<RegionRecord[]>(`${routes.topology}/region`, signal)
}

export function getProvinces(signal?: AbortSignal) {
  return fetchJson<ProvinceRecord[]>(`${routes.topology}/province`, signal)
}

export function getLeas(signal?: AbortSignal) {
  return fetchJson<LeaRecord[]>(`${routes.topology}/lea`, signal)
}

export function getEmployees(signal?: AbortSignal) {
  return fetchJson<EmployeeRecord[]>(`${routes.identity}/user`, signal)
}

export function getRoles(signal?: AbortSignal) {
  return fetchJson<RoleRecord[]>(`${routes.identity}/role`, signal)
}

export function getAccountRequests(signal?: AbortSignal) {
  return fetchJson<AccountRequestRecord[]>(`${routes.identity}/accountrequest`, signal)
}

export function updateAccountRequestStatus(requestId: number, status: 'APPROVED' | 'REJECTED', signal?: AbortSignal) {
  return requestJson<void>(`${routes.identity}/accountrequest/${requestId}/status`, 'PATCH', { status }, signal)
}

export function createDevice(payload: {
  deviceName: string
  deviceType: string
  ip: string
  priorityLevel: string
  latitude: number
  longitude: number
  leaCode: string
  assignedEngineerId: number
}, signal?: AbortSignal) {
  return requestJson<TopologyDevice>(`${routes.topology}/device`, 'POST', payload, signal)
}

export function getCorrelationFaults(signal?: AbortSignal) {
  return fetchJson<CorrelationFault[]>(`${routes.alarm}/correlation/faults`, signal)
}

export function getImpactedDevices(signal?: AbortSignal) {
  return fetchJson<Array<{ impactedDeviceId: number; rootCauseId: number; deviceId: number; deviceType: string; impactType: string; createdAt: string }>>(`${routes.alarm}/correlation/impacted-devices`, signal)
}

/** Composes real responses from the gateway into the dashboard's view model. */
export async function getDashboardSnapshot(signal?: AbortSignal): Promise<DashboardSnapshot> {
  const [activeAlarms, devices, faults, impactedDevices] = await Promise.all([
    getActiveAlarms('all', signal),
    getTopologyDevices(signal),
    getCorrelationFaults(signal),
    getImpactedDevices(signal),
  ])

  return {
    activeAlarms: activeAlarms.length,
    devices: devices.length,
    correlatedFaults: faults.length,
    impactedDevices: impactedDevices.length,
    recentAlarms: activeAlarms.slice(0, 3),
  }
}

/** Analytics currently uses alarm/correlation services because the AI service is not implemented yet. */
export async function getAnalyticsSnapshot(signal?: AbortSignal): Promise<AnalyticsSnapshot> {
  const [activeAlarms, faults] = await Promise.all([
    getActiveAlarms('all', signal),
    getCorrelationFaults(signal),
  ])

  return { generatedAt: new Date().toISOString(), activeAlarms, faults }
}

function normalizeAlarm(row: AlarmDto, source: Exclude<AlarmSource, 'all'>): AlarmRecord {
  const id = source === 'SLBN' ? row.slbnAlarmId : source === 'CEAN' ? row.ceaAlarmId : row.msanAlarmId
  if (id === undefined) throw new Error(`The ${source} alarm response did not include an alarm ID.`)

  return {
    id,
    source,
    deviceId: row.deviceId,
    alarmType: row.alarmType,
    raisedTime: row.raisedTime,
    clearedTime: row.clearedTime,
    isActive: row.isActive,
    regionCode: row.regionCode ?? null,
    provinceCode: row.provinceCode ?? null,
    leaCode: row.leaCode ?? null,
  }
}
