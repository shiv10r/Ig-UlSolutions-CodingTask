# Expression Evaluator API

UL Solutions coding task: a C# Web API that evaluates a string arithmetic
expression of non-negative integers and the operators `+ - * /`, with normal
operator precedence (brackets not required/supported).

```
POST /api/expression/evaluate        -> optimized algorithm (default)
POST /api/expression/evaluate/brute-force
POST /api/expression/evaluate/compare -> runs both, reports agreement
```

Request body: `{ "expression": "4+5*2" }`
Response: `{ "expression": "4+5*2", "result": 14, "algorithm": "optimized" }`

## Two implementations, same interface

| | `BruteForceExpressionEvaluator` | `OptimizedExpressionEvaluator` |
|---|---|---|
| Approach | Repeated scan-and-reduce (`*`/`/` pass, then `+`/`-` pass) | Single-pass stack (push signed terms, resolve `*`/`/` immediately) |
| Time | O(n²) | O(n) |
| Space | O(n) | O(n) |
| Default endpoint | `/evaluate/brute-force` | `/evaluate` |

Both are hand-written (no expression-evaluation library) and validated by
the same `Tokenizer` and the same xUnit theory data, so they're held to an
identical contract — see [`docs/DESIGN.md`](docs/DESIGN.md).

## Project layout

```
src/ExpressionEvaluator.Core    evaluation logic (framework-agnostic)
src/ExpressionEvaluator.Api     ASP.NET Core Web API
tests/ExpressionEvaluator.Tests xUnit tests (47 tests)
docs/ARCHITECTURE.md            architecture diagram + security notes
docs/DESIGN.md                  algorithm choices and rejected alternatives
docs/QA.md                      test strategy and coverage rationale
CLAUDE.md                       how AI assistance was used in this repo
```

## Running locally

```bash
dotnet test                                    # run the test suite
dotnet run --project src/ExpressionEvaluator.Api
curl -X POST http://localhost:5xxx/api/expression/evaluate \
  -H "Content-Type: application/json" -d "{\"expression\":\"4+5/2-1\"}"
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
passes — see [`CLAUDE.md`](CLAUDE.md) for what each pass covered and why.

## Branching

Feature branches off `develop01`; PRs target `develop01`. Merges from
`develop01` to `main` are handled by the repository owner.
