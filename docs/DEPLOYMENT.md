# Deployment plan

In progress — dashboard deployed to Vercel, API deployment to Render being
set up. This documents the path end to end.

## Target: Vercel (dashboard) + Render (API) + Supabase (Postgres)

| Piece | Today (local) | Target |
|---|---|---|
| Frontend | `dashboard/`, Vite dev server | Vercel static deploy (`npm run build` → `dashboard/dist`) |
| Backend | `src/ExpressionEvaluator.Api`, `dotnet run` | Render (Docker or native .NET web service) |
| Database | SQLite file (`expressionevaluator.db`, local, gitignored) | Supabase Postgres |

## Steps

1. **Database**: swap `Microsoft.EntityFrameworkCore.Sqlite` for
   `Npgsql.EntityFrameworkCore.PostgreSQL` in
   `ExpressionEvaluator.Api.csproj`, change `options.UseSqlite(...)` to
   `options.UseNpgsql(...)` in `Program.cs`. `AppDbContext`/`User` need no
   change — EF Core abstracts the provider. Point `ConnectionStrings:Default`
   at the Supabase connection string via an environment variable, never
   committed.
2. **Backend (Render)**: a root-level `Dockerfile` builds and publishes
   `src/ExpressionEvaluator.Api` (multi-stage: SDK image to build, ASP.NET
   runtime image to run). Deploy as a Render Web Service with **Branch:
   `main`**, **Runtime: Docker**. `Program.cs` honors Render's `PORT` env
   var automatically (binds Kestrel to it) — no extra config needed for
   that part. Required environment variables:
   - `ConnectionStrings__Default` — Supabase Postgres connection string
     (SQLite still works for a quick first deploy, but Render's filesystem
     is ephemeral — data is lost on every redeploy/restart until this is
     swapped in)
   - `Jwt__Key` — a real, stable secret (32+ bytes); without it the app
     refuses to start outside Development, by design (see `Program.cs`)
   - `AllowedOrigins__0` — the deployed Vercel URL(s) that should be allowed
     to call this API (exact origin, e.g. `https://your-app.vercel.app`,
     no trailing slash); add `AllowedOrigins__1`, etc. for more
   - `AllowedOriginPatterns__0` — optional, for Vercel preview URLs whose
     hash changes every deploy, e.g. `https://dashboard-*-vsr11.vercel.app`.
     Exactly one `*`, inside a label that also has fixed text, followed by
     at least two fixed labels; it matches letters/digits/hyphens only,
     never a dot. Anything wider (`*`, `https://*.vercel.app`) makes the API
     refuse to start, so a typo can't silently open CORS.
   - `Seed__AdminPassword` — optional. If set, an `admin` user is created
     with it on first start (never logged). If not set, no admin is seeded
     on Render — use **Create account** on the login screen instead.

   Set Render's **Health Check Path** to `/health` (anonymous, not rate
   limited). Behind Render's proxy the API trusts only the last
   `X-Forwarded-For` hop, so rate limiting is per real client, not shared
   by everyone coming through the proxy.
3. **Frontend (Vercel)**: deploy `dashboard/` with build command
   `npm run build`, output directory `dist`. Set `VITE_API_BASE_URL` to the
   Render API's public URL.
4. **CI**: extend `.github/workflows/ci.yml` with a deploy job gated on
   `main`, triggering Render's deploy hook and `vercel --prod` — only after
   you confirm hosting accounts/secrets are ready; not wired up yet.

## Why this split

Matches what you said you'd use (Supabase/Render/Vercel) and needs no
architecture change — `IExpressionEvaluationService`/`IAuthService` don't
know or care what's behind `AppDbContext`, so swapping SQLite → Postgres is
a provider + connection-string change, not a rewrite.

## Known failure mode: dashboard deployed alone

Deploying only `dashboard/` to Vercel without an API live anywhere produces
a dashboard that loads but fails every request ("Failed to fetch") — it's
still pointed at `http://localhost:5080` by default. This is expected, not
a bug: both pieces (API + dashboard) need to be live, API first, with
`VITE_API_BASE_URL` set in Vercel and a redeploy triggered afterward (Vite
bakes env vars in at build time, so the var alone isn't enough).

## Known failure mode: Render had no Dockerfile

First Render deploy attempt failed at build: `open Dockerfile: no such file
or directory` — Render was configured for a Docker-runtime build but the
repo had none yet. Fixed by adding a root `Dockerfile` (see above).

## Known failure mode: CORS blocks the deployed dashboard

After the API was live, requests from the deployed dashboard failed with a
browser CORS error (not "Failed to fetch" — the preflight `OPTIONS` request
succeeded with 204, but the actual request was blocked) because the
Vercel URL wasn't in `AllowedOrigins` on the Render deployment. Fix: add
the exact Vercel origin to `AllowedOrigins__0` (etc.) in Render's
environment variables. Vercel **preview** URLs change per deploy (the
random hash in `dashboard-<hash>-<team>.vercel.app`) - use
`AllowedOriginPatterns__0=https://dashboard-*-<team>.vercel.app` to cover
them all, or point `AllowedOrigins__0` at Vercel's stable production URL.

The dashboard now reports these failures as `Can't reach the API at
{url}...` instead of a bare "Failed to fetch", so a wrong
`VITE_API_BASE_URL` or missing CORS entry is visible on screen.

## Live URLs and post-deploy check

- API: `https://ig-ulsolutions-codingtask.onrender.com` (Render, `main`)
- Dashboard: `https://dashboard-ten-fawn-97.vercel.app` (Vercel, `main`)

After every merge to `main` and every redeploy, run:

```bash
bash scripts/smoke-test.sh
```

It checks health, register/login, the three task examples, division by
zero, CORS for the site origin, that the site is public, and that the live
bundle points at this API. Deploys aren't done until it prints
`RESULT: PASS`.

## Known failure mode: trailing slash in VITE_API_BASE_URL

`VITE_API_BASE_URL=https://...onrender.com/` made the dashboard call
`//api/...` (404). The dashboard now strips trailing slashes. If a browser
still sends `//api/...`, it's running a cached old bundle or an old
deployment URL: hard-refresh the production URL.

DevOps checklist for deploy-related changes: `agents/devops.md`.
