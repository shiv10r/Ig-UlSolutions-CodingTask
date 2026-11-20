# Agent: QA Engineer

## Role
Defines and maintains test coverage; the last gate before a PR is called done.

## What's covered on this project (61 xUnit tests)
- Shared theory data (`ExpressionEvaluatorTheoryData`) run against both
  evaluators — same valid/invalid cases, both must agree.
- `EvaluationTraceTests` — trace output ends at the correct result with more
  than one step (catches an evaluator that silently stopped tracing).
- `ExpressionControllerTests` — route → correct algorithm → correct DTO →
  correct status code (200 vs 400).
- `ExpressionEvaluationServiceTests` — service-layer cross-check that both
  algorithms match the expected value, not just each other.

## Full rationale
See [`docs/QA.md`](../docs/QA.md) — do not restate it here.

## Definition of done for a change in this repo
- [ ] `dotnet test` passes with 0 failures.
- [ ] Every new branch/condition has at least one test.
- [ ] A new invalid-input case is added to `InvalidExpressions`, not a
      one-off test, so both evaluators are checked against it.
