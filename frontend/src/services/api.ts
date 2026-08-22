/**
 * Small, dependency-free HTTP client used by all frontend service modules.
 *
 * The frontend talks to the gateway only. Local development provides the HTTP
 * gateway URL in .env.local; production builds must configure this explicitly.
 */
const configuredBaseUrl = (import.meta.env.VITE_API_BASE_URL as string | undefined)?.trim()
export const API_BASE_URL = (configuredBaseUrl || (import.meta.env.DEV ? 'https://localhost:7030' : '')).replace(/\/$/, '')

const useCookieCredentials = import.meta.env.VITE_API_USE_COOKIES === 'true'

export class ApiError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

/** Fetches JSON and turns non-2xx responses into a safe, typed error. */
export async function fetchJson<T>(path: string, signal?: AbortSignal, init?: RequestInit): Promise<T> {
  if (!API_BASE_URL) {
    throw new ApiError(0, 'The API base URL is not configured for this build.')
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    signal,
    credentials: useCookieCredentials ? 'include' : 'same-origin',
    headers: {
      Accept: 'application/json',
      ...init?.headers,
    },
  })

  if (!response.ok) {
    throw new ApiError(response.status, await readErrorMessage(response))
  }

  if (response.status === 204) return undefined as T
  return response.json() as Promise<T>
}

/** Sends a JSON command through the same guarded transport as read requests. */
export function requestJson<T>(path: string, method: 'POST' | 'PUT' | 'PATCH' | 'DELETE', body?: unknown, signal?: AbortSignal) {
  return fetchJson<T>(path, signal, {
    method,
    body: body === undefined ? undefined : JSON.stringify(body),
    headers: {
      'Content-Type': 'application/json',
    },
  })
}

async function readErrorMessage(response: Response): Promise<string> {
  try {
    const body: unknown = await response.json()
    if (typeof body === 'object' && body !== null) {
      const record = body as Record<string, unknown>
      if (typeof record.message === 'string') return record.message
      if (typeof record.title === 'string') return record.title
    }
  } catch {
    // Some reverse proxies return an empty or non-JSON error body.
  }

  return response.statusText || 'The API request failed.'
}

export function apiSetupMessage(): string {
  return API_BASE_URL
    ? `The request was sent through ${API_BASE_URL}.`
    : 'Set VITE_API_BASE_URL to the gateway URL for this environment.'
}
