# QA — QA agent notes

## Strategy

Both evaluator implementations must satisfy the same contract, so the test
suite uses **shared theory data** (`ExpressionEvaluatorTheoryData`) run
against both `BruteForceExpressionEvaluator` and `OptimizedExpressionEvaluator`.
If they ever disagree, the failing test names exactly which implementation
and which case regressed.

The `/compare` controller endpoint and its test (`Compare_ValidExpression_
BothAlgorithmsAgree`) additionally give a runtime cross-check that both
algorithms agree, which is also useful for manual/API-level verification
during review.

## Coverage

**Valid expressions** (from the task's own examples plus edge cases):
- `4+5*2 = 14`, `4+5/2 = 6.5`, `4+5/2-1 = 5.5` — the task's exact examples.
- Repeated same-precedence operators (`2*3*4`, `10-2-3`, `10/2/5`) — checks
  left-to-right associativity, not just precedence.
- `0+0`, bare `100` — boundary/trivial cases.
- Mixed precedence (`1+2*3-4/2`) — multiple precedence switches in one pass.

**Invalid expressions** (must throw `ExpressionException`, mapped to HTTP 400):
- Empty / whitespace-only input.
- Double operators (`4++5`), leading operator (`+4+5`), trailing operator
  (`4+5+`).
- Division by zero (`4+5/0`).
- Brackets (`4+(5*2)`) — explicitly out of scope per the task.
- Decimals (`4+5.5`) — task specifies integers only.
- Unary minus (`4+-5`) — task specifies non-negative integers only.
- Non-numeric tokens (`four+five`).
- Leading zero (`07+5`) — rejected per the design rule in `DESIGN.md`.
- Oversized input (1000 `+`-joined terms) — confirms the length cap is
  actually enforced, not just documented.

## Controller-level tests

`ExpressionControllerTests` exercises the controller directly (constructed
with real evaluator instances, no mocking needed since the evaluators are
pure functions) to confirm: correct algorithm is invoked per route, response
shape (`EvaluateResponse` / `EvaluateCompareResponse`), and that invalid
input surfaces as an HTTP 400 `ProblemDetails`, not a 500.

An ASP.NET Core `WebApplicationFactory`-based end-to-end test (asserting
behavior through real HTTP, including the rate limiter and CORS middleware)
was considered but deferred: it requires adding the
`Microsoft.AspNetCore.Mvc.Testing` NuGet package, and this repo's policy is
not to add third-party dependencies without discussion/approval first. The
controller-level tests already exercise the same production code path
(`ExpressionController` → `IExpressionEvaluator`) without that dependency.

## Auth coverage

`AuthServiceTests` uses an in-memory SQLite connection (`Microsoft.Data.Sqlite`,
already available transitively via the `ExpressionEvaluator.Api` project
reference — no new test package needed) to verify, without hitting the real
local database file:
- Correct username/password returns a well-formed JWT (3 dot-separated parts).
- Wrong password returns `null` (no token issued).
- Unknown username returns `null` (no username-enumeration timing shortcut).

Not covered by an automated test: the full HTTP 401 round-trip on protected
`ExpressionController` routes (requires `Microsoft.AspNetCore.Mvc.Testing`,
the same dependency deferred above) — verified manually instead (see
`docs/ARCHITECTURE.md` and the PR description for the curl trace: no token →
401, valid token → 200, wrong password → 401).

## Running the tests

```bash
dotnet test
```

64 tests, all passing as of this pass (2 evaluators × ~21 theory cases,
trace tests, service/controller tests, and 3 auth service tests).
