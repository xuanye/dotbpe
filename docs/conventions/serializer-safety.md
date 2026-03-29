# Serializer Safety

This document defines the rules for maintaining serializer behavior stability in the dotbpe repository.

## Rationale

The serializer handles conversion between .NET objects and binary representations. Changes to serialization behavior can break existing clients and servers. Users depend on consistent serialization across versions.

## Current Serializer Configuration

The current supported codec types (CodecType enum):
- **0**: Protobuf (default)
- **1**: MessagePack
- **2**: JSON

## Enforceable Rules

### 1. Default Serializer Behavior

- You MUST NOT change the default serializer behavior without explicit approval.
- You MUST NOT change the default CodecType value without explicit approval.
- You MUST NOT silently change serialization contract expectations.

### 2. Codec Type Values

- You MUST NOT change the numeric values of existing CodecType enumerations.
- You MUST NOT remove or reorder existing codec type values.
- You MAY add new codec types as additive changes.

### 3. Serialization Contract Stability

- You MUST NOT change the serialized field order of existing DTOs without explicit approval.
- You MUST NOT change the serialized field names of existing DTOs without explicit approval.
- You MUST NOT change the field type of existing DTOs without explicit approval.
- You MUST NOT add required fields to existing DTOs without explicit approval.

### 4. Serializer Implementation

- You MUST NOT change the behavior of existing serializer implementations.
- You MUST ensure new serializer implementations produce byte-for-byte identical output for equivalent inputs.
- You SHOULD add serializer compatibility tests when implementing new serializers.

### 5. Breaking Changes

- You MUST document serialization breaking changes in `docs/migration/`.
- You MUST provide migration paths for serialization breaking changes.
- You MUST NOT make silent breaking changes to serialization behavior.

## Current Enforcement

These rules are currently enforced through:
- Code review requiring explicit justification for serializer changes
- No automated serializer validation is currently integrated
- Developers must manually verify serializer compatibility

## Related Documents

- [Protocol Safety](./protocol-safety.md)
- [Testing Policy](./testing-policy.md)