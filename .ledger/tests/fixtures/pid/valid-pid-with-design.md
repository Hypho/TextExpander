# PID Card — create-order

## 谁在使用？
运营

## 他要做什么？
创建订单。

## 主流程映射
- PAD 业务主流程 Step：S1
- 功能类型：主流程
- 成功后用户去向：订单详情页

## 架构影响
- 涉及模块：orders
- 涉及实体：order
- 是否需要 ADR：否

## 设计附件判断
状态：需要 interaction
触发项：新页面
附件：
- .ledger/tests/fixtures/design/valid-interaction.md
人工决策：已确认

## 明确不做
- 不处理支付。
