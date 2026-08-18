import { Icon } from '../components/Icon'
import { BackendNotice, PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiResource } from '../hooks/useApiResource'

type Device = { id?: number | string; name?: string; deviceType?: string; status?: string; region?: string }
type LinkRecord = { id?: number | string; sourceDeviceId?: number | string; targetDeviceId?: number | string; status?: string }

export function TopologyPage() {
  const devices = useApiResource<Device[]>('/api/device')
  const links = useApiResource<LinkRecord[]>('/api/device-link')
  const loading = devices.loading || links.loading
  const error = devices.error || links.error
  const empty = !loading && !error && (devices.data?.length ?? 0) === 0 && (links.data?.length ?? 0) === 0
  return <>
    <PageHeader eyebrow="Network intelligence / Topology" title="Topology map" subtitle="Understand device relationships, hierarchy, and propagation paths."><button className="secondary-button" type="button" onClick={() => { devices.reload(); links.reload() }}><Icon name="refresh" />Refresh map</button><button className="primary-button" type="button">Add link</button></PageHeader>
    <div className="detail-grid"><BackendNotice copy="The interactive graph will be populated from the topology service device and device-link resources." /><div className="detail-card"><div className="detail-label">Device graph</div><div className="detail-title">{devices.loading ? 'Loading…' : devices.data ? `${devices.data.length} records` : '—'}</div><p className="detail-copy">Nodes available to the topology canvas.</p></div><div className="detail-card"><div className="detail-label">Relationships</div><div className="detail-title">{links.loading ? 'Loading…' : links.data ? `${links.data.length} records` : '—'}</div><p className="detail-copy">Edges available for traversal.</p></div></div>
    <Panel title="Topology canvas" description="A graph renderer will use these service-backed nodes and links."><div className="network-preview"><div className="network-copy"><div className="network-copy-inner"><div className="network-orb"><Icon name="topology" /></div><h3>Topology data required</h3><p>Nothing is drawn until the topology service returns device and relationship records.</p></div></div></div></Panel>
    <div style={{ marginTop: 18 }}><Panel title="Topology records" description="A table fallback keeps the data inspectable without a graph renderer."><RequestState loading={loading} error={error} empty={empty} onRetry={() => { devices.reload(); links.reload() }}><div className="table-wrap"><table className="data-table"><thead><tr><th>Device</th><th>Type</th><th>Area</th><th>Status</th><th>Relationships</th></tr></thead><tbody>{(devices.data ?? []).map((device, index) => <tr key={String(device.id ?? index)}><td>{device.name ?? device.id ?? '—'}</td><td>{device.deviceType ?? '—'}</td><td>{device.region ?? '—'}</td><td><span className="status-badge neutral">{device.status ?? 'Unknown'}</span></td><td className="muted-cell">{(links.data ?? []).filter((link) => link.sourceDeviceId === device.id || link.targetDeviceId === device.id).length}</td></tr>)}</tbody></table></div></RequestState></Panel></div>
  </>
}
