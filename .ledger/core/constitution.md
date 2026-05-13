# constitution.md — 项目宪法
> 温层：scope / build / verify / ship 时读取。只放不变的硬约束。
> 最后更新：2026-05-13

---

## 1. 产品概要

```
名称：TextExpander
描述：Windows 轻量文本扩展工具，输入缩写词+终止键自动替换为预设文本
阶段：原型验证
```

---

## 2. 技术栈

```
项目类型：Windows 桌面应用
前端/界面：WinForms（.NET 8）
后端/核心：C# + Win32 API（P/Invoke user32.dll）
数据库：无（JSON 配置文件）
发布方式：dotnet publish self-contained single-file
```

---

## 3. 测试命令

```
L1（单元，~10s）：dotnet test
L2（冒烟，~30s）：dotnet test --filter Category=Smoke
L3（完整，~60s+）：手动集成测试（键盘钩子 + 系统托盘 + GUI 联动）
```

---

## 4. 代码约束

- 删除策略：硬删除（配置文件直接覆盖）
- 命名规范：C# 标准（PascalCase 类名/方法名，camelCase 局部变量，文件名匹配类名）
- 错误处理：托盘气泡通知用户，不暴露技术细节
- 新增依赖：必须在 architecture.md 登记
- 配置文件格式：UTF-8 JSON

---

## 5. 严禁事项

- 不允许在 build 阶段未完成测试前输出收尾语言
- 禁止在 verify.md 中使用推理性语言替代真实运行结果
- 不允许引入重量级依赖（NuGet 包不超过 3 个）
- 不允许修改 JSON 配置格式而不更新 schema 文档
- 禁止在键盘钩子回调中执行耗时操作（> 50ms）

---

## 6. 文件命名规范（硬约束）

> 所有路径匹配依赖此规范。Claude 和开发者必须严格遵守，不得自行发挥。

**功能名 → 文件名转换规则：**
```
英文功能名：全小写 kebab-case
  例：Keyboard Hook Engine → keyboard-hook-engine
  例：Rule Manager GUI → rule-manager-gui
```

**生成路径（强制，不得偏离）：**
```
contract 文件   → .ledger/contracts/[功能名].md
verify 记录     → .ledger/knowledge/[功能名]-verify.md
exec-plan 文件  → .ledger/exec-plans/active/[功能名]-plan.md
pid-card 文件   → .ledger/specs/[功能名]-pid.md
```

> state.md 中"功能"字段的值必须与以上路径中的 [功能名] 完全一致。
> 路径不一致时，启动校验触发告警，流程停止。

---

## 7. 命令钩子协议

> Claude Code 提供 Hook 系统（PreToolUse / PostToolUse / SessionStart 等生命周期事件），
> 可在 shell 层面真正阻断流程，彻底解决"prompt 层无法强制执行约束"的问题。
>
> **硬检查点定义在各命令文件开头的「入口检查」小节，本节只负责 Hook 脚本的配置与注册，提供第二层兜底。**

---

### Hook 配置指南

本仓库提供检查脚本。是否注册为 Claude Code SessionStart Hook，由项目维护者按环境选择。

如需启用自动兜底，在项目根目录创建 `.claude/settings.json` 并注册：

**`.ledger/hooks/check-state.sh`**（SessionStart 时运行，校验 state.md 一致性）

完整脚本见仓库 `.ledger/hooks/check-state.sh`。关键实现要点：

- 解析前先归一化：全角冒号 `：` → 半角 `:`、剥除 Markdown 加粗 `**`，容忍模型的轻微格式改动
- 使用 `awk` 而非 `grep + sed` 链式处理，避免复杂正则在不同 sed 实现下的行为差异
- 阶段字段限定为 `[A-Za-z-]+`，即使周围有额外文本也能稳定提取

**`.claude/settings.json`** 中注册：
```json
{
  "hooks": {
    "SessionStart": [
      {
        "hooks": [{ "type": "command", "command": "bash .ledger/hooks/check-state.sh" }]
      }
    ]
  }
}
```

> Hook 脚本以非零退出码阻断会话继续；零退出码表示检查通过。
> 脚本需要 `chmod +x` 可执行权限。
> 未注册 settings 时，`check-state.sh` 仍可由 `ledger-check.sh` 和人工命令调用，但不会自动拦截会话启动。

---

### 软建议（工作习惯，不强制）

```
任意命令完成后：更新 state.md 阶段字段
任意 Step 失败时：将失败摘要写入 .ledger/knowledge/errors/[命令]-[YYYYMMDD].md
                   state.md 回退至上一阶段
```

---

## 8. Build 完成条件（硬约束）

```
/ledger.build 声明完成的唯一条件：
  1. 当前功能 contract 所有 FC 关键路径条目均已实现且可验证
  2. L1 或 L2 测试通过
  3. state.md 已更新至 build-complete

未满足以上全部条件，禁止输出"完成""已实现"
"基本框架就绪""可在此基础上扩展"等语言。
```

---

## 9. Verify Verdict 规则

```
PASS         — 所有 FC 在真实运行下成立，进入 /ledger.ship
FAIL         — 列出被打破的 FC 条目和触发条件，回退 /ledger.build
INCONCLUSIVE — 无法在当前环境实际运行（见 ledger.verify.md 中 INCONCLUSIVE 处置协议）

进入 /ledger.ship 的合法凭证：
  verdict = PASS，或
  verify.md 末尾包含有效的 MANUAL OVERRIDE 记录

禁止使用"应该""预期""理论上"等推理语言替代真实输出。
```

---

