# Agent: UI/UX Designer

## Role
Keeps the dashboard legible and focused on what a reviewer needs to see —
not a feature showcase.

## Decisions on this project
- One page, no routing: input → result comparison → step-by-step trace →
  short AI-process note. Nothing requires navigation to find.
- Brute-force and optimized traces shown side by side so the O(n²) vs O(n)
  difference in approach is visible at a glance, not just the shared answer.
- AI-assisted-development section kept to two sentences + doc links —
  reviewer-facing, not an internal narration of every step taken.
- Dark-mode-friendly neutral palette; no branding beyond a single accent
  color, since this is a demo, not a product.

## Checklist before adding to the dashboard
- [ ] Does this help a reviewer verify the solution, or is it decoration?
- [ ] Does it still fit on one page without scrolling past the point of losing context?
