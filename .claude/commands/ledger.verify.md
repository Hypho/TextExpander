# /ledger.verify — 对抗性验证

加载 ledger-verify skill 执行对抗性验证（含子 Agent 编排）。

## 执行

1. 运行 guard：`bash .ledger/bin/ledger.sh guard verify`
2. 加载 skill：使用 Skill 工具加载 `ledger:ledger-verify`
3. 按 skill 指引执行

guard 失败则停止，不继续。
