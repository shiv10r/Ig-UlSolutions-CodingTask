import { useEffect, useState } from 'react'
import { login, register, type AuthSession } from './auth'
import { applyTheme, getStoredTheme } from './theme'

type Mode = 'login' | 'register'

export default function LoginForm({ onLogin }: { onLogin: (session: AuthSession) => void }) {
  const [mode, setMode] = useState<Mode>('login')
  const [username, setUsername] = useState('admin')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  useEffect(() => applyTheme(getStoredTheme()), [])

  function switchMode(next: Mode) {
    setMode(next)
    setError(null)
    setUsername(next === 'login' ? 'admin' : '')
    setPassword('')
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setLoading(true)
    setError(null)
    try {
      const session = mode === 'login' ? await login(username, password) : await register(username, password)
      onLogin(session)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="login-shell">
      <div className="login-badge">Σ</div>
      <h1 className="login-title">Expression Evaluator</h1>
      <p className="muted login-subtitle">UL Solutions coding task — live demo</p>

      <form className="card login-card" onSubmit={handleSubmit}>
        <div className="auth-toggle">
          <button type="button" className={mode === 'login' ? 'active' : ''} onClick={() => switchMode('login')}>
            Sign in
          </button>
          <button type="button" className={mode === 'register' ? 'active' : ''} onClick={() => switchMode('register')}>
            Create account
          </button>
        </div>

        <label className="field">
          Username
          <input
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            autoComplete="username"
            minLength={3}
            required
          />
        </label>
        <label className="field">
          Password
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete={mode === 'login' ? 'current-password' : 'new-password'}
            minLength={mode === 'register' ? 8 : undefined}
            required
          />
          {mode === 'register' && <span className="field-hint">At least 8 characters.</span>}
        </label>

        {error && <p className="error">{error}</p>}

        <button type="submit" disabled={loading}>
          {loading && <span className="spinner" />}
          {loading ? 'Please wait…' : mode === 'login' ? 'Sign in' : 'Create account'}
        </button>

        {mode === 'login' && (
          <p className="hint">
            Default admin credentials were printed once in the API server log on first run.
          </p>
        )}
      </form>
    </main>
  )
}
