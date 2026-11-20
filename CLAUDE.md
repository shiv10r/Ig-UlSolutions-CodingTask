# CLAUDE.md — AI collaborator brief for this repository

This file is read by Claude/Copilot-style coding agents working in this repo.
It records how this project is organized and how AI assistance was used, per
the "Use of AI" requirement in the coding task.

## What this repo is

A C# Web API that evaluates arithmetic string expressions (non-negative
integers, `+ - * /`, standard operator precedence, no brackets), built as a
small product rather than a single throwaway script: two interchangeable
evaluator implementations, tests, security hardening, and docs.

## Project layout

- `src/ExpressionEvaluator.Core` — evaluation logic, no web framework
  dependency. Two implementations of `IExpressionEvaluator`:
  - `BruteForceExpressionEvaluator` — O(n²) repeated-scan reduction.
  - `OptimizedExpressionEvaluator` — O(n) single-pass stack algorithm.
- `src/ExpressionEvaluator.Api` — ASP.NET Core MVC Web API. Controllers are
  thin; `Services/ExpressionEvaluationService` orchestrates the two
  evaluators. All `/api/expression/*` endpoints (`/evaluate`,
  `/evaluate/brute-force`, `/evaluate/compare`, `/evaluate/trace`) require a
  JWT (`[Authorize]`). `Data/AppDbContext` (SQLite locally) +
  `Services/AuthService` + `Controllers/AuthController` provide
  `/api/auth/login`, `/api/auth/register`, and `/api/auth/logout`.
- `tests/ExpressionEvaluator.Tests` — xUnit tests (evaluators, service,
  controller, auth; shared theory data so both algorithms are held to the
  same contract).
- `dashboard/` — React/Vite/TS: login/register, then a 4-tab app
  (Calculator, Visualize, Architecture, About). Separate folder, separate
  deployable unit from the API.
- `docs/` — architecture, design rationale, QA strategy, ticket list (see
  `docs/TASKS.md` for status).
- `agents/` — one file per project role (architecture, backend-dev,
  frontend-dev, code-reviewer, devops, doc-writer, qa-engineer,
  security-reviewer, task-manager, ui-ux-designer) with that role's
  responsibilities and a checklist for future changes.

## How AI was used

This solution was built with Claude Code (Anthropic) as a pair-programmer
across role-based passes, documented as separate "agent" notes so the
reasoning behind each decision is traceable rather than implicit:

1. **Architect pass** — chose the project split (Core/Api/Tests), the
   evaluator interface, and the security boundary (see `docs/ARCHITECTURE.md`).
2. **Design pass** — chose the brute-force and optimized algorithms and
   documented why each is the "obvious" solution for its category, including
   rejected alternatives like `DataTable.Compute` and a full recursive-descent
   parser (see `docs/DESIGN.md`).
3. **Implementation pass** — wrote `Tokenizer`, both evaluators, the
   controller, and `Program.cs` hardening (rate limiting, security headers,
   CORS allow-list, strict input validation).
4. **QA pass** — derived test cases from the task's examples plus edge cases
   (division by zero, malformed input, leading zeros, oversized input) and
   verified both algorithms agree on every case (see `docs/QA.md`).

No third-party expression-evaluation library was used; both evaluators are
hand-written per the task's requirement.

## Conventions for future AI-assisted changes in this repo

Per-role responsibilities and checklists (backend, frontend, QA, security,
DevOps, etc.) live in `agents/` — read the matching file before making a
change in that area, rather than re-deriving the rules here.

Branch from `develop01`, open PRs as `feature/* -> develop01`. Merges to
`main` are handled by the repository owner, not by an agent.
