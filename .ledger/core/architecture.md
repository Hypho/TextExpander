# architecture.md — Architecture Spine 架构主干
> 冷层：按需读取，不在会话启动时强制加载。定义模块边界、实体归属、依赖方向和 ADR 触发条件。
> 最后更新：2026-05-13

## 架构原则
- 单一职责：每个模块只做一件事
- 核心引擎（键盘钩子）不依赖 UI
- 配置读写与业务逻辑分离
- 无网络依赖，纯本地运行

## 目录结构
```
TextExpander/
├── src/
│   ├── TextExpander/                 # 主项目
│   │   ├── Program.cs                # 入口
│   │   ├── Core/                     # 核心引擎
│   │   │   ├── KeyboardHook.cs       # 全局键盘钩子
│   │   │   ├── InputBuffer.cs        # 输入缓冲区
│   │   │   ├── AbbreviationMatcher.cs# 缩写词匹配
│   │   │   └── TextReplacer.cs       # 文本替换（含 SendInput）
│   │   ├── Config/                   # 配置管理
│   │   │   ├── AppConfig.cs          # 全局配置模型
│   │   │   ├── Rule.cs               # 规则模型
│   │   │   ├── ConfigManager.cs      # JSON 读写
│   │   │   └── VariableResolver.cs   # 动态变量解析
│   │   ├── UI/                       # 用户界面
│   │   │   ├── TrayIcon.cs           # 系统托盘
│   │   │   ├── RuleManagerForm.cs    # 规则管理 GUI
│   │   │   └── SettingsForm.cs       # 设置界面
│   │   └── Startup/                  # 启动相关
│   │       └── BootManager.cs        # 开机自启
│   └── TextExpander.Tests/           # 测试项目
│       ├── Core/
│       ├── Config/
│       └── Integration/
├── TextExpander.sln
└── .ledger/
```

## 模块边界
| 模块 | 路径 | 职责 | 不负责 |
|------|------|------|--------|
| Core | src/TextExpander/Core/ | 键盘监听、输入缓冲、匹配、替换 | 配置读写、UI 展示 |
| Config | src/TextExpander/Config/ | 规则/配置的模型和持久化、变量解析 | 键盘钩子、文本发送 |
| UI | src/TextExpander/UI/ | 托盘图标、规则管理窗口 | 业务逻辑 |
| Startup | src/TextExpander/Startup/ | 开机自启注册/注销 | 其他配置 |

## 核心实体归属
| 实体 | Owner 模块 | 写入入口 | 备注 |
|------|------------|----------|------|
| Rule | Config | ConfigManager | JSON 文件持久化 |
| AppConfig | Config | ConfigManager | JSON 文件持久化 |
| InputBuffer | Core | KeyboardHook 回调 | 内存环形缓冲区，不持久化 |

## 状态机归属
| 状态机 | Owner 模块 | 状态值 | 变更规则 |
|--------|------------|--------|----------|
| HookState | Core | Active / Paused | 托盘菜单切换 |
| AppLifecycle | Program | Starting / Running / Exiting | 启动/退出生命周期 |

## 权限判断位置
| 场景 | 判断层 | 规则来源 | 失败表现 |
|------|--------|----------|----------|
| 缩写词是否启用 | AbbreviationMatcher | Rule.enabled | 跳过该规则 |

## 数据写入边界
- Rule 只能通过 ConfigManager 写入 JSON 文件
- InputBuffer 只能通过 KeyboardHook 回调更新
- AppConfig 只能通过 ConfigManager / SettingsForm 写入

## 依赖方向
- 允许：UI → Config → Core
- 禁止：Core → UI, Core → Config（Core 通过接口/委托解耦）
- 共享逻辑放置：Config 模块（Rule/AppConfig 模型被所有层引用）

## 外部依赖
| 库名 | 用途 | 版本 | 引入日期 |
|------|------|------|---------|
| System.Text.Json | JSON 序列化 | 内置（.NET 8） | 2026-05-13 |
| （无第三方依赖） | | | |

## ADR 触发条件
命中以下任一情况时，在 `.ledger/knowledge/decisions/` 记录架构决策：
- 新增顶级模块
- 新增关键外部依赖
- 改变核心实体关系
- 改变状态机
- 引入异步任务或外部集成
- 同一问题存在两个以上合理技术方案

## 架构决策
| 日期 | 决策 | 原因 |
|------|------|------|
| 2026-05-13 | 使用 WinForms 而非 WPF | 小工具场景，WinForms 更轻量，代码量少 |
| 2026-05-13 | JSON 而非 YAML/INI | .NET 原生支持，无需额外 NuGet 包 |
| 2026-05-13 | LowLevelKeyboardHook | Windows 原生 API，C# P/Invoke 调用最成熟 |
