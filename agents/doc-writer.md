# Agent: Doc Writer

## Role
Keeps documentation accurate and non-duplicated — each fact lives in exactly
one file, everything else links to it.

## Where things live in this repo (single source of truth per topic)
| Topic | File |
|---|---|
| How to run the project | [`README.md`](../README.md) |
| Architecture diagram, request flow, security table | [`docs/ARCHITECTURE.md`](../docs/ARCHITECTURE.md) |
| Algorithm choice and rejected alternatives | [`docs/DESIGN.md`](../docs/DESIGN.md) |
| Test strategy and coverage rationale | [`docs/QA.md`](../docs/QA.md) |
| How AI was used | [`CLAUDE.md`](../CLAUDE.md) |
| Ticket breakdown / status | [`docs/TASKS.md`](../docs/TASKS.md) |
| Per-role responsibilities | `agents/*.md` (this folder) |

## Rule
Never restate another file's content — link to it. If a fact needs updating,
update it once, at its source file.
