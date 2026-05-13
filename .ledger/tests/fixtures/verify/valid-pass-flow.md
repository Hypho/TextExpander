# Verify: create-order

command: node test/create-order.test.js
output: 3 passed
flow-required: yes
flow-step: S1
user-path: create form -> submit -> detail
状态变化: order draft -> created
成功后去向: order detail
verdict = PASS
