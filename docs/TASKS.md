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
| 8 | xUnit test suite (evaluators, controller, service) | Done — 61 tests |
| 9 | Docs: `README.md`, `docs/ARCHITECTURE.md`, `docs/DESIGN.md`, `docs/QA.md`, `CLAUDE.md` | Done |
| 10 | `agents/` role-based instruction docs | Done |
| 11 | CI workflow (`dotnet test` on push/PR) | Done |
| 12 | React dashboard: live calculator demo | Done |
| 13 | Step-by-step algorithm trace (`/evaluate/trace`) + dashboard visualization | Done |
| 14 | Auth: SQLite + EF Core, JWT login/logout, seeded admin user, `[Authorize]` on all expression endpoints | Done |
| 15 | Dashboard login screen + session handling (token storage, 401 → re-login) | Done |
| 16 | Deployment plan (Vercel + Render + Supabase) | Documented in `docs/DEPLOYMENT.md` — not executed, awaiting hosting account setup |
| 17 | Merge `feature/expression-evaluator-api` → `develop01` → `main` | PR #1 open against `develop01`; `main` merge is the repo owner's call |

## Deliberately deferred (flagged, not silently skipped)

- Token revocation/refresh tokens (JWT logout is client-side only).
- User registration/management UI (one seeded admin user only).
- Supabase/Render/Vercel deployment itself (plan is written; execution needs hosting credentials only you should hold).

## Branching
`feature/expression-evaluator-api` → `develop01` (PR #1). No ticket here
merges to `main` directly — that's done by the repository owner.
