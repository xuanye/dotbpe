# Integration Tests

This directory contains integration tests for DotBPE.

## Purpose

Integration tests verify cross-module behavior and end-to-end scenarios that unit tests cannot cover.

## Criteria for Integration Tests

An integration test should:

1. **Test cross-module behavior** - Verify interaction between multiple components (e.g., client ↔ server, gateway ↔ RPC)
2. **Test client→server round-trip** - Complete request/response flow through the system
3. **Use real dependencies** - No heavy mocking of core components
4. **Verify system properties** - Such as serialization, transport, protocol handling

## What Belongs Here

- End-to-end RPC call tests
- Gateway to service communication tests
- Protocol serialization/deserialization round-trips
- Multi-component integration scenarios

## What Does NOT Belong Here

- Unit tests (keep in respective `DotBPE.*.Tests` projects)
- Tests with heavy mocking of core components
- Tests that only verify single class behavior

## Running Integration Tests

```bash
dotnet test tests/integration/
```
