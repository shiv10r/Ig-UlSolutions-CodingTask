# Agent: Architecture

## Role
Owns project structure, layering, and technology choices. Decides where code
lives and how components talk to each other, not what each algorithm does.

## Responsibilities on this project
- Chose the solution split: `src/ExpressionEvaluator.Core` (framework-free
  logic), `src/ExpressionEvaluator.Api` (ASP.NET Core MVC Web API),
  `tests/ExpressionEvaluator.Tests`, `dashboard/` (React, separate folder,
  separate deployable unit).
- Defined the backend as MVC: `Controllers/` (HTTP only) → `Services/`
  (`IExpressionEvaluationService`, orchestration) → `Core` evaluators (pure
  logic) → `Models/` (request/response DTOs). No business logic in
  controllers; no HTTP concerns in `Core`.
- Picked keyed DI (`"bruteForce"` / `"optimized"`) for the two
  `IExpressionEvaluator` implementations so a third algorithm is a
  registration, not a rewrite.
- Decided repo layout: one repo (monorepo), not split backend/frontend
  repos — standard for a take-home exercise reviewed as a single link.

## Reference
Full diagram and request flow: [`docs/ARCHITECTURE.md`](../docs/ARCHITECTURE.md).

## Checklist for future structural changes
- [ ] Does `Core` stay free of ASP.NET Core references?
- [ ] Does the controller stay thin (delegates to a service, no algorithm logic)?
- [ ] Is a new algorithm added as `IExpressionEvaluator` + keyed DI entry, not a branch in existing code?
- [ ] Does `docs/ARCHITECTURE.md` get updated alongside the change?
