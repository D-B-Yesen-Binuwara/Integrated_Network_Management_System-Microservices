import { Icon } from '../components/Icon'
import { BackendNotice, PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiResource } from '../hooks/useApiResource'

type AnalyticsResponse = { generatedAt?: string; reports?: unknown[]; summary?: string }

export function AnalyticsPage() {
  const resource = useApiResource<AnalyticsResponse>('/api/analytics/summary')
  const reports = resource.data?.reports ?? []
  return <>
    <PageHeader eyebrow="Network intelligence / Analytics" title="Analytics studio" subtitle="Turn alarm history and fault relationships into useful operational signals."><button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh</button><button className="primary-button" type="button">Ask analytics</button></PageHeader>
    <div className="detail-grid"><BackendNotice copy="Charts and AI summaries will be rendered from the analytics service once its endpoint is available." /><div className="detail-card"><div className="detail-label">Reporting window</div><div className="detail-title">Service supplied</div><p className="detail-copy">Date and area filters will be connected to historical event queries.</p></div><div className="detail-card"><div className="detail-label">Generated at</div><div className="detail-title">{resource.data?.generatedAt ?? '—'}</div><p className="detail-copy">No local or synthetic analytics values are used.</p></div></div>
    <div className="dashboard-grid"><Panel title="Alarm trend" description="Historical alarm volumes by device family and severity."><div className="chart-placeholder"><RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && reports.length === 0} onRetry={resource.reload}>{null}</RequestState></div></Panel><Panel title="AI event summary" description="Plain-language summaries from the analytics service."><RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && !resource.data?.summary} onRetry={resource.reload}><div className="empty-state"><div><div className="empty-icon"><Icon name="spark" /></div><strong>No summary generated</strong><p>{resource.data?.summary ?? 'A summary will appear after analytics data is fetched.'}</p></div></div></RequestState></Panel></div>
  </>
}
