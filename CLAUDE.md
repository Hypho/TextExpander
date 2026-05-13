# TextExpander — Ledger 工作空间
> Ledger — Auditable AI-Assisted Development Protocol v0.1.0

---

## 0. Skills 系统

PACT 2.x 引入 skills 系统。每个功能阶段有对应的 skill，slash commands 是 skill 的快捷入口。

**SessionStart hook** 会在新会话时自动检测 `.ledger/` 并注入 `using-ledger` skill。Agent 看到后应：
1. 读取 state.md 识别当前 phase
2. 根据 phase 加载对应 skill（使用 Skill 工具）
3. 进入阶段前运行 guard

可用 skills：`using-ledger` / `ledger-scope` / `ledger-pid` / `ledger-contract` / `ledger-build` / `ledger-verify` / `ledger-ship` / `ledger-retro`

---

## 1. 启动序列

> 入口职责：`AGENTS.md` 是跨工具 agent 入口；`CLAUDE.md` 是 Claude Code 热层入口。
> 若二者在流程事实上不一致，以 `.ledger/core/workflow.md` 为准。
> 若二者在硬约束上不一致，以 `.ledger/core/constitution.md` 为准。

每次新会话，按以下顺序执行，不跳过：

```
Step 0  初始化检测：
          读取 constitution.md 产品名称字段
          若名称字段 = "[由 /ledger.init 填写]"：
            输出 "⚠️ 项目尚未初始化，请先执行 /ledger.init"
            停止，不执行后续步骤，不响应功能命令

Step 1  读取 state.md，获取当前功能名和阶段

Step 2  文件存在性校验（根据阶段）：
          阶段 = pid                      → 检查 .ledger/specs/[功能名]-pid.md 是否存在
          阶段 = contract / build / build-complete → 检查 .ledger/contracts/[功能名].md 是否存在
          阶段 = verify-pass              → 检查 .ledger/knowledge/[功能名]-verify.md 是否存在
                                            且内容包含 "verdict = PASS" 或 "MANUAL OVERRIDE"
        校验失败：输出 "⚠️ 状态不一致：state.md 声明 [阶段] 但对应文件缺失或不匹配"
                  等待人工修正，不继续执行任何命令

Step 3  校验通过后输出：当前任务理解（一句话）+ 发现的约束冲突风险

Step 4  等待人工确认后再执行
```

> **⚠️ Session Memory 说明**
> Claude Code 在后台维护自身的 session_memory（持久化会话摘要）。
> 当 session_memory 与 state.md 内容冲突时，**以 state.md 为准**。
> state.md 是 PACT 的唯一状态事实来源，任何时候有疑问都重新读取该文件。

---

## 2. 执行模式

```
每个功能：/ledger.pid → /ledger.contract → /ledger.build → /ledger.verify → /ledger.ship
每 3-5 个功能：/ledger.retro
```

> 流程定义源见 `.ledger/core/workflow.md`。本文件只保留热层摘要，避免多处定义漂移。

---

## 3. 命令清单

| 命令 | 触发时机 | 加载 Skill |
|------|---------|-----------|
| `/ledger.init` | 项目开始（一次性） | 无（直接执行） |
| `/ledger.scope` | 首次功能前建议执行 | `ledger:ledger-scope` |
| `/ledger.pid` | 每个功能开始 | `ledger:ledger-pid` |
| `/ledger.contract` | pid 完成后 | `ledger:ledger-contract` |
| `/ledger.build` | contract 完成后 | `ledger:ledger-build` |
| `/ledger.verify` | build 完成后 | `ledger:ledger-verify` |
| `/ledger.ship` | verify PASS 后 | `ledger:ledger-ship` |
| `/ledger.retro` | 每 3-5 个功能 | `ledger:ledger-retro` |

---

## 4. 文件读取装配规则

> 控制 context 加载边界。非列表内的文件不主动读取。

**常驻层**（每次会话启动必读）
```
CLAUDE.md              — 当前文件
.ledger/state.md         — 当前进度与阻塞
```

**命令触发层**（进入对应命令时读取）
```
/ledger.pid       → .ledger/scope/boundaries.md
                  .ledger/specs/PAD.md
                  .ledger/core/architecture.md
/ledger.contract  → .ledger/templates/contract.md
                  .ledger/specs/[当前功能]-pid.md
                  .ledger/specs/PAD.md
/ledger.build     → .ledger/core/constitution.md
                  .ledger/core/architecture.md
                  .ledger/contracts/[当前功能].md
                  .ledger/specs/[当前功能]-pid.md
/ledger.verify    → .ledger/contracts/[当前功能].md
/ledger.ship      → .ledger/core/constitution.md
```

---

## 5. 硬约束

> 完整约束见 `.ledger/core/constitution.md`

- 不允许引入重量级依赖（NuGet 包不超过 3 个）
- 禁止在键盘钩子回调中执行耗时操作（> 50ms）
- 不允许修改 JSON 配置格式而不更新 schema 文档

---

## 6. 框架边界（不覆盖）

- 键盘钩子被安全软件拦截
- 远程桌面/游戏不兼容
- 中文输入法下 SendInput 行为异常
