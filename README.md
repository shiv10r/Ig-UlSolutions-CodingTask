# Expression Evaluator API

UL Solutions coding task: a C# Web API that evaluates a string arithmetic
expression of non-negative integers and the operators `+ - * /`, with normal
operator precedence (brackets not required/supported).

```http
POST /api/expression/evaluate          -> optimized algorithm (default)
POST /api/expression/evaluate/brute-force
POST /api/expression/evaluate/compare  -> runs both, reports agreement
POST /api/expression/evaluate/trace    -> step-by-step trace of both algorithms
```

Request body: `{ "expression": "4+5*2" }`
Response: `{ "expression": "4+5*2", "result": 14, "algorithm": "optimized" }`

## Two implementations, same interface

| &nbsp; | `BruteForceExpressionEvaluator` | `OptimizedExpressionEvaluator` |
| --- | --- | --- |
| Approach | Repeated scan-and-reduce (`*`/`/` pass, then `+`/`-` pass) | Single-pass stack (push signed terms, resolve `*`/`/` immediately) |
| Time | O(n²) | O(n) |
| Space | O(n) | O(n) |
| Default endpoint | `/evaluate/brute-force` | `/evaluate` |

Both are hand-written (no expression-evaluation library) and validated by
the same `Tokenizer` and the same xUnit theory data, so they're held to an
identical contract — see [`docs/DESIGN.md`](docs/DESIGN.md).

## Project layout

```text
src/ExpressionEvaluator.Core    evaluation logic (framework-agnostic)
src/ExpressionEvaluator.Api     ASP.NET Core MVC Web API (Controllers/Services/Models)
tests/ExpressionEvaluator.Tests xUnit tests (61 tests)
dashboard/                      React/Vite/TS live demo + algorithm trace viewer
docs/                           architecture, design, QA, ticket list
agents/                         per-role responsibilities and checklists
CLAUDE.md                       how AI assistance was used in this repo
```

## Running locally

```bash
dotnet test                                    # run the test suite
dotnet run --project src/ExpressionEvaluator.Api --urls http://localhost:5080
curl -X POST http://localhost:5080/api/expression/evaluate \
  -H "Content-Type: application/json" -d "{\"expression\":\"4+5/2-1\"}"

# dashboard (separate terminal)
cd dashboard && npm install && npm run dev
```

## Security hardening applied

- Strict input whitelist + 1000-char length cap (`Tokenizer`), enforced
  before any parsing.
- Per-IP rate limiting (60 req/min).
- CORS locked to an explicit origin allow-list, not wildcard.
- `X-Frame-Options`, `X-Content-Type-Options` response headers.
- No dynamic code execution / expression library — hand-rolled parser only.

Full rationale in [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).

## AI-assisted development

Built with Claude Code across architect / design / implementation / QA
passes — see [`CLAUDE.md`](CLAUDE.md) and [`agents/`](agents/) for what each
role covered and why.

## Branching

Feature branches off `develop01`; PRs target `develop01`
([`docs/TASKS.md`](docs/TASKS.md) tracks ticket status). Merges from
`develop01` to `main` are handled by the repository owner.
