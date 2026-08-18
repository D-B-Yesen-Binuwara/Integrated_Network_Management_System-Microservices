import { Icon } from '../components/Icon'
import { BackendNotice, PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiResource } from '../hooks/useApiResource'

type RootCause = { id?: number | string; deviceId?: number | string; deviceType?: string; confidence?: number; status?: string; createdAt?: string }

export function CorrelationPage() {
  const resource = useApiResource<RootCause[]>('/api/impact-analysis/root-causes')
  const roots = resource.data ?? []
  return <>
    <PageHeader eyebrow="Command center / Correlation" title="Correlation desk" subtitle="Move from alarm clusters to probable root causes and impact paths."><button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh</button><button className="primary-button" type="button">Run analysis</button></PageHeader>
    <div className="detail-grid"><BackendNotice copy="Root-cause scores, supporting alarms, and propagation paths are read from the impact-analysis service." /><div className="detail-card"><div className="detail-label">Correlation mode</div><div className="detail-title">Topology-aware</div><p className="detail-copy">Windowed alarm matching with upstream and downstream traversal.</p></div><div className="detail-card"><div className="detail-label">Confidence model</div><div className="detail-title">Service supplied</div><p className="detail-copy">Confidence and evidence fields will be displayed from the correlation engine.</p></div></div>
    <Panel title="Probable root causes" description="Candidates returned by the impact analysis service."><RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && roots.length === 0} onRetry={resource.reload}><div className="table-wrap"><table className="data-table"><thead><tr><th>Candidate</th><th>Type</th><th>Confidence</th><th>State</th><th>Detected</th></tr></thead><tbody>{roots.map((root, index) => <tr key={String(root.id ?? index)}><td>{root.deviceId ?? '—'}</td><td>{root.deviceType ?? '—'}</td><td>{root.confidence === undefined ? '—' : `${Math.round(root.confidence * 100)}%`}</td><td><span className="status-badge investigating">{root.status ?? 'Investigating'}</span></td><td className="muted-cell">{root.createdAt ?? '—'}</td></tr>)}</tbody></table></div></RequestState></Panel>
  </>
}
