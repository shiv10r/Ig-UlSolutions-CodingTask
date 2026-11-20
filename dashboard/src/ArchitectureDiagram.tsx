function Box({ x, y, w, h, label, sub, accent }: { x: number; y: number; w: number; h: number; label: string; sub?: string; accent?: boolean }) {
  return (
    <g>
      <rect
        x={x} y={y} width={w} height={h} rx={10}
        className={accent ? 'diagram-box diagram-box-accent' : 'diagram-box'}
      />
      <text x={x + w / 2} y={y + h / 2 + (sub ? -4 : 5)} textAnchor="middle" className="diagram-label">{label}</text>
      {sub && <text x={x + w / 2} y={y + h / 2 + 14} textAnchor="middle" className="diagram-sublabel">{sub}</text>}
    </g>
  )
}

function Arrow({ x1, y1, x2, y2 }: { x1: number; y1: number; x2: number; y2: number }) {
  return (
    <line x1={x1} y1={y1} x2={x2} y2={y2} className="diagram-arrow" markerEnd="url(#arrowhead)" />
  )
}

export default function ArchitectureDiagram() {
  return (
    <svg viewBox="0 0 760 480" role="img" aria-label="Architecture diagram" className="diagram-svg">
      <defs>
        <marker id="arrowhead" markerWidth="8" markerHeight="8" refX="7" refY="4" orient="auto">
          <path d="M0,0 L8,4 L0,8 Z" className="diagram-arrowhead" />
        </marker>
      </defs>

      <Box x={300} y={10} w={160} h={50} label="Browser" sub="React dashboard" />
      <Arrow x1={380} y1={60} x2={380} y2={100} />

      <Box x={60} y={100} w={640} h={160} label="" />
      <text x={80} y={122} className="diagram-group-label">ExpressionEvaluator.Api</text>

      <Box x={90} y={140} w={190} h={50} label="AuthController" sub="/api/auth/login, /logout" />
      <Box x={320} y={140} w={190} h={50} label="ExpressionController" sub="[Authorize]" accent />
      <Box x={550} y={140} w={130} h={50} label="Middleware" sub="JWT · CORS · rate limit" />

      <Arrow x1={380} y1={190} x2={380} y2={230} />
      <Box x={300} y={230} w={160} h={50} label="ExpressionEvaluationService" sub="orchestrates both" />

      <Arrow x1={185} y1={190} x2={185} y2={230} />
      <Box x={110} y={230} w={150} h={50} label="AppDbContext" sub="SQLite: Users" />

      <Arrow x1={380} y1={280} x2={380} y2={320} />
      <Box x={60} y={320} w={640} h={140} label="" />
      <text x={80} y={342} className="diagram-group-label">ExpressionEvaluator.Core</text>

      <Box x={90} y={360} w={170} h={50} label="Tokenizer" sub="validation + lexing" />
      <Arrow x1={260} y1={385} x2={320} y2={385} />
      <Box x={320} y={360} w={190} h={50} label="BruteForceEvaluator" sub="O(n²)" />
      <Box x={320} y={420} w={190} h={40} label="OptimizedEvaluator" sub="O(n)" />
      <Arrow x1={260} y1={400} x2={320} y2={440} />
    </svg>
  )
}
