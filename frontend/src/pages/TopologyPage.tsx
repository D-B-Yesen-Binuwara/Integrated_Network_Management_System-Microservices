import { useCallback } from 'react'
import { DeviceMap } from '../components/DeviceMap'
import { Icon } from '../components/Icon'
import { PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiRequest } from '../hooks/useApiRequest'
import { useApiResource } from '../hooks/useApiResource'
import { getActiveAlarms, type TopologyDevice, type TopologyLink } from '../services/networkApi'

export function TopologyPage() {
  const devices = useApiResource<TopologyDevice[]>('/topology/api/device')
  const links = useApiResource<TopologyLink[]>('/topology/api/device-link')
  const alarmRequest = useCallback((signal: AbortSignal) => getActiveAlarms('all', signal), [])
  const alarms = useApiRequest('topology-active-alarms', alarmRequest)
  const loading = devices.loading || links.loading || alarms.loading
  const error = devices.error || links.error || alarms.error
  const empty = !loading && !error && (devices.data?.length ?? 0) === 0
  const reload = () => { devices.reload(); links.reload(); alarms.reload() }

  return <>
    <PageHeader eyebrow="Network intelligence / Topology" title="Topology map" subtitle="Understand device relationships, hierarchy, and propagation paths.">
      <button className="secondary-button" type="button" onClick={reload}><Icon name="refresh" />Refresh map</button>
      <button className="primary-button" type="button" disabled title="Topology mutations are not exposed in this preview">Add link</button>
    </PageHeader>
    <div className="detail-grid"><div className="detail-card"><div className="detail-label">Live source</div><div className="detail-title">Topology service</div><p className="detail-copy">Device locations and metadata are read from the backend.</p></div><div className="detail-card"><div className="detail-label">Device graph</div><div className="detail-title">{devices.loading ? 'Loading…' : `${devices.data?.length ?? 0} records`}</div><p className="detail-copy">Nodes available to the Leaflet map.</p></div><div className="detail-card"><div className="detail-label">Relationships</div><div className="detail-title">{links.loading ? 'Loading…' : `${links.data?.length ?? 0} records`}</div><p className="detail-copy">Edges available for traversal.</p></div></div>
    <Panel title="Device locations" description="Marker shape identifies the device type. Marker color reflects active alarms or device status."><RequestState loading={loading} error={error} empty={empty} onRetry={reload}><DeviceMap devices={devices.data ?? []} links={links.data ?? []} alarms={alarms.data ?? []} /><div className="map-legend"><span><i className="legend-dot marker-up" />Healthy</span><span><i className="legend-dot marker-alarm" />Active alarm</span><span><i className="legend-dot marker-down" />Down / unreachable</span><span><i className="legend-dot marker-impacted" />Impacted</span></div></RequestState></Panel>
    <div className="topology-records"><Panel title="Topology records" description="Device IP address, priority, and derived hierarchy are supplied by the topology service."><RequestState loading={loading} error={error} empty={empty} onRetry={reload}><div className="table-wrap"><table className="data-table"><thead><tr><th>Device</th><th>Type</th><th>IP address</th><th>Region</th><th>Province</th><th>LEA</th><th>Priority</th><th>Status</th><th>Relationships</th></tr></thead><tbody>{(devices.data ?? []).map((device) => <tr key={device.deviceId}><td><strong>{device.deviceName}</strong><span className="table-subtext">Engineer ID {device.assignedEngineerId || 'Unavailable'}</span></td><td>{device.deviceType}</td><td>{device.ip || 'Unavailable'}</td><td>{device.regionCode || 'Unavailable'}</td><td>{device.provinceCode || 'Unavailable'}</td><td>{device.leaCode || 'Unavailable'}</td><td><span className={`priority-badge priority-${device.priorityLevel.toLowerCase()}`}>{device.priorityLevel}</span></td><td><span className={`status-badge ${statusTone(device.status)}`}>{device.status}</span></td><td className="muted-cell">{(links.data ?? []).filter((link) => link.parentDeviceId === device.deviceId || link.childDeviceId === device.deviceId).length}</td></tr>)}</tbody></table></div></RequestState></Panel></div>
  </>
}

function statusTone(status: string) {
  const normalized = status.toLowerCase()
  if (normalized === 'up') return 'healthy'
  if (normalized === 'down' || normalized === 'unreachable') return 'critical'
  if (normalized === 'impacted') return 'warning'
  return 'neutral'
}
