import { useState } from 'react'
import { Icon } from '../components/Icon'
import { PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiResource } from '../hooks/useApiResource'

type Alarm = { id?: number | string; alarmType?: string; deviceId?: number | string; severity?: string; status?: string; occurredAt?: string }
const sources = [{ label: 'All types', path: '/api/alarms/active' }, { label: 'SLBN', path: '/api/slbn-alarms/active' }, { label: 'CEAN', path: '/api/cea-alarms/active' }, { label: 'MSAN', path: '/api/msan-alarms/active' }]

export function AlarmsPage() {
  const [source, setSource] = useState(sources[0])
  const [query, setQuery] = useState('')
  const resource = useApiResource<Alarm[]>(source.path)
  const rows = (resource.data ?? []).filter((alarm) => JSON.stringify(alarm).toLowerCase().includes(query.toLowerCase()))
  return <>
    <PageHeader eyebrow="Command center / Alarms" title="Alarm center" subtitle="Review external alarms by device family and severity."><button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh</button><button className="primary-button" type="button">Export report</button></PageHeader>
    <div className="filter-bar"><div className="tab-list">{sources.map((item) => <button className={`tab${item.path === source.path ? ' active' : ''}`} key={item.label} type="button" onClick={() => setSource(item)}>{item.label}</button>)}</div><label className="search-field"><Icon name="search" /><input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search alarm records" aria-label="Search alarm records" /></label><button className="secondary-button" type="button"><Icon name="download" />Export</button></div>
    <Panel title="Active alarm stream" description="Records are supplied by the alarm service; no preview records are inserted locally."><RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && rows.length === 0} onRetry={resource.reload}><div className="table-wrap"><table className="data-table"><thead><tr><th>Alarm</th><th>Device</th><th>Severity</th><th>State</th><th>Occurred</th></tr></thead><tbody>{rows.map((alarm, index) => <tr key={String(alarm.id ?? index)}><td>{alarm.alarmType ?? 'Unnamed alarm'}</td><td>{alarm.deviceId ?? '—'}</td><td>{alarm.severity ?? '—'}</td><td><span className={`status-badge ${alarm.status?.toLowerCase() === 'resolved' ? 'resolved' : 'critical'}`}>{alarm.status ?? 'Active'}</span></td><td className="muted-cell">{alarm.occurredAt ?? '—'}</td></tr>)}</tbody></table></div></RequestState></Panel>
  </>
}
