# Testing Policy

This document defines the testing requirements and organization for the dotbpe repository.

## Rationale

Tests protect the stability of the framework. They ensure that changes do not break existing functionality and that new features work as expected. A well-organized test suite also helps developers understand the codebase and make changes with confidence.

## Test Organization

### Current Structure

Tests are organized by purpose:
- **Unit tests**: Test individual components in isolation
- **Integration tests**: Test multiple components working together
- **Compatibility tests**: Test protocol and serializer compatibility (not yet implemented)
- **Performance tests**: Test performance characteristics (not yet implemented)

### Test Project Naming

- Test projects SHOULD follow the naming pattern `DotBPE.[Module].Tests`
- Test classes SHOULD be named after the class being tested with `Tests` suffix
- Test methods SHOULD follow the pattern `MethodName_Scenario_ExpectedResult`

## Enforceable Rules

### 1. Test Coverage Requirements

- You MUST ensure new public APIs have corresponding unit tests.
- You MUST ensure protocol-affecting changes have corresponding compatibility tests.
- You MUST ensure serializer-affecting changes have corresponding compatibility tests.
- You SHOULD maintain at least basic coverage for existing public APIs.

### 2. Test-Driven Development

- You SHOULD write tests before implementing new features.
- You MUST write tests before fixing bugs to prevent regression.
- You MAY write tests after implementation for documentation purposes.

### 3. Test Isolation

- You MUST ensure tests do not depend on execution order.
- You MUST ensure tests do not depend on external services that may be unavailable.
- You SHOULD use mocks or in-memory implementations for external dependencies.

### 4. Test Naming and Documentation

- You MUST name test methods descriptively to indicate what is being tested.
- You SHOULD include comments for complex test scenarios.
- You MUST avoid test method names that are too generic (e.g., `Test1`, `Test2`).

### 5. Test Maintenance

- You MUST fix failing tests before merging changes.
- You MUST update tests when changing public API contracts.
- You SHOULD remove tests for removed features rather than commenting them out.

## Current Enforcement

These rules are currently enforced through:
- Code review requiring justification for missing tests
- No automated test coverage checking is currently integrated
- Developers must manually verify test coverage

## Related Documents

- [Public API Boundary](./public-api-boundary.md)
- [Protocol Safety](./protocol-safety.md)
- [Serializer Safety](./serializer-safety.md)