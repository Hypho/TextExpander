# templates/ — 空白参照模板

本目录存放**空白模板文件**，供 PACT 命令生成实例文件时作为结构参照。

开发者不应直接填写此目录下的文件。项目实际文件生成至：
- `.ledger/specs/`（PAD、FDG、PID Card）
- `.ledger/contracts/`（行为契约）
- `.ledger/knowledge/`（verify 记录）

## 模板清单

| 文件 | 用途 | 实例生成命令 |
|------|------|------------|
| `PAD.md` | 产品结构文档结构参照 | /ledger.init |
| `FDG.md` | 功能依赖图结构参照 | /ledger.scope |
| `IFD.md` | 交互流设计参照 | /ledger.pid（前端功能） |
| `pid-card.md` | PID Card 结构参照 | /ledger.pid |
| `contract.md` | 行为契约结构参照 | /ledger.contract |
| `verify.md` | Verify 记录结构参照 | /ledger.verify |
| `exec-plan.md` | 执行计划结构参照 | 大功能门控触发 |
| `handover.md` | 交接文档结构参照 | 按需 |
| `module-AGENTS.md` | 模块级 agent 入口结构参照 | 按需复制到子模块 |
