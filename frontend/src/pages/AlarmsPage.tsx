import { useCallback, useState } from 'react'
import { Icon } from '../components/Icon'
import { PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiRequest } from '../hooks/useApiRequest'
import { getActiveAlarmsWithDeviceDetails, type AlarmSource } from '../services/networkApi'

const sources: Array<{ label: string; value: AlarmSource }> = [
  { label: 'All types', value: 'all' },
  { label: 'SLBN', value: 'SLBN' },
  { label: 'CEAN', value: 'CEAN' },
  { label: 'MSAN', value: 'MSAN' },
]

export function AlarmsPage() {
  const [source, setSource] = useState<AlarmSource>('all')
  const [query, setQuery] = useState('')
  const request = useCallback((signal: AbortSignal) => getActiveAlarmsWithDeviceDetails(source, signal), [source])
  const resource = useApiRequest(`active-alarms-${source}`, request)
  const rows = (resource.data ?? []).filter((alarm) => JSON.stringify(alarm).toLowerCase().includes(query.toLowerCase()))

  return <>
    <PageHeader eyebrow="Command center / Alarms" title="Alarm center" subtitle="Review external alarms by device family and severity.">
      <button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh</button>
      <button className="primary-button" type="button" disabled title="Export is not available until a report endpoint is added">Export report</button>
    </PageHeader>
    <div className="filter-bar">
      <div className="tab-list">{sources.map((item) => <button className={`tab${item.value === source ? ' active' : ''}`} key={item.value} type="button" onClick={() => setSource(item.value)}>{item.label}</button>)}</div>
      <label className="search-field"><Icon name="search" /><input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search alarm records" aria-label="Search alarm records" /></label>
    </div>
    <Panel title="Active alarm stream" description="Records are supplied by the alarm service; no local alarm fixtures are used.">
      <RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && rows.length === 0} onRetry={resource.reload}>
        <div className="table-wrap"><table className="data-table"><thead><tr><th>Source</th><th>Alarm</th><th>Device</th><th>IP address</th><th>Priority level</th><th>State</th><th>Raised</th></tr></thead><tbody>{rows.map((alarm) => <tr key={`${alarm.source}-${alarm.id}`}><td>{alarm.source}</td><td>{alarm.alarmType}</td><td>{alarm.deviceName}<span className="table-subtext">ID {alarm.deviceId}</span></td><td>{alarm.ip}</td><td><span className={`priority-badge priority-${alarm.priorityLevel.toLowerCase()}`}>{alarm.priorityLevel}</span></td><td><span className={`status-badge ${alarm.isActive ? 'critical' : 'resolved'}`}>{alarm.isActive ? 'Active' : 'Cleared'}</span></td><td className="muted-cell">{formatDate(alarm.raisedTime)}</td></tr>)}</tbody></table></div>
      </RequestState>
    </Panel>
  </>
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value))
}
