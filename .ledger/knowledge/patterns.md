# 可复用工程模式
> 只记录跨功能、跨会话仍然有效的工程模式。最后更新：[日期]

## 稳定模式

| 范围 | 模式 / 约束 | 来源功能 | 最近验证 |
|------|-------------|----------|----------|
| Core/Win32 | Win32 API 调用通过接口（如 ITextSender）抽象，实现类（WinTextSender）负责 P/Invoke，测试中用 Fake 替代 | keyboard-hook-engine | 2026-05-13 |
| Core/回调 | 键盘钩子回调通过委托注入（Action<char>, Func<int,bool>），不直接依赖具体业务逻辑，便于测试和复用 | keyboard-hook-engine | 2026-05-13 |

## 常见陷阱

| 范围 | 陷阱 | 规避方式 | 来源功能 |
|------|------|----------|----------|
| Config/JSON | System.Text.Json 默认 PascalCase，但配置文件约定 camelCase | 使用 JsonNamingPolicy.CamelCase 配置序列化选项 | keyboard-hook-engine |
| Core/Win32 | 键盘钩子回调中执行 I/O 或耗时操作会导致 Windows 自动移除钩子（300ms 超时） | 回调中只做内存操作（缓冲区追加、匹配），替换通过 SendInput 异步发送 | keyboard-hook-engine |

## 记录规则

- 只记录可复用模式、模块约定、非显然依赖、测试方式和常见陷阱。
- 不记录单个功能流水账、临时 debug 信息或主观偏好。
- 不重复记录 README / AGENTS.md / constitution.md 已有内容。
- 每次 `/ledger.retro` 清理过时、重复或未验证的条目。