## 10. State 源规则

```
v1.x 阶段：
  .ledger/state.md 是状态真相源。
  .ledger/schemas/state.schema.json 仅作为未来结构化迁移草案。
  .ledger/state.example.json 仅作为 state schema 示例，不参与运行时读取。
  ledger-check.sh 必须检查 state.md 的基础字段和阶段枚举。
  check-state.sh 必须通过 fixture 覆盖常见非法状态。

未来 v2.0 候选：
  只有当 state.md 解析脆弱性形成真实维护成本时，才迁移到 state.json。
  迁移后 state.md 应由结构化状态渲染生成，不再手动维护。
```

---

## 11. Contract / Verify Lint 规则

```
/ledger.contract 产物必须通过 contract lint：
  1. 文件存在且非空
  2. 至少包含一个 FC 条目（FC-01 / FC-1）
  3. 包含"明确不做"或 "Out of Scope"
  4. 不保留明显模板占位符（[请补充] / [功能名] / [待填写] / TODO）
  5. FC 条目数不得超过默认阈值 7；超过时必须拆分功能或生成 exec-plan

/ledger.verify 产物必须通过 verify lint：
  1. 文件存在且非空
  2. verdict 行严格为 verdict = PASS / FAIL / INCONCLUSIVE
  3. verdict 只能出现一次
  4. 禁止使用"应该 / 预期 / 理论上 / should / expected / theoretically"
  5. verdict = PASS 时，必须包含运行证据标记（output / 输出 / command / 命令 / result / 结果）
```

---

## 12. 功能粒度与可复用经验规则

```
一个功能应能完成一次完整的 contract -> build -> verify -> ship 闭环。

命中以下任一信号时，必须拆分功能或生成 exec-plan：
  1. 跨 3+ 模块
  2. 涉及 Schema 变更且同时包含多类实现工作
  3. 预计需要 2+ 个会话
  4. 依赖 3+ 个尚未完成的功能
  5. 无法用 2-3 句话描述单一用户价值
  6. 无法独立验证核心 FC 条目

.ledger/knowledge/patterns.md 只记录跨功能、跨会话仍有效的工程模式：
  - 模块约定
  - 非显然依赖
  - 测试方式
  - 常见陷阱
```

---

## 13. Agent Entry Lint 规则

```
根目录 AGENTS.md 是跨工具 agent 入口。
CLAUDE.md 是 Claude Code 运行入口，不应与 AGENTS.md 逐字同步。

ledger-lint-agents.sh 检查：
  1. AGENTS.md 存在且非空
  2. 文件行数不超过默认阈值 150 行
  3. 引用 .ledger/core/workflow.md
  4. 包含 guard 命令引用
  5. 包含 check --project
  6. Markdown 引用数量不超过默认阈值 10
  7. 多条禁止项必须配 Don't / Do 对照
```

---

## 14. Command Guard 规则

```
ledger-guard.sh 只判断命令是否允许进入，不执行命令，不写文件，不修改 state。

支持入口：
  ledger-guard.sh pid
  ledger-guard.sh contract
  ledger-guard.sh build
  ledger-guard.sh verify
  ledger-guard.sh ship

进入 /ledger.contract 前必须满足：
  当前阶段 = pid
  .ledger/specs/[功能名]-pid.md 存在

进入 /ledger.build 前必须满足：
  当前阶段 = contract
  .ledger/contracts/[功能名].md 存在
  contract lint 通过

进入 /ledger.verify 前必须满足：
  当前阶段 = build-complete
  .ledger/contracts/[功能名].md 存在
  contract lint 通过

进入 /ledger.ship 前必须满足：
  当前阶段 = verify-pass
  .ledger/knowledge/[功能名]-verify.md 存在
  verify lint 通过
  verify 文件包含 verdict = PASS 或 MANUAL OVERRIDE
```

---

## 15. 外部 Skills（可选增强）

> 外部 Skills 不是 PACT 主流程的必需依赖。
> 到达挂载点时，仅在开发者明确要求或项目已安装时触发。

| Skill | 挂载点 | 触发时机 | 安装命令 | 状态 |
|-------|--------|---------|---------|------|
| code-simplifier | /ledger.retro | 每轮回顾 | `claude plugin install code-simplifier` | [x] |

---

## 16. 归档触发规则

```
已完成功能表超过 10 条时：
  将最早的 5 条移至 .ledger/knowledge/archive/completed.md
  本表只保留最近 5 条 + 首行标注总计数

state.md 已完成队列超过 10 条时：
  将最早的 5 条移至 .ledger/knowledge/archive/state-history.md
  state.md 只保留最近 5 条
```

---

## 17. 开源发布规则（硬约束）

```
VERSION 是唯一可编辑版本真相源。
PACT 默认自检必须保持 file-only，不依赖 git / GitHub / 网络。

版本判断：
  PATCH：不改变协议契约的修整
  MINOR：向后兼容的能力增强
  MAJOR：不兼容协议或状态机变更

发布前检查：
  1. VERSION 存在，且格式为 MAJOR.MINOR.PATCH
  2. README.md、CLAUDE.md 版本号与 VERSION 一致
  3. 本地 *.local.md 和未公开路线草案未进入提交
  4. 若项目采用 git，运行可选 git-aware 发布检查
  5. 若发布到 GitHub，维护者已明确确认 tag / Release 动作
```

---

## 已完成功能

> 总计：0 个功能

| 功能 | 开始 | 完成 | 契约 | 测试 | Verify |
|------|------|------|------|------|--------|
| （暂无） | | | | | |
