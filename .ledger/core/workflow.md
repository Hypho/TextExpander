# PACT Workflow Reference
> Stable reference for the core feature flow. Adapter files and user-facing docs may summarize this file, but should not redefine the workflow differently.

---

## Core Flow

```text
pid -> contract -> build -> verify -> ship
```

`scope` is recommended before the first feature and whenever product direction or risk boundaries change. It is not a state-machine phase.

`retro` runs after 3-5 shipped features or when intent drift is suspected.

---

## Phase Reference

| Phase | Entry Condition | Reads | Writes | Stop Conditions |
|-------|-----------------|-------|--------|-----------------|
| `pid` | No active unshipped feature | `state.md`, `boundaries.md`, optional `FDG.md`, optional `intent.md` | `specs/[feature]-pid.md`, `state.md` | high-risk boundary, unresolved dependency, feature too large, large feature plan required |
| `contract` | phase = `pid`; PID Card exists | PID Card, optional PAD | `contracts/[feature].md`, `state.md` | missing PID Card, unresolved PAD ambiguity, contract too large to verify in one loop |
| `build` | phase = `contract`; contract exists and lints | contract, PID Card, constitution, optional PAD | code changes, `state.md` | unconfirmed component plan, boundary risk, contract violation |
| `verify` | phase = `build-complete`; contract exists and lints | contract, optional `intent.md` | `knowledge/[feature]-verify.md`, `state.md` | no real runtime evidence, `FAIL`, `INCONCLUSIVE` |
| `ship` | phase = `verify-pass`; verify record PASS or manual override | verify record, constitution | archived contract, updated `state.md` | test failure, missing manual acceptance, impact not handled |

---

## Guard Mapping

```bash
bash .ledger/bin/ledger.sh guard pid
bash .ledger/bin/ledger.sh guard contract
bash .ledger/bin/ledger.sh guard build
bash .ledger/bin/ledger.sh guard verify
bash .ledger/bin/ledger.sh guard ship
```

Equivalent direct script:

```bash
bash .ledger/bin/ledger-guard.sh <pid|contract|build|verify|ship>
```

---

## Check Mapping

Installed project self-check:

```bash
bash .ledger/bin/ledger.sh check --project
```

Framework repository self-check:

```bash
bash .ledger/bin/ledger.sh check
```

Contract lint:

```bash
bash .ledger/bin/ledger.sh lint-contract <file|--all|--fixtures>
```

Verify lint:

```bash
bash .ledger/bin/ledger.sh lint-verify <file|--all|--fixtures>
```

Agent entry lint:

```bash
bash .ledger/bin/ledger.sh lint-agents <file|--all|--fixtures>
```

State validation and controlled state updates:

```bash
bash .ledger/bin/ledger.sh state validate
bash .ledger/bin/ledger.sh state enqueue <feature>
bash .ledger/bin/ledger.sh state set-phase <phase>
bash .ledger/bin/ledger.sh state complete
bash .ledger/bin/ledger.sh state fail-verify
```

Stale state diagnostics:

```bash
bash .ledger/bin/ledger.sh check --stale
```

---

## State Rules

`.ledger/state.md` is the v1.x source of truth.

Agents may read `state.md` directly, but machine-critical state updates should use `ledger state` commands so phase transitions, duplicate queue/completed entries, unsafe feature names, and verify evidence requirements are checked before writing.

Structured state and queue files are draft-only in v1.x:

```text
.ledger/schemas/state.schema.json
.ledger/state.example.json
.ledger/schemas/queue.schema.json
.ledger/queue.example.json
```

Do not create `.ledger/state.json` or `.ledger/queue.json` as active truth sources unless a future incompatible protocol version explicitly enables them.

The current feature name must exactly match generated file paths:

```text
.ledger/specs/[feature]-pid.md
.ledger/contracts/[feature].md
.ledger/knowledge/[feature]-verify.md
```

Do not infer that a missing artifact is acceptable because it was discussed in the current chat. The file must exist when the phase requires it.

---

## Verification Rules

Verdict rules, evidence requirements, and speculative language prohibition are defined in constitution.md §9 (authoritative source). Verify records must contain exactly one strict verdict line.

## Feature Sizing Rules

Each feature should fit one complete `contract -> build -> verify -> ship` loop.

Criteria for "too large" are defined in constitution.md §12 (authoritative source). When sizing fails, split the feature or create an exec-plan before continuing.

## Reusable Patterns

Use `.ledger/knowledge/patterns.md` for durable cross-feature engineering knowledge.

Patterns should be promoted during `ship` or cleaned during `retro`. Do not use it as a story log, debug scratchpad, or replacement for `AGENTS.md`, `constitution.md`, or module handover files.
