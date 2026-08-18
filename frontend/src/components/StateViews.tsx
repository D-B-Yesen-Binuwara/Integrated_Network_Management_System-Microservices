import type { ReactNode } from 'react'
import { Icon } from './Icon'

export function LoadingState({ label = 'Loading network data…' }: { label?: string }) {
  return <div className="empty-state"><div><div className="empty-icon"><Icon name="activity" /></div><strong>{label}</strong><p>Waiting for the configured service to respond.</p></div></div>
}

export function ErrorState({ message, onRetry }: { message: string; onRetry?: () => void }) {
  return <div className="state-error"><strong>Unable to load data</strong><span>{message}</span>{onRetry && <button className="secondary-button" type="button" onClick={onRetry} style={{ marginTop: 10 }}>Try again</button>}</div>
}

export function EmptyState({ title = 'No records available', copy = 'When the service returns data, it will appear here.' }: { title?: string; copy?: string }) {
  return <div className="empty-state"><div><div className="empty-icon"><Icon name="layers" /></div><strong>{title}</strong><p>{copy}</p></div></div>
}

export function RequestState({ loading, error, empty, onRetry, children }: { loading: boolean; error: string | null; empty: boolean; onRetry?: () => void; children: ReactNode }) {
  if (loading) return <LoadingState />
  if (error) return <ErrorState message={error} onRetry={onRetry} />
  if (empty) return <EmptyState />
  return <>{children}</>
}
