# /ledger.pid — 功能意图定义

加载 ledger-pid skill 执行功能意图定义和边界检测。

## 执行

1. 运行 guard：`bash .ledger/bin/ledger.sh guard pid`
2. 加载 skill：使用 Skill 工具加载 `ledger:ledger-pid`
3. 按 skill 指引执行

guard 失败则停止，不继续。
