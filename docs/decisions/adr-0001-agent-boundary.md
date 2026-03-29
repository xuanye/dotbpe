# ADR-0001: Agent Boundary

## Status

Accepted

## Context

dotbpe is a .NET RPC framework targeting .NET Standard 2.0. The repository is being restructured to support AI-assisted development workflows (code generation, review, migration tooling).

There is a risk that AI-specific dependencies or patterns leak into core runtime modules. If that happens:

- Consumers who don't use AI tooling still pull in unnecessary dependencies.
- Core runtime stability becomes coupled to rapidly evolving AI tooling.
- .NET Standard 2.0 compatibility becomes harder to guarantee.

The question is: where should AI-assisted functionality live?

## Decision

AI-assisted tooling must remain **optional and separated** from core runtime modules.

Concretely:

1. Core runtime modules (`DotBPE.Core`, `DotBPE.Protocol`, `DotBPE.Serialization`, `DotBPE.Rpc`) must not reference or depend on any AI/agent packages.
2. AI tooling lives in separate, optional modules (`DotBPE.Tooling`, `DotBPE.Agent.*`).
3. Core runtime must build and function without any agent/tooling modules present.
4. Agent modules may depend on core runtime, but not the reverse.

## Consequences

**Positive:**

- Core runtime stays lean and stable.
- Consumers who don't need AI tooling aren't affected.
- .NET Standard 2.0 compatibility is easier to preserve in core modules.
- Agent tooling can evolve independently without risking runtime stability.

**Negative:**

- Some duplication may occur if agent tooling needs to extend core types.
- Integration points between agent modules and core must be designed carefully.

**Neutral:**

- This is a structural constraint, not a feature limitation. Agent tooling can still be powerful; it just can't be mandatory.
