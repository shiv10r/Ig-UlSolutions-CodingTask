# Expression Evaluator — Dashboard

Small React (Vite + TypeScript) demo for the Expression Evaluator API: a live
calculator that calls `/api/expression/evaluate/compare` and shows both the
brute-force and optimized results side by side.

## Run locally

```bash
# 1. start the API (from repo root)
dotnet run --project ../src/ExpressionEvaluator.Api --urls http://localhost:5080

# 2. start the dashboard
npm install
npm run dev
```

Set `VITE_API_BASE_URL` (see `.env.example`) if the API runs elsewhere.
