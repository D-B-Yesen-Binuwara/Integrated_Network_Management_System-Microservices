import type { ReactNode } from 'react'
import { Icon } from './Icon'

export function PageHeader({ eyebrow, title, subtitle, children }: { eyebrow: string; title: string; subtitle: string; children?: ReactNode }) {
  return <div className="page-heading"><div><div className="eyebrow">{eyebrow}</div><h1>{title}</h1><p className="page-subtitle">{subtitle}</p></div>{children && <div className="heading-actions">{children}</div>}</div>
}

export function MetricCard({ label, value, foot, icon, tone = '' }: { label: string; value: string; foot: string; icon: 'alarm' | 'device' | 'activity' | 'topology'; tone?: string }) {
  return <div className="metric-card"><div className="metric-head"><span>{label}</span><span className={`metric-icon ${tone}`}><Icon name={icon} /></span></div><div className="metric-value pending">{value}</div><div className="metric-foot">{foot}</div></div>
}

export function Panel({ title, description, action, children, className = '' }: { title: string; description?: string; action?: ReactNode; children: ReactNode; className?: string }) {
  return <section className={`panel ${className}`}><div className="panel-header"><div><h2 className="panel-title">{title}</h2>{description && <p className="panel-description">{description}</p>}</div>{action}</div>{children}</section>
}

export function BackendNotice({ copy = 'This view is ready for live service data. Connect the API gateway to populate it.' }: { copy?: string }) {
  return <div className="detail-card"><div className="detail-label">Frontend preview</div><div className="detail-title">Backend not connected</div><p className="detail-copy">{copy}</p></div>
}
