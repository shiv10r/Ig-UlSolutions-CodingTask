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

## What's in place for deployment
- Root `Dockerfile` (multi-stage: SDK build → ASP.NET runtime) builds just
  `src/ExpressionEvaluator.Api` for Render's Docker runtime.
  `Program.cs` binds Kestrel to Render's `PORT` env var automatically.
- `docs/DEPLOYMENT.md` has the full plan (Vercel for `dashboard/`, Render
  for the API, Supabase Postgres swapped in for local SQLite) and the
  required Render env vars.
- Local data store is SQLite (`expressionevaluator.db`, gitignored, never
  committed) with one seeded admin user — see `docs/ARCHITECTURE.md` for
  the seeding/secret-handling rules. On Render without a Postgres
  connection string, SQLite still runs but data is lost on every
  redeploy/restart (ephemeral filesystem) — fine for a quick demo, not for
  anything that needs to persist.

## Real incident: dashboard deployed alone fails silently

The dashboard was deployed to Vercel before the API existed anywhere public.
Result: page loads, every request fails with "Failed to fetch" — it was
still pointed at `http://localhost:5080`. Not a bug, but a real ordering
mistake worth avoiding next time: **deploy the API first**, get its public
URL, then deploy the dashboard pointed at it. See `docs/DEPLOYMENT.md` for
the fix (`VITE_API_BASE_URL` + redeploy — Vite bakes env vars at build time,
so setting the var without rebuilding does nothing).

## Checklist before adding a deploy step
- [ ] Confirm hosting target with the repo owner before wiring secrets/workflows.
- [ ] Never commit API keys/connection strings — use GitHub Actions secrets or Vercel/Render env vars.
- [ ] CI must stay green before any deploy workflow is added on top of it.
- [ ] Deploy order: API (Render) first, then the dashboard (Vercel) pointed at its public URL — never the reverse.
- [ ] `main` is the deploy branch for both Vercel and Render — not `develop01` or a feature branch.
- [ ] Vercel's deployed origin is in Render's `AllowedOrigins__*` env vars (use the stable production URL, not a per-deploy preview hash).
- [ ] `Jwt__Key` and `ConnectionStrings__Default` are set as Render env vars before the service is expected to actually work, not just to boot.
