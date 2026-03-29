#!/bin/bash
# verify-protocol.sh - Verify protocol constants haven't changed
# This ensures binary protocol compatibility

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

echo "=== Protocol Constants Verification ==="
echo ""

ERRORS=0

# Expected values
EXPECTED_VERSION_0_HEAD_LENGTH=18
EXPECTED_VERSION_1_HEAD_LENGTH=21
EXPECTED_CODEC_PROTOBUF=0
EXPECTED_CODEC_MSGPACK=1
EXPECTED_CODEC_JSON=2

# Find protocol constant definitions
VERSION_0_HEAD_LENGTH=$(grep -r "VERSION_0_HEAD_LENGTH" "$PROJECT_ROOT" --include="*.cs" | grep -E "const.*VERSION_0_HEAD_LENGTH.*=" | head -1 || true)
VERSION_1_HEAD_LENGTH=$(grep -r "VERSION_1_HEAD_LENGTH" "$PROJECT_ROOT" --include="*.cs" | grep -E "const.*VERSION_1_HEAD_LENGTH.*=" | head -1 || true)
CODECTYPE=$(grep -r "enum.*CodecType" "$PROJECT_ROOT" --include="*.cs" -A 5 | head -20 || true)

# Verify VERSION_0_HEAD_LENGTH
echo "Checking VERSION_0_HEAD_LENGTH..."
if echo "$VERSION_0_HEAD_LENGTH" | grep -q "$EXPECTED_VERSION_0_HEAD_LENGTH"; then
    echo "  ✓ VERSION_0_HEAD_LENGTH = $EXPECTED_VERSION_0_HEAD_LENGTH"
else
    echo "  ✗ VERSION_0_HEAD_LENGTH mismatch (expected $EXPECTED_VERSION_0_HEAD_LENGTH)"
    ERRORS=$((ERRORS + 1))
fi

# Verify VERSION_1_HEAD_LENGTH
echo "Checking VERSION_1_HEAD_LENGTH..."
if echo "$VERSION_1_HEAD_LENGTH" | grep -q "$EXPECTED_VERSION_1_HEAD_LENGTH"; then
    echo "  ✓ VERSION_1_HEAD_LENGTH = $EXPECTED_VERSION_1_HEAD_LENGTH"
else
    echo "  ✗ VERSION_1_HEAD_LENGTH mismatch (expected $EXPECTED_VERSION_1_HEAD_LENGTH)"
    ERRORS=$((ERRORS + 1))
fi

# Verify CodecType enum values
echo "Checking CodecType enum..."

# Check each codec type individually
CODEC_CHECK=$(grep -r "Protobuf.*=" "$PROJECT_ROOT/src/DotBPE.Protocol/Codec/CodecType.cs" 2>/dev/null | head -1 || true)
if echo "$CODEC_CHECK" | grep -q "= *0"; then
    echo "  ✓ CodecType.Protobuf = $EXPECTED_CODEC_PROTOBUF"
else
    echo "  ✗ CodecType.Protobuf mismatch (expected $EXPECTED_CODEC_PROTOBUF)"
    ERRORS=$((ERRORS + 1))
fi

CODEC_CHECK=$(grep -r "MessagePack.*=" "$PROJECT_ROOT/src/DotBPE.Protocol/Codec/CodecType.cs" 2>/dev/null | head -1 || true)
if echo "$CODEC_CHECK" | grep -q "= *1"; then
    echo "  ✓ CodecType.MessagePack = $EXPECTED_CODEC_MSGPACK"
else
    echo "  ✗ CodecType.MessagePack mismatch (expected $EXPECTED_CODEC_MSGPACK)"
    ERRORS=$((ERRORS + 1))
fi

CODEC_CHECK=$(grep -r "JSON.*=" "$PROJECT_ROOT/src/DotBPE.Protocol/Codec/CodecType.cs" 2>/dev/null | head -1 || true)
if echo "$CODEC_CHECK" | grep -q "= *2"; then
    echo "  ✓ CodecType.JSON = $EXPECTED_CODEC_JSON"
else
    echo "  ✗ CodecType.JSON mismatch (expected $EXPECTED_CODEC_JSON)"
    ERRORS=$((ERRORS + 1))
fi

echo ""
if [ $ERRORS -eq 0 ]; then
    echo "=== All protocol constants verified successfully ==="
    exit 0
else
    echo "=== Verification failed with $ERRORS error(s) ==="
    exit 1
fi
