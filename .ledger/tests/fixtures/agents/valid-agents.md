# Agent Instructions

Use the PACT feature flow:

```text
pid -> contract -> build -> verify -> ship
```

Read `.ledger/core/workflow.md` for the full workflow.

## Decision Table

| Current state | Next action |
|---|---|
| no active feature | Run `bash .ledger/bin/ledger.sh guard pid` and create a PID Card. |
| phase = contract | Run `bash .ledger/bin/ledger.sh guard build` before implementation. |

## Do / Don't

| Don't | Do |
|---|---|
| Do not skip verification. | Run tests and write real output. |
| Do not infer missing files from chat. | Check the expected PACT file path. |

Run this after PACT file changes:

```bash
bash .ledger/bin/ledger.sh check --project
```

