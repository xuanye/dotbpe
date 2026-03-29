# Performance Tests

This directory is reserved for performance benchmarking and profiling of dotbpe.

## Purpose

Establish baseline performance metrics and regression detection for:
- Serialization/deserialization throughput
- Message encoding/decoding latency
- Memory allocation patterns
- Concurrent request handling

## Test Criteria

Performance tests should measure:
1. **Latency**: P50, P95, P99 response times
2. **Throughput**: Requests/operations per second
3. **Memory**: Allocations per operation, GC pressure
4. **CPU**: Compute time per operation

## Guidelines

- Use realistic payloads and scenarios
- Measure across multiple iterations
- Record environmental context (hardware, .NET version)
- Compare against baseline before introducing changes
