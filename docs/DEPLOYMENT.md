# Deployment plan

Not deployed yet — this documents the intended path so it can be done in one
pass once you're ready, without re-deciding the approach each time.

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
2. **Backend (Render)**: deploy `src/ExpressionEvaluator.Api` as a Render Web
   Service. Required environment variables:
   - `ConnectionStrings__Default` — Supabase Postgres connection string
   - `Jwt__Key` — a real, stable secret (32+ bytes); without it the app
     refuses to start outside Development, by design (see `Program.cs`)
   - `AllowedOrigins__0` — the deployed Vercel URL, so CORS allows it
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

DevOps checklist for deploy-related changes: `agents/devops.md`.
