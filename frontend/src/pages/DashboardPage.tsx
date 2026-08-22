import { Link } from 'react-router-dom'
import { Icon } from '../components/Icon'
import { MetricCard, PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiRequest } from '../hooks/useApiRequest'
import { getDashboardSnapshot } from '../services/networkApi'

export function DashboardPage() {
  const resource = useApiRequest('dashboard-snapshot', getDashboardSnapshot)
  const data = resource.data
  const metric = (value: number | undefined) => value === undefined ? '—' : value.toLocaleString()

  return <>
    <PageHeader eyebrow="Network operations / Today" title="Good morning, operator" subtitle="A calm view of the network starts with clear signals.">
      <button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh view</button>
      <Link className="primary-button" to="/alarms">Open alarm center</Link>
    </PageHeader>

    <div className="metric-grid">
      <MetricCard label="Active alarms" value={resource.loading ? '…' : metric(data?.activeAlarms)} foot="From SLBN, CEAN and MSAN" icon="alarm" tone="orange" />
      <MetricCard label="Network devices" value={resource.loading ? '…' : metric(data?.devices)} foot="Topology inventory" icon="device" tone="blue" />
      <MetricCard label="Correlated faults" value={resource.loading ? '…' : metric(data?.correlatedFaults)} foot="Persisted fault groups" icon="activity" tone="purple" />
      <MetricCard label="Impacted devices" value={resource.loading ? '…' : metric(data?.impactedDevices)} foot="Propagation analysis" icon="topology" />
    </div>

    <RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && !data} onRetry={resource.reload}>
      <div className="dashboard-grid">
        <Panel title="Network posture" description="Live topology data from the topology service." action={<Link className="panel-link" to="/topology">Explore topology <Icon name="chevron" /></Link>}>
          <div className="network-preview">
            <div className="network-copy"><div className="network-copy-inner"><div className="network-orb"><Icon name="topology" /></div><h3>Topology service connected</h3><p>{data?.devices.toLocaleString() ?? '0'} devices are available for topology-aware monitoring.</p></div></div>
          </div>
        </Panel>
        <Panel title="Recent active alarms" description="The newest active records returned by the alarm services." action={<Link className="panel-link" to="/alarms">View all <Icon name="chevron" /></Link>}>
          <div className="feed-list">
            {data?.recentAlarms.map((alarm) => <div className="feed-row" key={`${alarm.source}-${alarm.id}`}><span className="feed-dot danger" /><div className="feed-content"><div className="feed-title">{alarm.source} · {alarm.alarmType}</div><div className="feed-meta">Device {alarm.deviceId} · {formatDate(alarm.raisedTime)}</div></div></div>)}
            {data?.recentAlarms.length === 0 && <div className="empty-state"><div><div className="empty-icon"><Icon name="bell" /></div><strong>No active alarms</strong><p>The alarm services returned no active records.</p></div></div>}
          </div>
        </Panel>
      </div>
    </RequestState>
  </>
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value))
}
