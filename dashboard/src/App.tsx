import { useState } from 'react'
import './App.css'

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080'

type CompareResult = {
  expression: string
  bruteForceResult: number
  optimizedResult: number
  match: boolean
}

type EvaluationStep = { stepNumber: number; description: string; state: string }

type TraceResult = {
  expression: string
  bruteForceSteps: EvaluationStep[]
  optimizedSteps: EvaluationStep[]
  result: number
}

type ApiError = { detail?: string; title?: string }

const EXAMPLES = ['4+5*2', '4+5/2', '4+5/2-1']

function App() {
  const [expression, setExpression] = useState('4+5/2-1')
  const [result, setResult] = useState<CompareResult | null>(null)
  const [trace, setTrace] = useState<TraceResult | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  async function post<T>(path: string, expr: string): Promise<T> {
    const res = await fetch(`${API_BASE}${path}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ expression: expr }),
    })
    if (!res.ok) {
      const body: ApiError = await res.json().catch(() => ({}))
      throw new Error(body.detail ?? `Request failed (${res.status})`)
    }
    return res.json()
  }

  async function evaluate(expr: string) {
    setLoading(true)
    setError(null)
    setResult(null)
    setTrace(null)
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
    <main className="page">
      <header>
        <h1>Expression Evaluator</h1>
        <p className="subtitle">UL Solutions coding task — live demo</p>
      </header>

      <section className="card">
        <h2>Try it</h2>
        <div className="input-row">
          <input
            value={expression}
            onChange={(e) => setExpression(e.target.value)}
            placeholder="e.g. 4+5/2-1"
            onKeyDown={(e) => e.key === 'Enter' && evaluate(expression)}
          />
          <button onClick={() => evaluate(expression)} disabled={loading}>
            {loading ? 'Evaluating…' : 'Evaluate'}
          </button>
        </div>

        <div className="examples">
          {EXAMPLES.map((ex) => (
            <button key={ex} className="example-chip" onClick={() => { setExpression(ex); evaluate(ex) }}>
              {ex}
            </button>
          ))}
        </div>

        {error && <p className="error">{error}</p>}

        {result && (
          <div className="result">
            <div className="result-row">
              <span className="label">Brute-force (O(n²))</span>
              <span className="value">{result.bruteForceResult}</span>
            </div>
            <div className="result-row">
              <span className="label">Optimized (O(n))</span>
              <span className="value">{result.optimizedResult}</span>
            </div>
            <div className={`match-badge ${result.match ? 'ok' : 'fail'}`}>
              {result.match ? '✓ Both algorithms agree' : '✗ Mismatch'}
            </div>
          </div>
        )}
      </section>

      {trace && (
        <section className="card">
          <h2>How each algorithm got there</h2>
          <div className="trace-columns">
            <div className="trace-column">
              <h3>Brute-force (O(n²))</h3>
              <ol className="steps">
                {trace.bruteForceSteps.map((step) => (
                  <li key={step.stepNumber}>
                    <span className="step-desc">{step.description}</span>
                    <span className="step-state">{step.state}</span>
                  </li>
                ))}
              </ol>
            </div>
            <div className="trace-column">
              <h3>Optimized (O(n))</h3>
              <ol className="steps">
                {trace.optimizedSteps.map((step) => (
                  <li key={step.stepNumber}>
                    <span className="step-desc">{step.description}</span>
                    <span className="step-state">{step.state}</span>
                  </li>
                ))}
              </ol>
            </div>
          </div>
        </section>
      )}

      <section className="card">
        <h2>AI-assisted development</h2>
        <p>
          Built with <strong>Claude Code</strong> (Claude Sonnet 5), run as role-based
          passes — architecture, algorithm design, implementation, and QA — so each
          decision has a documented rationale rather than a single opaque generation.
        </p>
        <p className="docs-links">
          <a href="https://github.com/shiv10r/Ig-UlSolutions-CodingTask/blob/develop01/CLAUDE.md" target="_blank" rel="noreferrer">AI process</a>
          {' · '}
          <a href="https://github.com/shiv10r/Ig-UlSolutions-CodingTask/blob/develop01/docs/ARCHITECTURE.md" target="_blank" rel="noreferrer">Architecture</a>
          {' · '}
          <a href="https://github.com/shiv10r/Ig-UlSolutions-CodingTask/blob/develop01/docs/DESIGN.md" target="_blank" rel="noreferrer">Design</a>
          {' · '}
          <a href="https://github.com/shiv10r/Ig-UlSolutions-CodingTask/blob/develop01/docs/QA.md" target="_blank" rel="noreferrer">QA</a>
        </p>
      </section>
    </main>
  )
}

export default App
