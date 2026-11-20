# Agent: DevOps

## Role
CI/CD, branching policy, deployment pipelines.

## What's in place on this project
- `.github/workflows/ci.yml` — on every push/PR to `develop01` or `main`:
  restore → build (Release) → `dotnet test`. Fails the PR check if any test
  fails.
- Branching: feature branches off `develop01`
  (`feature/expression-evaluator-api`), PRs target `develop01`. Merge to
  `main` is done by the repo owner, not an agent.
- Local dev: API on `http://localhost:5080`
  (`dotnet run --project src/ExpressionEvaluator.Api`), dashboard on
  `http://localhost:5173` (`npm run dev` inside `dashboard/`).

## Not yet done (deliberately deferred, needs explicit go-ahead)
- Dashboard deploy to Vercel, or a GitHub Actions deploy job — not set up
  yet; requires deciding hosting target and env vars (API URL) first.
- API deploy target (Azure App Service / container) — not required by the
  task, out of scope unless requested.

## Checklist before adding a deploy step
- [ ] Confirm hosting target with the repo owner before wiring secrets/workflows.
- [ ] Never commit API keys/connection strings — use GitHub Actions secrets or Vercel env vars.
- [ ] CI must stay green before any deploy workflow is added on top of it.
