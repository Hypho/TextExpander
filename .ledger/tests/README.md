# tests/ — 测试文件目录

存放 PACT 框架管理的测试脚本和测试数据。与项目自身的测试目录分离，专门存放行为级测试。

---

## 目录说明

### features/
**对应测试层级：L2（冒烟）/ L3（完整）**

每个功能一个测试文件，命名与功能名一致：
```
features/[功能名].[扩展名]
例：features/用户登录.test.ts
    features/create-post.spec.py
```

`/ledger.ship` 阶段会扫描此目录，检查未完成的 TODO 项。
`/ledger.verify` 阶段运行 L2 时执行此目录下的冒烟测试。

### fixtures/
**对应测试层级：L1 / L2 / L3 共用**

测试数据文件，按功能、实体或 PACT 自检类型分组：
```
fixtures/[功能名或实体名].[扩展名]
例：fixtures/user.json
    fixtures/order-seed.sql
    fixtures/agents/valid-agents.md
```

### api/
**对应测试层级：L2（冒烟 API）/ L3（完整 API 回归）**

API 层面的测试脚本（如 HTTP 请求集合、集成测试）：
```
api/[功能名或模块名].[扩展名]
例：api/auth.http
    api/order-flow.test.ts
```

### reports/
自动生成，不手动编辑。存放测试运行报告（.gitignore 已排除版本控制）。

---

## 与测试层级的对应关系

| 层级 | 说明 | 主要来源 |
|------|------|---------|
| L1（单元，~10s） | 纯函数/模块级，运行不依赖 dev server | 项目自身测试目录 |
| L2（冒烟，~30s） | 核心路径验证，需要 dev server | `features/` + `api/` |
| L3（完整，~60s+）| 全功能回归，含已完成功能 | `features/` + `api/` 全部 |
