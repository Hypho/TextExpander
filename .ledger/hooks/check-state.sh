#!/bin/bash
# PACT SessionStart Hook — 校验 state.md 与文件系统一致性
# 配置方式见 .ledger/core/constitution.md § 7 Hook 配置指南

ROOT="${PACT_ROOT:-.}"
STATE="${PACT_STATE_FILE:-$ROOT/.ledger/state.md}"
[ ! -f "$STATE" ] && exit 0

# 归一化：全角冒号→半角、移除 Markdown 加粗标记 ** 和行首 # 标题前缀
# 目的：容忍模型将 "功能：X" 改写为 "功能:X" / "**功能**：X" / "## 功能：X"
STATE_NORM=$(sed -e 's/：/:/g' -e 's/\*\*//g' "$STATE")

# 提取功能名：定位含"功能:"的行，剥除前缀，截断到首个 | 或行尾
FEATURE=$(echo "$STATE_NORM" | awk '
  /功能[[:space:]]*:/ {
    sub(/^.*功能[[:space:]]*:[[:space:]]*/, "")
    sub(/[[:space:]]*\|.*$/, "")
    sub(/[[:space:]]+$/, "")
    print
    exit
  }
')

# 提取阶段：定位含"阶段:"的行，仅取合法阶段字符（字母和连字符）
PHASE=$(echo "$STATE_NORM" | awk '
  /阶段[[:space:]]*:/ {
    sub(/^.*阶段[[:space:]]*:[[:space:]]*/, "")
    if (match($0, /[A-Za-z-]+/)) {
      print substr($0, RSTART, RLENGTH)
    }
    exit
  }
')

# 无活跃功能或仍是模板占位符，跳过检查
[ -z "$FEATURE" ] || [ "$FEATURE" = "[名称]" ] && exit 0

# pid 阶段：pid-card 文件必须存在
if [[ "$PHASE" == "pid" ]]; then
  if [ ! -f "$ROOT/.ledger/specs/${FEATURE}-pid.md" ]; then
    echo "❌ PACT 状态不一致：state.md 声明 pid，但 specs/${FEATURE}-pid.md 不存在"
    echo "   请检查文件是否被误删，或手动修正 state.md 中的阶段字段"
    exit 1
  fi
fi

# contract / build / build-complete 阶段：contract 文件必须存在
if [[ "$PHASE" == "contract" || "$PHASE" == "build" || "$PHASE" == "build-complete" ]]; then
  if [ ! -f "$ROOT/.ledger/contracts/${FEATURE}.md" ]; then
    echo "❌ PACT 状态不一致：state.md 声明 ${PHASE}，但 contracts/${FEATURE}.md 不存在"
    echo "   请检查文件是否被误删，或手动修正 state.md 中的阶段字段"
    exit 1
  fi
fi

# verify-pass 阶段：verify 文件必须存在且含合法凭证
if [[ "$PHASE" == "verify-pass" ]]; then
  VERIFY="$ROOT/.ledger/knowledge/${FEATURE}-verify.md"
  if [ ! -f "$VERIFY" ]; then
    echo "❌ PACT 状态不一致：state.md 声明 verify-pass，但 ${VERIFY} 不存在"
    exit 1
  fi
  if ! grep -q "verdict = PASS\|MANUAL OVERRIDE" "$VERIFY"; then
    echo "❌ PACT 状态不一致：${VERIFY} 存在但不含 verdict = PASS 或 MANUAL OVERRIDE"
    exit 1
  fi
fi

exit 0
