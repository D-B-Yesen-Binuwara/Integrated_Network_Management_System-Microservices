import { useCallback, useEffect, useState } from 'react'
import { apiSetupMessage } from '../services/api'

export type ApiRequest<T> = (signal: AbortSignal) => Promise<T>

/**
 * Shared request lifecycle for pages: loading, error, retry and cancellation.
 * The AbortSignal prevents a slow response from updating an unmounted page.
 */
export function useApiRequest<T>(requestKey: string, request: ApiRequest<T>) {
  const [data, setData] = useState<T | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [refreshKey, setRefreshKey] = useState(0)

  const reload = useCallback(() => setRefreshKey((key) => key + 1), [])

  useEffect(() => {
    const controller = new AbortController()
    const timeout = window.setTimeout(() => controller.abort(), 15_000)
    let active = true

    const load = async () => {
      setLoading(true)
      setError(null)

      try {
        const nextData = await request(controller.signal)
        if (active) setData(nextData)
      } catch (reason: unknown) {
        if (!active) return

        if (reason instanceof DOMException && reason.name === 'AbortError') {
          setError('The request timed out or was cancelled. Check that the gateway is running.')
        } else {
          const requestError = reason instanceof Error ? reason.message : 'Unknown request error'
          setError(`${requestError} ${apiSetupMessage()}`)
        }
      } finally {
        window.clearTimeout(timeout)
        if (active) setLoading(false)
      }
    }

    void load()

    return () => {
      active = false
      window.clearTimeout(timeout)
      controller.abort()
    }
  }, [request, requestKey, refreshKey])

  return { data, loading, error, reload }
}
