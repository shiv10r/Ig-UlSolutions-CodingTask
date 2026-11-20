type Tab = 'calculator' | 'visualize' | 'architecture' | 'about'

const TABS: { id: Tab; label: string }[] = [
  { id: 'calculator', label: 'Calculator' },
  { id: 'visualize', label: 'Visualize' },
  { id: 'architecture', label: 'Architecture' },
  { id: 'about', label: 'About' },
]

export default function Nav({
  active, onChange, theme, onToggleTheme, username, onLogout,
}: {
  active: Tab
  onChange: (tab: Tab) => void
  theme: 'light' | 'dark'
  onToggleTheme: () => void
  username: string
  onLogout: () => void
}) {
  return (
    <nav className="nav">
      <div className="nav-brand">Expression Evaluator</div>
      <div className="nav-tabs">
        {TABS.map((tab) => (
          <button
            key={tab.id}
            className={`nav-tab ${active === tab.id ? 'active' : ''}`}
            onClick={() => onChange(tab.id)}
          >
            {tab.label}
          </button>
        ))}
      </div>
      <div className="nav-actions">
        <button className="icon-button" onClick={onToggleTheme} aria-label="Toggle theme" title="Toggle light/dark theme">
          {theme === 'dark' ? '☀️' : '🌙'}
        </button>
        <span className="nav-user">{username}</span>
        <button className="link-button" onClick={onLogout}>Log out</button>
      </div>
    </nav>
  )
}

export type { Tab }
