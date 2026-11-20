import { useState } from 'react'
import { login, type AuthSession } from './auth'

export default function LoginForm({ onLogin }: { onLogin: (session: AuthSession) => void }) {
  const [username, setUsername] = useState('admin')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setLoading(true)
    setError(null)
    try {
      onLogin(await login(username, password))
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Login failed.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="page">
      <header>
        <h1>Expression Evaluator</h1>
        <p className="subtitle">Sign in to use the live demo</p>
      </header>

      <form className="card" onSubmit={handleSubmit}>
        <h2>Login</h2>
        <label className="field">
          Username
          <input value={username} onChange={(e) => setUsername(e.target.value)} autoComplete="username" />
        </label>
        <label className="field">
          Password
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="current-password"
          />
        </label>
        {error && <p className="error">{error}</p>}
        <button type="submit" disabled={loading}>
          {loading ? 'Signing in…' : 'Sign in'}
        </button>
        <p className="hint">
          Default admin credentials were printed once in the API server log on first run.
        </p>
      </form>
    </main>
  )
}
