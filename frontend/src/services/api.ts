const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string | undefined)?.replace(/\/$/, '') ?? ''

export async function fetchJson<T>(path: string, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, { signal, headers: { Accept: 'application/json' } })
  if (!response.ok) throw new Error(`${response.status} ${response.statusText || 'Request failed'}`)
  return response.json() as Promise<T>
}

export function apiSetupMessage() {
  return API_BASE_URL ? `The service returned an error from ${API_BASE_URL}.` : 'The API base URL is not configured. Set VITE_API_BASE_URL when the backend is ready.'
}
