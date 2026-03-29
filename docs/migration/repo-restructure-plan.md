# Repository Restructure: Migration Notes

## Who is affected?

Consumers of dotbpe who:

- Reference `DotBPE.Extra` assemblies or namespaces.
- Depend on the current project/solution layout for CI or build scripts.
- Have forked or vendored parts of the repository.

## What changed?

### Module renames

| Old | New | Status |
|-----|-----|--------|
| `DotBPE.Extra` | `DotBPE.Extensions` | Completed |

The old `DotBPE.Extra` namespace is forwarded via `ExtraNamespaceForwarding.cs` in each Extensions module. Consumers will see compiler warnings pointing to the new namespace.

### New modules added

| Module | Purpose |
|--------|---------|
| `DotBPE.Abstractions` | Shared contracts and interfaces (IRpcMessage, ISerializer, IMethod, IRpcContext, RpcResult) |
| `DotBPE.Protocol` | Binary protocol implementation (AmpMessage, AmpProtocol, CodecType, encode/decode handlers) |
| `DotBPE.BestPractice` | Best practices and recommended patterns |

### Final project layout

The source tree is organized as follows:

```text
src/
  DotBPE.Abstractions/     # Shared contracts and interfaces
  DotBPE.Protocol/         # Binary protocol implementation
  DotBPE.Rpc/              # RPC runtime (references Abstractions + Protocol)
  DotBPE.Baseline/         # Foundation utilities
  DotBPE.Gateway/          # HTTP gateway
  DotBPE.BestPractice/     # Best practices module
  DotBPE.Extensions.*/     # Extension modules (was Extra)
    Protobuf               # Protobuf serializer (CodecType=0)
    MessagePack            # MessagePack serializer (CodecType=1)
    Json                   # System.Text.Json serializer (CodecType=2)
    JsonNet                # Newtonsoft.Json serializer (CodecType=2)
    Castle                 # AOP interceptors, dynamic proxies
```

### Test layout

Tests are organized by purpose:

```text
tests/
  unit/
  integration/
  compatibility/          # Protects protocol, serializer, contract stability
  perf/
```

## What should consumers do?

### If you reference DotBPE.Extra

1. Update your code to use `DotBPE.Extensions` namespaces.
2. The `[Obsolete]` warnings will guide you to the correct types.
3. No behavior changes, just namespace updates.

### If you reference DotBPE.Rpc for protocol types

1. Protocol types (`AmpMessage`, `AmpProtocol`, `CodecType`) moved to `DotBPE.Protocol`.
2. Interface types (`ISerializer`, `IRpcMessage`) moved to `DotBPE.Abstractions`.
3. Update your `using` statements accordingly.

### If you have build scripts referencing project paths

1. Check the new project layout above.
2. Update any hardcoded paths in CI or build scripts.
3. Solution file (`.sln`) has been updated to reflect new structure.

### If you have forked or vendored code

1. Review the new module boundaries.
2. Identify which new module your forked code aligns with.
3. Consider upstreaming improvements.

## What did NOT change?

- Binary protocol layout and message semantics.
- Default serializer behavior.
- .NET Standard 2.0 target compatibility.
- Public API contracts (except namespace moves with forwarding).
