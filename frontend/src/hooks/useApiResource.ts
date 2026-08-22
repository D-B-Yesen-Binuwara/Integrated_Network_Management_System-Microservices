import { useCallback } from 'react'
import { fetchJson } from '../services/api'
import { useApiRequest } from './useApiRequest'

/** Convenience wrapper for one GET endpoint. */
export function useApiResource<T>(path: string) {
  const request = useCallback((signal: AbortSignal) => fetchJson<T>(path, signal), [path])
  return useApiRequest(path, request)
}
