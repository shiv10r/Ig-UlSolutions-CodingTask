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
- Full deployment plan written: `docs/DEPLOYMENT.md` (Vercel for `dashboard/`,
  Render for the API, Supabase Postgres swapped in for local SQLite).
  Not executed yet — needs hosting accounts/secrets, which only the repo
  owner should hold.
- Local data store is SQLite (`expressionevaluator.db`, gitignored, never
  committed) with one seeded admin user — see `docs/ARCHITECTURE.md` for
  the seeding/secret-handling rules.

## Checklist before adding a deploy step
- [ ] Confirm hosting target with the repo owner before wiring secrets/workflows.
- [ ] Never commit API keys/connection strings — use GitHub Actions secrets or Vercel env vars.
- [ ] CI must stay green before any deploy workflow is added on top of it.
