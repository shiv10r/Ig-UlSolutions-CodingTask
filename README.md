# Expression Evaluator

UL Solutions coding task: a C# Web API that evaluates a string arithmetic
expression of non-negative integers and the operators `+ - * /`, with normal
operator precedence (brackets not required/supported) — built out as a small
full-stack app: authenticated API + React dashboard.

## What this repo can do

- **Evaluate expressions** two ways — a brute-force O(n²) algorithm and an
  optimized O(n) algorithm — and compare them, or get a step-by-step trace
  of exactly how each one reached its answer.
- **Authenticate** — JWT-based login/logout; every evaluation endpoint is
  protected, nothing is reachable anonymously.
- **Persist users** in a local SQLite database (swappable for Supabase
  Postgres later with no code change — see `docs/DEPLOYMENT.md`).
- **Visualize it live** in a React dashboard: sign in, run an expression,
  see both algorithms' results side by side and their step-by-step traces.
- **Explain itself** — every engineering decision (why this algorithm, why
  this layering, why this security control) is written down in `docs/` and
  `agents/`, not left implicit.

## API endpoints

```http
POST /api/auth/login                   -> { username, password } -> JWT
POST /api/auth/logout                  -> [Authorize] confirms token is valid

POST /api/expression/evaluate          -> [Authorize] optimized algorithm (default)
POST /api/expression/evaluate/brute-force
POST /api/expression/evaluate/compare  -> runs both, reports agreement
POST /api/expression/evaluate/trace    -> step-by-step trace of both algorithms
```

Example: `{ "expression": "4+5*2" }` → `{ "expression": "4+5*2", "result": 14, "algorithm": "optimized" }`

## Two evaluator implementations, same interface

| &nbsp; | `BruteForceExpressionEvaluator` | `OptimizedExpressionEvaluator` |
| --- | --- | --- |
| Approach | Repeated scan-and-reduce (`*`/`/` pass, then `+`/`-` pass) | Single-pass stack (push signed terms, resolve `*`/`/` immediately) |
| Time | O(n²) | O(n) |
| Space | O(n) | O(n) |
| Default endpoint | `/evaluate/brute-force` | `/evaluate` |

Both are hand-written (no expression-evaluation library) and validated by
the same `Tokenizer` and the same xUnit theory data, so they're held to an
identical contract — see [`docs/DESIGN.md`](docs/DESIGN.md).

## Project layout

```text
src/ExpressionEvaluator.Core    evaluation logic (framework-agnostic)
src/ExpressionEvaluator.Api     ASP.NET Core MVC Web API (Controllers/Services/Data/Models)
tests/ExpressionEvaluator.Tests xUnit tests (64 tests)
dashboard/                      React/Vite/TS: login + live calculator + trace viewer
docs/                           architecture, design, QA, deployment plan, ticket list
agents/                         per-role responsibilities and checklists
CLAUDE.md                       how AI assistance was used in this repo
```

## Running locally

```bash
# API (first run creates expressionevaluator.db and seeds one admin user;
# the generated password is printed ONCE to this console — save it)
dotnet run --project src/ExpressionEvaluator.Api --urls http://localhost:5080

# tests
dotnet test

# dashboard (separate terminal)
cd dashboard && npm install && npm run dev
```

Open `http://localhost:5173`, sign in with the admin username/password from
the API console log, then try an expression.

## Authentication & data

- SQLite file (`expressionevaluator.db`) is local-only and **gitignored** —
  never committed. One `admin` user is seeded on first run with a random
  password, logged once and nowhere else.
- JWTs are stateless, 60-minute expiry; `/api/auth/logout` is a client-side
  convention (discard the token) since there's no server-side session to
  revoke.
- Full rationale: [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md#authentication--data-store).

## Security hardening applied

- Strict input whitelist + 1000-char length cap (`Tokenizer`), enforced
  before any parsing.
- JWT auth required on every expression endpoint; BCrypt password hashing;
  signing key never committed (config/env-var only).
- Per-IP rate limiting (60 req/min); CORS locked to an explicit allow-list.
- `X-Frame-Options`, `X-Content-Type-Options` response headers.
- No dynamic code execution / expression library — hand-rolled parser only.
- A transitive dependency vulnerability (NU1903, `SQLitePCLRaw`) was caught
  during dependency install and pinned to a patched version before merge.

Full rationale in [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).

## Deployment

Not deployed yet. The intended path — Vercel (dashboard) + Render (API) +
Supabase Postgres (swapped in for local SQLite) — is fully documented in
[`docs/DEPLOYMENT.md`](docs/DEPLOYMENT.md), including exact steps and env vars.

## AI-assisted development

Built with Claude Code across architect / design / implementation / QA /
security passes — see [`CLAUDE.md`](CLAUDE.md) and [`agents/`](agents/) for
what each role covered and why.

## Branching

Feature branches off `develop01`; PRs target `develop01`
([`docs/TASKS.md`](docs/TASKS.md) tracks ticket status). Merges from
`develop01` to `main` are handled by the repository owner.
