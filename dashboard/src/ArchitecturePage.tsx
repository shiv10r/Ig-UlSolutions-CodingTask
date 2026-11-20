import ArchitectureDiagram from './ArchitectureDiagram'

export default function ArchitecturePage() {
  return (
    <div className="page">
      <section className="card">
        <h2>Architecture</h2>
        <p className="muted">Browser → API (auth + evaluation) → services → core algorithms. Rendered live, not a static image.</p>
        <ArchitectureDiagram />
      </section>

      <section className="kpi-row">
        <div className="kpi-card">
          <span className="kpi-label">Auth</span>
          <span className="kpi-value">JWT</span>
          <span className="kpi-sub">every endpoint protected</span>
        </div>
        <div className="kpi-card">
          <span className="kpi-label">Data</span>
          <span className="kpi-value">SQLite</span>
          <span className="kpi-sub">swappable for Postgres</span>
        </div>
        <div className="kpi-card">
          <span className="kpi-label">Tests</span>
          <span className="kpi-value">64</span>
          <span className="kpi-sub">all passing</span>
        </div>
      </section>
    </div>
  )
}
