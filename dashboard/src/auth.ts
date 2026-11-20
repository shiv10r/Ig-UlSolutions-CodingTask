const API_BASE = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080'
const STORAGE_KEY = 'expressionEvaluator.auth'

export type AuthSession = { token: string; username: string }

export function loadSession(): AuthSession | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    return raw ? (JSON.parse(raw) as AuthSession) : null
  } catch {
    return null
  }
}

function saveSession(session: AuthSession) {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(session))
  } catch {
    // ignore storage failures (private browsing, blocked storage, etc.)
  }
}

function clearSession() {
  try {
    localStorage.removeItem(STORAGE_KEY)
  } catch {
    // ignore
  }
}

async function authRequest(path: string, username: string, password: string): Promise<AuthSession> {
  const res = await fetch(`${API_BASE}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password }),
  })
  if (!res.ok) {
    const body = await res.json().catch(() => ({}))
    throw new Error(body.message ?? body.detail ?? 'Request failed.')
  }
  const data = await res.json()
  const session: AuthSession = { token: data.token, username: data.username }
  saveSession(session)
  return session
}

export function login(username: string, password: string): Promise<AuthSession> {
  return authRequest('/api/auth/login', username, password)
}

export function register(username: string, password: string): Promise<AuthSession> {
  return authRequest('/api/auth/register', username, password)
}

export async function logout(session: AuthSession): Promise<void> {
  try {
    await fetch(`${API_BASE}/api/auth/logout`, {
      method: 'POST',
      headers: { Authorization: `Bearer ${session.token}` },
    })
  } finally {
    clearSession()
  }
}
