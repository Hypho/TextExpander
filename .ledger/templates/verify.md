# Verify 记录 — [功能名称]
关联 Contract：[功能名].md | 日期：[日期]

## 对抗测试结果

| FC 条目 | 构造的边界输入 | 实际命令输出（前 30 行）| 是否成立 |
|--------|-------------|----------------------|---------|
| FC-01  | [边界输入描述] | [截取输出]           | ✓ / ✗  |
| FC-02  | [边界输入描述] | [截取输出]           | ✓ / ✗  |

> 超出 30 行的输出标注 "[...截断，完整输出见终端]"

## 测试报告

```
L1 结果：[X passed / Y failed]
L2 结果：[X passed / Y failed]

失败用例完整输出：
[无 / 粘贴失败用例的完整输出]
```

## 可复用经验候选

- [无 / 可沉淀到 knowledge/patterns.md 的跨功能模式或陷阱]

> 只记录跨功能仍然有效的工程模式。单个功能流水账、临时 debug 信息、已存在于 README / AGENTS.md / constitution.md 的内容不要写入 patterns。

## Verdict

> ⚠️ 此行格式固定，启动校验依赖精确匹配，不得修改键名或格式。

verdict = PASS

> 可选值：PASS / FAIL / INCONCLUSIVE
> FAIL 时补充：
>   failed_fc = [FC-XX, FC-XX]
>   reason = [在什么条件下失败，具体表现]
>   action = 回退 /ledger.build
>
> INCONCLUSIVE 时补充：
>   reason = [无法运行的具体原因]
>   action = 等待人工从 A/B/C 三个选项中选择（见 ledger.verify.md INCONCLUSIVE 处置协议）

---

<!-- 以下由人工填写，仅 INCONCLUSIVE 且选择 [B] 时追加 -->
## MANUAL OVERRIDE（如适用）

MANUAL OVERRIDE — [日期] — [签字人] — [确认理由]
