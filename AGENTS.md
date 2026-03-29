# AGENTS.md

This repository is a framework/library repository for dotbpe.

## Primary goals
- Preserve .NET Standard 2.0 compatibility unless explicitly changed.
- Preserve protocol stability and serializer behavior unless explicitly changed.
- Prefer additive, backward-compatible public API changes.
- Keep AI-assisted functionality outside core runtime unless explicitly required.

## Documentation entry point
All repository knowledge, architecture, and rules are documented under:

- `docs/index.md`

Agents must use `docs/index.md` as the primary entry point to locate relevant documentation.

## Hard constraints
- Do not introduce APIs unavailable in .NET Standard 2.0 unless explicitly requested.
- Do not change binary protocol layout or message semantics unless explicitly requested.
- Do not silently change default serializer behavior.
- Do not rename or remove public APIs unless explicitly requested.
- Do not mix AI-specific tooling into core runtime without clear justification.

## Change expectations
When completing a task, report:
- what changed
- why it changed
- compatibility impact
- protocol/serializer impact
- tests run or recommended