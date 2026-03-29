# ADR-0002: Rename Extra to Extensions

## Status

Accepted

## Context

The repository contains modules using the `Extra.*` naming convention (e.g., `DotBPE.Extra`). This name is unclear to new contributors. It doesn't communicate what the module contains or why it exists separately from core.

The repository is being restructured toward clearer module boundaries. The `Extra` naming doesn't fit the target structure, which uses `Extensions` for optional, additive functionality.

## Decision

Rename `Extra.*` modules to `Extensions.*`.

Migration path:

1. Create new `DotBPE.Extensions` namespace and assembly.
2. Move public types from `Extra` to `Extensions`.
3. In the old `Extra` assembly, add `[Obsolete]` forwarding types that redirect to the new `Extensions` types.
4. Keep the `Extra` assembly for at least one major version cycle to give consumers time to migrate.

Example forwarding pattern:

```csharp
namespace DotBPE.Extra
{
    [Obsolete("Use DotBPE.Extensions.Foo instead.")]
    public class Foo : DotBPE.Extensions.Foo { }
}
```

## Consequences

**Positive:**

- Name clearly communicates purpose: these are extensions, not random extras.
- Aligns with .NET ecosystem conventions (`Microsoft.Extensions.*`).
- Forwarding types give consumers a smooth migration path.

**Negative:**

- Temporary maintenance burden of keeping both `Extra` and `Extensions` assemblies.
- Namespace change requires consumer code updates (though `[Obsolete]` warnings guide them).

**Neutral:**

- This is a naming improvement, not a behavior change. Functionality stays the same.
- The old `Extra` assembly can be removed in a future major version after deprecation period.
