import { useCallback, useEffect, useState } from 'react'
import { apiSetupMessage, fetchJson } from '../services/api'

export function useApiResource<T>(path: string) {
  const [data, setData] = useState<T | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [refreshKey, setRefreshKey] = useState(0)

  const reload = useCallback(() => setRefreshKey((key) => key + 1), [])

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setError(null)
    fetchJson<T>(path, controller.signal)
      .then(setData)
      .catch((reason: unknown) => {
        if (reason instanceof DOMException && reason.name === 'AbortError') return
        const requestError = reason instanceof Error ? reason.message : 'Unknown request error'
        setError(`${requestError}. ${apiSetupMessage()}`)
      })
      .finally(() => setLoading(false))
    return () => controller.abort()
  }, [path, refreshKey])

  return { data, loading, error, reload }
}
