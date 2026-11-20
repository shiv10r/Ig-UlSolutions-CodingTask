import type { CompareResult } from './types'

const EXAMPLES = ['4+5*2', '4+5/2', '4+5/2-1']

export default function CalculatorPage({
  expression, onExpressionChange, onEvaluate, loading, error, result, onGoVisualize,
}: {
  expression: string
  onExpressionChange: (v: string) => void
  onEvaluate: (expr: string) => void
  loading: boolean
  error: string | null
  result: CompareResult | null
  onGoVisualize: () => void
}) {
  return (
    <div className="page">
      <section className="card">
        <h2>Try an expression</h2>
        <div className="input-row">
          <input
            value={expression}
            onChange={(e) => onExpressionChange(e.target.value)}
            placeholder="e.g. 4+5/2-1"
            onKeyDown={(e) => e.key === 'Enter' && onEvaluate(expression)}
          />
          <button onClick={() => onEvaluate(expression)} disabled={loading}>
            {loading ? 'Evaluating…' : 'Evaluate'}
          </button>
        </div>

        <div className="examples">
          {EXAMPLES.map((ex) => (
            <button key={ex} className="example-chip" onClick={() => { onExpressionChange(ex); onEvaluate(ex) }}>
              {ex}
            </button>
          ))}
        </div>

        {error && <p className="error">{error}</p>}
      </section>

      {result && (
        <section className="kpi-row">
          <div className="kpi-card">
            <span className="kpi-label">Brute-force</span>
            <span className="kpi-value">{result.bruteForceResult}</span>
            <span className="kpi-sub">O(n²)</span>
          </div>
          <div className="kpi-card kpi-accent">
            <span className="kpi-label">Optimized</span>
            <span className="kpi-value">{result.optimizedResult}</span>
            <span className="kpi-sub">O(n)</span>
          </div>
          <div className={`kpi-card ${result.match ? 'kpi-ok' : 'kpi-fail'}`}>
            <span className="kpi-label">Agreement</span>
            <span className="kpi-value">{result.match ? '✓' : '✗'}</span>
            <span className="kpi-sub">{result.match ? 'Both match' : 'Mismatch'}</span>
          </div>
        </section>
      )}

      {result && (
        <button className="cta" onClick={onGoVisualize}>See how each algorithm got there →</button>
      )}
    </div>
  )
}
