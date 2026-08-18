import { useState } from 'react'
import { Icon } from '../components/Icon'
import { PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiResource } from '../hooks/useApiResource'

type Device = { id?: number | string; name?: string; deviceType?: string; status?: string; vendor?: string; region?: string }

export function InventoryPage() {
  const [query, setQuery] = useState('')
  const resource = useApiResource<Device[]>('/api/device')
  const devices = (resource.data ?? []).filter((device) => JSON.stringify(device).toLowerCase().includes(query.toLowerCase()))
  return <>
    <PageHeader eyebrow="Network intelligence / Inventory" title="Device inventory" subtitle="One place for the devices that make up the service topology."><button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh</button><button className="primary-button" type="button">Add device</button></PageHeader>
    <div className="filter-bar"><label className="search-field"><Icon name="search" /><input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search devices, vendors, areas" aria-label="Search devices" /></label><label className="select-field"><select aria-label="Filter device type" defaultValue="all"><option value="all">All device types</option><option value="SLBN">SLBN</option><option value="CEAN">CEAN</option><option value="MSAN">MSAN</option></select></label><button className="secondary-button" type="button"><Icon name="download" />Export</button></div>
    <Panel title="Managed devices" description="Inventory is service-backed and intentionally contains no local fixture records."><RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && devices.length === 0} onRetry={resource.reload}><div className="table-wrap"><table className="data-table"><thead><tr><th>Device</th><th>Type</th><th>Vendor</th><th>Area</th><th>Status</th></tr></thead><tbody>{devices.map((device, index) => <tr key={String(device.id ?? index)}><td>{device.name ?? device.id ?? '—'}</td><td>{device.deviceType ?? '—'}</td><td>{device.vendor ?? '—'}</td><td>{device.region ?? '—'}</td><td><span className={`status-badge ${device.status?.toLowerCase() === 'up' || device.status?.toLowerCase() === 'active' ? 'healthy' : 'neutral'}`}>{device.status ?? 'Unknown'}</span></td></tr>)}</tbody></table></div></RequestState></Panel>
  </>
}
