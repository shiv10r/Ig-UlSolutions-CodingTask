export default function AboutPage() {
  return (
    <div className="page">
      <section className="card">
        <h2>About this build</h2>
        <p>
          Built with <strong>Claude Code</strong> (Claude Sonnet 5) across role-based
          passes — architecture, algorithm design, implementation, security, and QA —
          so each decision has a documented reason, not a single opaque generation.
        </p>
        <p className="muted">
          Full write-up, per-role notes, and the deployment plan live in the repo's
          <code> docs/</code> and <code>agents/</code> folders.
        </p>
        <p>
          <a href="https://github.com/shiv10r/Ig-UlSolutions-CodingTask" target="_blank" rel="noreferrer">
            View the repository ↗
          </a>
        </p>
      </section>
    </div>
  )
}
