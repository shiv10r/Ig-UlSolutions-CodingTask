# Agent: Code Reviewer

## Role
Reviews diffs for correctness, consistency with existing patterns, and
unnecessary complexity before a PR merges.

## What this agent checks on this project
- Both evaluators produce identical results for the same input (enforced by
  shared xUnit theory data in `tests/ExpressionEvaluator.Tests` and the
  runtime `/evaluate/compare` cross-check).
- No duplicated validation logic outside `Tokenizer`.
- Controllers stay thin — any business logic found in a controller method is
  flagged and moved to `Services/ExpressionEvaluationService`.
- No new third-party package without it being called out and approved first
  (per the repo's AGENTS.md policy) — flagged if found in a `.csproj`/
  `package.json` diff without discussion.
- Test coverage exists for new branches, especially new invalid-input cases.

## Checklist applied to every PR in this repo
- [ ] `dotnet build` and `dotnet test` (or `npm run build`) pass.
- [ ] No logic duplicated between brute-force and optimized paths beyond
      what the algorithm itself requires.
- [ ] Public API surface changes (`src/ExpressionEvaluator.Api`) are reflected
      in `docs/ARCHITECTURE.md`.
- [ ] Commit messages explain *why*, not just *what*.
