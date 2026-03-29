# Public API Boundary

This document defines the rules for public API changes in the dotbpe repository. These rules ensure backward compatibility and stable contracts for library consumers.

## Rationale

dotbpe is a framework/library repository. Users depend on public APIs for their own applications. Breaking changes must be intentional, rare, and clearly communicated.

## Enforceable Rules

### 1. Naming and Signature Stability

- You MUST NOT rename public methods, properties, events, or types without explicitly flagging the breaking change.
- You MUST NOT change the signature of public methods (parameter types, return types, generic constraints) without a deprecation path.
- You MUST NOT remove public members without marking them obsolete first and providing a migration window.

### 2. Obsolescence Process

- You MUST mark obsolete APIs with `[Obsolete]` attribute including a replacement recommendation.
- You MUST provide at least one minor version cycle before removing an obsolete API.
- You MUST document breaking changes in `docs/migration/` when removing obsolete APIs.

### 3. Additive Changes Preferred

- You MAY add new optional parameters with default values to existing methods.
- You MAY add new method overloads that maintain existing behavior.
- You MAY add new public types or members without breaking existing consumers.
- You MUST NOT change default behavior of existing public methods without explicit approval and documentation.

### 4. Interface and Abstract Member Stability

- You MUST NOT remove members from public interfaces.
- You MUST NOT add required members to interfaces without a default implementation or explicit approval.
- You MUST NOT change the signature of abstract members in base classes without a deprecation path.

### 5. Extension Points

- You MAY introduce new extension points (e.g., new options classes, plugin interfaces) as additive public APIs.
- You MUST NOT change the behavior of existing extension points without documenting the change.

## Current Enforcement

These rules are currently enforced through:
- Code review requiring explicit justification for breaking changes
- No automated tooling currently validates public API surface
- Developers must manually verify API compatibility

## Related Documents

- [Testing Policy](./testing-policy.md)
- [Protocol Safety](./protocol-safety.md)
- [Serializer Safety](./serializer-safety.md)