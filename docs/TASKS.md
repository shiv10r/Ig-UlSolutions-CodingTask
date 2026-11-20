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
| 14 | Deploy dashboard (Vercel) / API (GitHub Actions or hosting) | Not started — awaiting hosting decision |
| 15 | Merge `feature/expression-evaluator-api` → `develop01` → `main` | PR #1 open against `develop01`; `main` merge is the repo owner's call |

## Branching
`feature/expression-evaluator-api` → `develop01` (PR #1). No ticket here
merges to `main` directly — that's done by the repository owner.
