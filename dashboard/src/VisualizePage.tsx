import TraceStepper from './TraceStepper'
import type { TraceResult } from './types'

export default function VisualizePage({ trace }: { trace: TraceResult | null }) {
  if (!trace) {
    return (
      <div className="page">
        <section className="card">
          <h2>Visualize</h2>
          <p>Evaluate an expression on the Calculator tab first — the step-by-step trace for both algorithms shows up here.</p>
        </section>
      </div>
    )
  }

  return (
    <div className="page">
      <section className="card">
        <h2>How <code>{trace.expression}</code> was solved</h2>
        <p className="muted">Step through each algorithm independently — click a dot or use the controls.</p>
      </section>

      <section className="stepper-row">
        <TraceStepper title="Brute-force" badge="O(n²)" steps={trace.bruteForceSteps} />
        <TraceStepper title="Optimized" badge="O(n)" steps={trace.optimizedSteps} />
      </section>
    </div>
  )
}
