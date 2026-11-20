import { useState } from 'react'

type EvaluationStep = { stepNumber: number; description: string; state: string }

export default function TraceStepper({ title, badge, steps }: { title: string; badge: string; steps: EvaluationStep[] }) {
  const [index, setIndex] = useState(0)
  const step = steps[index]

  return (
    <div className="stepper">
      <div className="stepper-header">
        <h3>{title}</h3>
        <span className="pill">{badge}</span>
      </div>

      <div className="stepper-dots">
        {steps.map((s, i) => (
          <button
            key={s.stepNumber}
            className={`dot ${i === index ? 'dot-active' : ''} ${i < index ? 'dot-done' : ''}`}
            onClick={() => setIndex(i)}
            aria-label={`Step ${i + 1}`}
          />
        ))}
      </div>

      <div className="stepper-card">
        <p className="stepper-desc">{step.description}</p>
        <p className="stepper-state">{step.state}</p>
      </div>

      <div className="stepper-controls">
        <button onClick={() => setIndex((i) => Math.max(0, i - 1))} disabled={index === 0}>‹ Prev</button>
        <span className="stepper-progress">{index + 1} / {steps.length}</span>
        <button onClick={() => setIndex((i) => Math.min(steps.length - 1, i + 1))} disabled={index === steps.length - 1}>Next ›</button>
      </div>
    </div>
  )
}
