# Module Agent Instructions — [module]

Use this template for module-level `AGENTS.md` files such as `src/billing/AGENTS.md` or `src/auth/AGENTS.md`.
Keep the file short and specific to the surrounding module.

## Module Boundary

This module owns:
- [responsibility]

This module does not own:
- [out-of-scope responsibility]

## Decision Table

| If the task involves | Use |
|----------------------|-----|
| [local pattern or workflow] | [specific file, helper, or command] |
| [cross-module concern] | Stop and check [owner/reference] |

## Local Patterns

| Need | Use |
|------|-----|
| [operation] | [existing helper/API] |
| [data access] | [existing repository/service] |

## Don't / Do

| Don't | Do |
|-------|----|
| Do not [common wrong local pattern]. | Use [correct local pattern]. |
| Do not change [external module]. | Update [owned file] or ask for a boundary decision. |

## References

- [one local reference file]
- [optional second local reference file]

