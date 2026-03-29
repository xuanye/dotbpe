# Agent Scripts

This directory contains utility scripts for dotbpe agents.

## Conventions

- All scripts should be idempotent and safe to run multiple times
- Scripts must NOT modify source code
- Scripts must NOT have side effects on the codebase
- Use `set -e` for exit-on-error behavior
- Prefix output with `echo` for clarity

## Scripts

### verify-protocol.sh

Verifies that protocol constants remain unchanged. This ensures binary protocol compatibility.

**Constants verified:**
- `VERSION_0_HEAD_LENGTH = 18`
- `VERSION_1_HEAD_LENGTH = 21`
- `CodecType`: Protobuf=0, MessagePack=1, JSON=2

**Usage:**
```bash
./scripts/agent/verify-protocol.sh
```

**Exit codes:**
- 0: All constants verified
- 1: Verification failed
