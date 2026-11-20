# Agent: Frontend Developer

## Role
Builds the `dashboard/` React app — a thin client over the API, no business
logic duplicated client-side.

## Responsibilities on this project
- Scaffolded `dashboard/` with Vite + React + TypeScript (separate folder
  from the backend, separate `package.json`, deploys independently).
- `App.tsx`: live calculator — calls `/api/expression/evaluate/compare` and
  `/api/expression/evaluate/trace`, renders brute-force vs optimized results,
  a match badge, and a side-by-side step-by-step trace of both algorithms.
- `VITE_API_BASE_URL` env var points at the API; defaults to
  `http://localhost:5080` for local dev (see `.env.example`).
- No calculation logic in the frontend — every number shown comes from the
  API response, per the task's "calculation must be performed in the API,
  not a UI" requirement.

## Checklist before changing the dashboard
- [ ] Any new number/result on screen comes from an API response, not computed in TS.
- [ ] `npm run build` passes (`tsc -b && vite build`) before pushing.
- [ ] CORS origin added to backend `appsettings.json` `AllowedOrigins` if the dev port changes.
