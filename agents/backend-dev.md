# Agent: Backend Developer

## Role
Implements the C# API: evaluator logic, MVC controllers/services, DI wiring.

## Responsibilities on this project
- `ExpressionEvaluator.Core`: `Tokenizer` (shared validation/lexing),
  `BruteForceExpressionEvaluator` (O(n²)), `OptimizedExpressionEvaluator`
  (O(n)), both also exposing `EvaluateWithTrace` for the step-by-step
  visualization the dashboard consumes.
- `ExpressionEvaluator.Api` (MVC layering):
  - **Models** — `EvaluateRequest`, `EvaluateResponse`, `EvaluateCompareResponse`, `TraceResponse`.
  - **Services** — `ExpressionEvaluationService` implements
    `IExpressionEvaluationService`, the only thing controllers talk to.
  - **Controllers** — `ExpressionController` exposes `/evaluate`,
    `/evaluate/brute-force`, `/evaluate/compare`, `/evaluate/trace`.
- `Program.cs`: DI registration, rate limiting, CORS, security headers, JWT
  auth setup, SQLite `AppDbContext` registration, first-run admin seeding.
- **Auth**: `Data/User.cs` (entity), `Data/AppDbContext.cs` (EF Core, SQLite
  locally), `Services/IAuthService`/`AuthService` (BCrypt verify + JWT
  issue), `Controllers/AuthController` (`/api/auth/login`, `/logout`).
  `ExpressionController` is `[Authorize]` at the class level.

## Conventions
- All input validation happens once, in `Tokenizer` — never duplicated in a
  controller or evaluator.
- `double` is the result type (division can be fractional); division by
  zero throws `ExpressionException`, never returns `Infinity`/`NaN`.
- No third-party expression-evaluation library, per the task's requirement.

## Checklist before adding an endpoint or evaluator
- [ ] New evaluator implements `IExpressionEvaluator` fully, including `EvaluateWithTrace`.
- [ ] Controller method is a thin wrapper: validate → call service → map to DTO → return.
- [ ] Added to DI in `Program.cs` (keyed singleton).
- [ ] Covered by the shared theory data in `tests/ExpressionEvaluator.Tests`.
