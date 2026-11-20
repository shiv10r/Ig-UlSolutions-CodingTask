# Tickets

| # | Ticket | Status |
|---|---|---|
| 1 | Project scaffold: Core / Api (MVC) / Tests split | Done |
| 2 | Brute-force evaluator (O(n²)) | Done |
| 3 | Optimized evaluator (O(n)) | Done |
| 4 | Shared input validation (`Tokenizer`): length cap, charset, leading-zero rule | Done |
| 5 | API endpoints: `/evaluate`, `/evaluate/brute-force`, `/evaluate/compare` | Done |
| 6 | Security hardening: rate limiting, CORS allow-list, security headers | Done |
| 7 | MVC service layer (`IExpressionEvaluationService`) between controllers and Core | Done |
| 8 | xUnit test suite (evaluators, controller, service) | Done |
| 9 | Docs: `README.md`, `docs/ARCHITECTURE.md`, `docs/DESIGN.md`, `docs/QA.md`, `CLAUDE.md` | Done |
| 10 | `agents/` role-based instruction docs | Done |
| 11 | CI workflow (`dotnet test` on push/PR) | Done |
| 12 | React dashboard: live calculator demo | Done |
| 13 | Step-by-step algorithm trace (`/evaluate/trace`) + dashboard visualization | Done |
| 14 | Auth: SQLite + EF Core, JWT login/logout, seeded admin user, `[Authorize]` on all expression endpoints | Done |
| 15 | Self-registration (`/api/auth/register`) + combined login/register dashboard UI | Done |
| 16 | Dashboard redesign: tabbed nav, KPI cards, interactive trace stepper, live SVG architecture diagram, theme toggle | Done |
| 17 | Deployment plan (Vercel + Render + Supabase) | Documented in `docs/DEPLOYMENT.md` — not executed, awaiting hosting account setup |
| 18 | Merge `feature/expression-evaluator-api` → `develop01` → `main` | PR #1 merged. PR #2 (`feature → develop01`) and PR #3 (`develop01 → main`) open — merge #2 then #3 |

## Deliberately deferred (flagged, not silently skipped)

- Token revocation/refresh tokens (JWT logout is client-side only).
- Supabase/Render/Vercel deployment itself (plan is written; execution needs hosting credentials only you should hold).

## Branching

`feature/expression-evaluator-api` → `develop01` → `main`. Merges to `main`
are the repository owner's call, not an agent's.
