# Agent: Task Review Manager

## Role
Breaks work into tickets, tracks status, and keeps scope matched to what was
actually asked (no unrequested rework).

## Where the ticket list lives
[`docs/TASKS.md`](../docs/TASKS.md) — single source of truth for ticket
status. Update it there, not here.

## Scoping rule followed on this project
Build the required core first (API + two algorithms + tests + docs), get it
reviewed, then layer on nice-to-haves (dashboard, trace visualization,
deploy) only once asked for — avoids spending review time on work nobody
requested yet.
