import { useMemo, useState } from 'react'
import { Icon } from '../components/Icon'
import { PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiResource } from '../hooks/useApiResource'
import type { TopologyDevice } from '../services/networkApi'

export function InventoryPage() {
  const [query, setQuery] = useState('')
  const [deviceType, setDeviceType] = useState('all')
  const resource = useApiResource<TopologyDevice[]>('/topology/api/device')
  const devices = useMemo(() => (resource.data ?? []).filter((device) => {
    const matchesType = deviceType === 'all' || device.deviceType === deviceType
    const matchesQuery = JSON.stringify(device).toLowerCase().includes(query.toLowerCase())
    return matchesType && matchesQuery
  }), [deviceType, query, resource.data])

  return <>
    <PageHeader eyebrow="Network intelligence / Inventory" title="Device inventory" subtitle="One place for the devices that make up the service topology.">
      <button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh</button>
      <button className="primary-button" type="button" disabled title="Device mutations are not exposed in this preview">Add device</button>
    </PageHeader>
    <div className="filter-bar"><label className="search-field"><Icon name="search" /><input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search devices, vendors, areas" aria-label="Search devices" /></label><label className="select-field"><select aria-label="Filter device type" value={deviceType} onChange={(event) => setDeviceType(event.target.value)}><option value="all">All device types</option><option value="SLBN">SLBN</option><option value="CEAN">CEAN</option><option value="MSAN">MSAN</option><option value="Customer">Customer</option></select></label></div>
    <Panel title="Managed devices" description="Inventory is supplied by the topology service; no local fixture records are used."><RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && devices.length === 0} onRetry={resource.reload}><div className="table-wrap"><table className="data-table"><thead><tr><th>Device</th><th>Type</th><th>IP address</th><th>Priority level</th><th>Status</th></tr></thead><tbody>{devices.map((device) => <tr key={device.deviceId}><td>{device.deviceName}</td><td>{device.deviceType}</td><td>{device.ip || 'Unavailable'}</td><td><span className={`priority-badge priority-${device.priorityLevel.toLowerCase()}`}>{device.priorityLevel}</span></td><td><span className={`status-badge ${device.status === 'UP' ? 'healthy' : device.status === 'DOWN' || device.status === 'UNREACHABLE' ? 'critical' : device.status === 'IMPACTED' ? 'warning' : 'neutral'}`}>{device.status}</span></td></tr>)}</tbody></table></div></RequestState></Panel>
  </>
}
