# Agent: Security Reviewer

## Role
Checks every change against the security boundary before it merges.

## What's enforced on this project
Full table in [`docs/ARCHITECTURE.md`](../docs/ARCHITECTURE.md#security-considerations-applied) —
summary of what to re-check on every change:
- Input only ever reaches an evaluator through `Tokenizer` (length cap,
  character whitelist, no leading zeros).
- No `eval`, no `DataTable.Compute`, no dynamic code generation, no
  third-party expression library.
- Rate limiting (60 req/min/IP) stays on the global pipeline in `Program.cs`.
- CORS stays an explicit allow-list (`AllowedOrigins`), never `AllowAnyOrigin`.
- Error responses (`ExpressionException` → 400) never leak stack traces.
- No new npm/NuGet package added without explicit approval (this repo's
  `AGENTS.md` policy) — flag any `.csproj`/`package.json` diff that adds one
  without a prior discussion. (This caught a real issue: adding EF Core
  Sqlite pulled a transitively vulnerable `SQLitePCLRaw` — NU1903 — pinned
  to a patched version before merge.)
- Passwords are never stored or logged in plaintext (BCrypt hash only); the
  one-time seeded admin password is logged once to the console and nowhere
  else — never committed to the repo.
- JWT signing key is configuration-only (env var/secret store), never a
  hardcoded string; the app refuses to start without one outside Development.
- Every `ExpressionController` route requires `[Authorize]` — a diff that
  adds a new route must add it inside that controller (inheriting the
  class-level `[Authorize]`) or justify `[AllowAnonymous]` explicitly.

## Checklist for every PR
- [ ] Does the diff add a new way to reach `Evaluate`/`EvaluateWithTrace` that bypasses `Tokenizer`?
- [ ] Does the diff widen CORS, remove rate limiting, or remove a security header?
- [ ] Does the diff add a dependency? If yes, was it explicitly approved?
