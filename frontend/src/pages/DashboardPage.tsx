import { Link } from 'react-router-dom'
import { Icon } from '../components/Icon'
import { MetricCard, PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiResource } from '../hooks/useApiResource'

type DashboardResponse = { activeAlarms?: number; devices?: number; correlatedFaults?: number; impactedDevices?: number }

export function DashboardPage() {
  const resource = useApiResource<DashboardResponse>('/api/dashboard/summary')
  const data = resource.data
  const metric = (value: number | undefined) => value === undefined ? '—' : value.toLocaleString()

  return <>
    <PageHeader eyebrow="Network operations / Today" title="Good morning, operator" subtitle="A calm view of the network starts with clear signals."><button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh view</button><Link className="primary-button" to="/alarms">Open alarm center</Link></PageHeader>
    <div className="metric-grid"><MetricCard label="Active alarms" value={resource.loading ? '…' : metric(data?.activeAlarms)} foot="Awaiting live service data" icon="alarm" tone="orange" /><MetricCard label="Network devices" value={resource.loading ? '…' : metric(data?.devices)} foot="Topology inventory" icon="device" tone="blue" /><MetricCard label="Correlated faults" value={resource.loading ? '…' : metric(data?.correlatedFaults)} foot="Root-cause candidates" icon="activity" tone="purple" /><MetricCard label="Impacted devices" value={resource.loading ? '…' : metric(data?.impactedDevices)} foot="Propagation analysis" icon="topology" /></div>
    {resource.error && <div style={{ marginBottom: 18 }}><RequestState loading={false} error={resource.error} empty={false} onRetry={resource.reload}>{null}</RequestState></div>}
    <div className="dashboard-grid"><Panel title="Network posture" description="A topology-aware view will appear when the topology service is connected." action={<Link className="panel-link" to="/topology">Explore topology <Icon name="chevron" /></Link>}><div className="network-preview"><div className="network-copy"><div className="network-copy-inner"><div className="network-orb"><Icon name="topology" /></div><h3>Live topology canvas</h3><p>Connect the gateway to render device health, links, and propagation paths here.</p></div></div></div></Panel><Panel title="Recent activity" description="Live events from alarms and correlation services."><div className="feed-list"><div className="feed-row"><span className="feed-dot" /><div className="feed-content"><div className="feed-title">Waiting for event stream</div><div className="feed-meta">No activity fetched</div></div></div><div className="feed-row"><span className="feed-dot" /><div className="feed-content"><div className="feed-title">Correlation updates will appear here</div><div className="feed-meta">SignalR-ready surface</div></div></div><div className="feed-row"><span className="feed-dot" /><div className="feed-content"><div className="feed-title">External alarm ingestion</div><div className="feed-meta">No records fetched</div></div></div></div></Panel></div>
  </>
}
