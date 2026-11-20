# Expression Evaluator — Dashboard

Small React (Vite + TypeScript) demo for the Expression Evaluator API: sign
in via `/api/auth/login`, then navigate four tabs —

- **Calculator** — enter an expression, see brute-force vs optimized results
  as KPI-style cards with an agreement badge.
- **Visualize** — step through each algorithm's trace interactively (dots,
  prev/next), instead of a wall of text.
- **Architecture** — a live-rendered inline-SVG diagram (not a static image
  or a Mermaid block that may not render), theme-aware.
- **About** — short AI-assisted-development note and repo link.

Light/dark theme toggle (nav bar, persisted in `localStorage`) applies
across all four.

## Run locally

```bash
# 1. start the API (from repo root)
dotnet run --project ../src/ExpressionEvaluator.Api --urls http://localhost:5080

# 2. start the dashboard
npm install
npm run dev
```

Set `VITE_API_BASE_URL` (see `.env.example`) if the API runs elsewhere.

Sign in with the `admin` username and the password printed once to the API's
console log on its first run, or use **Create account** to register a new
user (`/api/auth/register`).

Frontend/UI checklists: `agents/frontend-dev.md` and `agents/ui-ux-designer.md`.
