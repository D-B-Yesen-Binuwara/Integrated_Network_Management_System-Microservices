import { useCallback } from 'react'
import { Icon } from '../components/Icon'
import { BackendNotice, PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiRequest } from '../hooks/useApiRequest'
import { getCorrelationFaults } from '../services/networkApi'

export function CorrelationPage() {
  const request = useCallback((signal: AbortSignal) => getCorrelationFaults(signal), [])
  const resource = useApiRequest('correlation-faults', request)
  const faults = resource.data ?? []

  return <>
    <PageHeader eyebrow="Command center / Correlation" title="Correlation desk" subtitle="Move from alarm clusters to probable root causes and impact paths.">
      <button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh</button>
      <button className="primary-button" type="button" disabled title="Correlation runs when alarms are created or updated">Auto on alarm ingest</button>
    </PageHeader>
    <div className="detail-grid"><BackendNotice copy="Root-cause records, confidence, and suppressed alarm evidence are read from the alarm/correlation service." /><div className="detail-card"><div className="detail-label">Correlation mode</div><div className="detail-title">Topology-aware</div><p className="detail-copy">Windowed alarm matching with upstream and downstream traversal.</p></div><div className="detail-card"><div className="detail-label">Fault records</div><div className="detail-title">{resource.loading ? 'Loading…' : faults.length.toLocaleString()}</div><p className="detail-copy">Current persisted correlation groups.</p></div></div>
    <Panel title="Probable root causes" description="Candidates returned by the alarm/correlation service.">
      <RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && faults.length === 0} onRetry={resource.reload}>
        <div className="table-wrap"><table className="data-table"><thead><tr><th>Rule</th><th>Source</th><th>Alarm</th><th>Confidence</th><th>State</th><th>Detected</th></tr></thead><tbody>{faults.map((fault) => <tr key={fault.correlatedFaultId}><td>{fault.correlationRuleName}</td><td>{fault.sourceDeviceType} · {fault.sourceDeviceId}</td><td>{fault.sourceAlarmType}</td><td>{Math.round(fault.confidenceScore * 100)}%</td><td><span className={`status-badge ${fault.status.toLowerCase() === 'active' ? 'critical' : 'resolved'}`}>{fault.status}</span></td><td className="muted-cell">{formatDate(fault.startedAt)}</td></tr>)}</tbody></table></div>
      </RequestState>
    </Panel>
  </>
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value))
}
