# Design — Design agent notes

## Problem

Evaluate a string like `4+5/2-1` with standard precedence (`*`, `/` before
`+`, `-`), non-negative integers only, no brackets, no third-party expression
library, calculation performed server-side.

## Solution 1 — Brute force (`BruteForceExpressionEvaluator`)

Tokenize into a list of numbers and a list of operators, then do two full
left-to-right passes over the operator list: first collapse every `*`/`/`
pair in place, then collapse every `+`/`-` pair in place. Each collapse
removes one operator and one operand, so a pass is O(n) and is repeated,
giving **O(n²) time, O(n) space**.

This is the "obviously correct, easy to read and verify by eye" version —
closest to how you'd compute it by hand with a pencil: do all the
multiplication/division first, then add/subtract what's left.

## Solution 2 — Optimized (`OptimizedExpressionEvaluator`)

Single left-to-right pass (the standard "Basic Calculator II" technique):
keep a stack of signed terms. For `+`/`-`, push the (possibly negated) number.
For `*`/`/`, pop the previous term, combine immediately, push the result back.
Sum the stack at the end. **O(n) time, O(n) space, one pass, no backtracking.**

This is the one that scales to long expressions without quadratic blowup
and is the default used by `/api/expression/evaluate`.

## Alternatives considered and rejected

- **`DataTable.Compute` / `System.Data` expression evaluation** — disallowed
  by the task ("must use your own calculation logic rather than a
  third-party expression-evaluation library"); `DataTable.Compute` is
  effectively that.
- **Full recursive-descent / Shunting-yard parser** — correct and standard
  for expressions that need brackets and more operators, but more machinery
  than this task's grammar (no brackets, 4 flat operators) needs. Kept as a
  natural extension point if brackets are added later — `Tokenizer` already
  separates lexing from evaluation, so a recursive-descent evaluator could
  be dropped in behind `IExpressionEvaluator` without touching the API.
- **Regex-based reduction** (repeatedly regex-replace `\d+[*/]\d+` style
  patterns) — works for this grammar but is slower, harder to extend, and
  easy to get subtly wrong with multi-digit numbers and overlapping matches.
  The explicit tokenizer is clearer and was preferred for maintainability.

## Data contract

`double` is used for results (not `int`/`decimal`) because `/` can produce
non-integers (`4+5/2 = 6.5`), matching the task's expected output exactly.
Division by zero throws `ExpressionException`, surfaced as HTTP 400 rather
than `double.PositiveInfinity`/`NaN`, since a silently "valid-looking" float
result would be a worse API contract than an explicit error.

## Input validation rules (enforced once, in `Tokenizer`)

- Non-empty, max 1000 characters.
- Only digits and `+ - * /` (whitespace tolerated and skipped).
- No leading zeros (`07` rejected) to avoid ambiguity with octal-looking
  input and keep numeric parsing unambiguous.
- Cannot start or end with an operator; no two operators back-to-back
  (catches unary minus, which the task doesn't require since inputs are
  non-negative integers only).
