# Expression Evaluator — Dashboard

Small React (Vite + TypeScript) demo for the Expression Evaluator API: sign
in via `/api/auth/login`, then a live calculator that calls
`/api/expression/evaluate/compare` and `/evaluate/trace` and shows both the
brute-force and optimized results (and how each got there) side by side.

## Run locally

```bash
# 1. start the API (from repo root)
dotnet run --project ../src/ExpressionEvaluator.Api --urls http://localhost:5080

# 2. start the dashboard
npm install
npm run dev
```

Set `VITE_API_BASE_URL` (see `.env.example`) if the API runs elsewhere.

Sign in with the `admin` username and the password printed once to the API's
console log on its first run.
