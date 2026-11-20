import { useEffect, useState } from 'react'
import './App.css'
import LoginForm from './LoginForm'
import Nav, { type Tab } from './Nav'
import CalculatorPage from './CalculatorPage'
import VisualizePage from './VisualizePage'
import ArchitecturePage from './ArchitecturePage'
import AboutPage from './AboutPage'
import { apiFetch, loadSession, logout, type AuthSession } from './auth'
import { applyTheme, getStoredTheme, type Theme } from './theme'
import type { ApiError, CompareResult, TraceResult } from './types'


function Dashboard({ session, onLogout }: { session: AuthSession; onLogout: () => void }) {
  const [theme, setTheme] = useState<Theme>(() => getStoredTheme())
  const [tab, setTab] = useState<Tab>('calculator')
  const [expression, setExpression] = useState('4+5/2-1')
  const [result, setResult] = useState<CompareResult | null>(null)
  const [trace, setTrace] = useState<TraceResult | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  useEffect(() => applyTheme(theme), [theme])

  async function post<T>(path: string, expr: string): Promise<T> {
    const res = await apiFetch(path, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${session.token}` },
      body: JSON.stringify({ expression: expr }),
    })
    if (res.status === 401) {
      onLogout()
      throw new Error('Session expired — please sign in again.')
    }
    if (!res.ok) {
      const body: ApiError = await res.json().catch(() => ({}))
      throw new Error(body.detail ?? `Request failed (${res.status})`)
    }
    return res.json()
  }

  async function evaluate(expr: string) {
    setLoading(true)
    setError(null)
    try {
      const [compareResult, traceResult] = await Promise.all([
        post<CompareResult>('/api/expression/evaluate/compare', expr),
        post<TraceResult>('/api/expression/evaluate/trace', expr),
      ])
      setResult(compareResult)
      setTrace(traceResult)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Could not reach the API.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="app-shell">
      <Nav
        active={tab}
        onChange={setTab}
        theme={theme}
        onToggleTheme={() => setTheme((t) => (t === 'dark' ? 'light' : 'dark'))}
        username={session.username}
        onLogout={onLogout}
      />
      {tab === 'calculator' && (
        <CalculatorPage
          expression={expression}
          onExpressionChange={setExpression}
          onEvaluate={evaluate}
          loading={loading}
          error={error}
          result={result}
          onGoVisualize={() => setTab('visualize')}
        />
      )}
      {tab === 'visualize' && <VisualizePage trace={trace} />}
      {tab === 'architecture' && <ArchitecturePage />}
      {tab === 'about' && <AboutPage />}
    </div>
  )
}

function App() {
  const [session, setSession] = useState<AuthSession | null>(() => loadSession())

  useEffect(() => applyTheme(getStoredTheme()), [])

  async function handleLogout() {
    if (session) await logout(session).catch(() => {})
    setSession(null)
  }

  return session
    ? <Dashboard session={session} onLogout={handleLogout} />
    : <LoginForm onLogin={setSession} />
}

export default App
