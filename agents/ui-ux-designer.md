# Agent: UI/UX Designer

## Role
Keeps the dashboard legible, visual over textual, and focused on what a
reviewer needs to see — not a feature showcase.

## Decisions on this project (v2 redesign)
- Four tabs, state-based navigation (no router library added — avoids an
  unneeded dependency for four in-memory views): Calculator, Visualize,
  Architecture, About.
- Calculator results are KPI cards (big number, label, sub-label), not a
  text list — scannable in under a second.
- Trace is an interactive stepper (dots + prev/next) per algorithm, not a
  wall of numbered text — one step visible at a time, click to jump.
- Architecture is a live inline-SVG diagram rendered by the app itself, not
  a Mermaid code block (which depends on the viewer rendering it) or a
  static image export — guaranteed to display, and theme-aware.
- Light/dark theme toggle via CSS custom properties on `:root[data-theme]`,
  persisted in `localStorage`; system preference used as the first default.
- AI-assisted-development content moved to its own "About" tab instead of
  competing for space with the calculator — kept to a short paragraph + a
  single real link (the repo root, not deep branch-specific file links that
  can break).

## Checklist before adding to the dashboard
- [ ] Does this help a reviewer verify the solution, or is it decoration?
- [ ] Is it a card/visual, not another paragraph of prose?
- [ ] Does it work in both light and dark theme (uses CSS variables, not hardcoded colors)?
- [ ] Does adding it justify a new dependency, or does plain state/CSS do the job?
