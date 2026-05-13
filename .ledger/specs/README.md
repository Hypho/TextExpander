# specs/ — 项目实例文件

本目录存放由 PACT 命令**生成的项目特定文件**，区别于 templates/（空白参照模板）。

## 文件说明

| 文件 | 生成时机 | 说明 |
|------|---------|------|
| `PAD.md` | /ledger.init Step 3 | 产品结构文档，定义核心实体和全局规则 |
| `FDG.md` | /ledger.scope 可选步骤 | 功能依赖图，开发者明确选择时生成的可选规划附件 |
| `[功能名]-pid.md` | /ledger.pid | 各功能的 PID Card 实例 |

## templates/ vs specs/ 的区别

```
templates/   空白模板，只用于参照结构，不填写项目具体内容
specs/       项目实例，由命令生成初稿，开发者确认后作为项目事实
```

> specs/ 中的文件应纳入版本控制（不在 .gitignore 中排除）。
