# Architecture Spine

## 架构原则
- 业务写入由 owner 模块负责。

## 核心实体归属
| 实体 | Owner 模块 | 写入入口 | 备注 |
|------|------------|----------|------|
| order | orders | order service | 无 |

## 状态机归属
| 状态机 | Owner 模块 | 状态值 | 变更规则 |
|--------|------------|--------|----------|
| order lifecycle | orders | draft / created | 由 service 变更 |

## 权限判断位置
| 场景 | 判断层 | 规则来源 | 失败表现 |
|------|--------|----------|----------|
| 修改订单 | service | role | forbidden |

## 数据写入边界
- order 只能通过 orders service 写入。

## 依赖方向
- 允许：orders -> shared

## ADR 触发条件
- 新增顶级模块。
