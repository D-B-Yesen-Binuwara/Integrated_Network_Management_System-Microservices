import { useCallback } from 'react'
import { Icon } from '../components/Icon'
import { BackendNotice, PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiRequest } from '../hooks/useApiRequest'
import { getAnalyticsSnapshot } from '../services/networkApi'

export function AnalyticsPage() {
  const request = useCallback((signal: AbortSignal) => getAnalyticsSnapshot(signal), [])
  const resource = useApiRequest('analytics-snapshot', request)
  const snapshot = resource.data
  const hasData = Boolean(snapshot && (snapshot.activeAlarms.length > 0 || snapshot.faults.length > 0))

  return <>
    <PageHeader eyebrow="Network intelligence / Analytics" title="Analytics studio" subtitle="Operational analytics derived from live alarm and correlation responses.">
      <button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh</button>
      <button className="primary-button" type="button" disabled title="Natural-language analytics is not implemented yet">Ask analytics</button>
    </PageHeader>
    <div className="detail-grid"><BackendNotice copy="The AI/Analytics microservice is not implemented yet, so this view currently summarizes live alarm and correlation service data." /><div className="detail-card"><div className="detail-label">Active alarms</div><div className="detail-title">{resource.loading ? 'Loading…' : snapshot?.activeAlarms.length.toLocaleString() ?? '—'}</div><p className="detail-copy">Current records across all alarm tables.</p></div><div className="detail-card"><div className="detail-label">Snapshot time</div><div className="detail-title">{snapshot ? formatDate(snapshot.generatedAt) : '—'}</div><p className="detail-copy">Generated in the frontend from backend responses.</p></div></div>
    <div className="dashboard-grid"><Panel title="Live alarm distribution" description="Counts are calculated from the active alarm APIs."><RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && !hasData} onRetry={resource.reload}>{snapshot && <div className="feed-list">{countBySource(snapshot.activeAlarms).map((item) => <div className="feed-row" key={item.source}><span className="feed-dot info" /><div className="feed-content"><div className="feed-title">{item.source} alarms</div><div className="feed-meta">{item.count.toLocaleString()} active records</div></div></div>)}</div>}</RequestState></Panel><Panel title="Correlated fault groups" description="Persisted fault groups returned by the correlation API."><RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && (snapshot?.faults.length ?? 0) === 0} onRetry={resource.reload}>{snapshot && <div className="feed-list">{snapshot.faults.slice(0, 6).map((fault) => <div className="feed-row" key={fault.correlatedFaultId}><span className="feed-dot warning" /><div className="feed-content"><div className="feed-title">{fault.correlationRuleName}</div><div className="feed-meta">{fault.sourceDeviceType} device {fault.sourceDeviceId} · {fault.status}</div></div></div>)}</div>}</RequestState></Panel></div>
  </>
}

function countBySource(alarms: Array<{ source: string }>) {
  return ['SLBN', 'CEAN', 'MSAN'].map((source) => ({ source, count: alarms.filter((alarm) => alarm.source === source).length }))
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value))
}
