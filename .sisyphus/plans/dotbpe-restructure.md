# dotbpe Repository Restructuring — Phases 2-6

## TL;DR

> **Quick Summary**: Restructure dotbpe into an agent-friendly repository by normalizing documentation, isolating tool config, refactoring source boundaries (with Extra→Extensions rename and Abstractions extraction), reorganizing tests by purpose, and adding optional agent scaffolding — while preserving .NET Standard 2.0 compatibility, binary protocol stability, and backward-compatible public API evolution.
>
> **Deliverables**:
> - Complete docs/ structure with architecture, conventions, decisions, migration, guides, reference
> - .codex/config.toml for AI tool isolation
> - DotBPE.Abstractions project extracted from DotBPE.Rpc
> - DotBPE.Protocol project extracted from DotBPE.Rpc
> - All DotBPE.Extra.* renamed to DotBPE.Extensions.* with [Obsolete] forwarding
> - Tests reorganized into unit/, integration/, compatibility/, perf/
> - Optional agent/tooling scaffolding
>
> **Estimated Effort**: Large
> **Parallel Execution**: YES — 6 waves
> **Critical Path**: T12 (.codex) → T13 (Abstractions extract) → T16 (Protocol extract) → T19 (Extra→Extensions rename) → T23 (compat tests) → T26 (agent scaffolding)

---

## Context

### Original Request
User wants to restructure the dotbpe repository based on the existing PLAN.md, which defines a 7-phase restructuring plan (Phase 0-6). Phase 0 (baseline) and Phase 1 (governance docs) are mostly done. This plan covers Phases 2-6.

### Interview Summary
**Key Discussions**:
- **Execution Scope**: Full Phases 2-6 in one comprehensive plan
- **Extra→Extensions Rename**: Full rename with [Obsolete] forwarding types for backward compat
- **Source Split Strategy**: Incremental extraction — extract interfaces to Abstractions first, preserve DotBPE.Rpc as facade

**Research Findings**:
- Current structure: 9 src projects (mostly netstandard2.0), 4 test projects (xUnit), 5 samples
- Protocol-sensitive: AmpMessage.cs (binary header 18/21 bytes), AmpProtocol.cs, CodecType enum
- Serializer-sensitive: ISerializer.cs, all Extra.* implementations
- Public API: IRpcMessage, IRpcClient, IServiceActorHandler, IMethod, IRpcContext, RpcResult<T>
- "Extra" projects: 5 modules (Protobuf, MessagePack, Json, JsonNet, Castle)
- Tests use xUnit + Moq + AutoMocker, no test categories exist
- CI: GitHub Actions with .NET 8.0.x

### Metis Review
**Identified Gaps** (addressed):
- Protocol compatibility tests needed BEFORE Phase 4 moves → Added pre-refactor compatibility baseline task
- Gateway TFM strategy unclear → Default: keep current multi-target, document decision in ADR
- BestPractice project purpose unclear → Document in architecture overview, do not move in Phase 4
- Peach dependency version lock → Guardrail: do not change Peach 0.2.2 during refactor
- Sample project references need updating after reorganization → Included in Phase 4 tasks
- [Obsolete] forwarding approach needs specifics → Detailed in Phase 4 task descriptions

---

## Work Objectives

### Core Objective
Restructure dotbpe repository into an agent-friendly, well-documented, clearly-bounded module structure while preserving protocol stability, serializer behavior, and .NET Standard 2.0 compatibility.

### Concrete Deliverables
- Complete `docs/` hierarchy with 11+ documentation files
- `.codex/config.toml` for AI tool isolation
- `src/DotBPE.Abstractions/` project (extracted from DotBPE.Rpc)
- `src/DotBPE.Protocol/` project (extracted from DotBPE.Rpc)
- All 5 `DotBPE.Extra.*` projects renamed to `DotBPE.Extensions.*` with backward-compat forwarding
- Tests reorganized: `tests/unit/`, `tests/integration/`, `tests/compatibility/`, `tests/perf/`
- Optional `src/DotBPE.Tooling/` and `samples/ai-assisted/`

### Definition of Done
- [ ] `dotnet build DotBPE.sln` succeeds (exit code 0)
- [ ] `dotnet test` passes all existing tests
- [ ] Solution file updated with all new/renamed projects
- [ ] All docs/index.md links resolve
- [ ] Protocol compatibility tests pass (post-refactor)
- [ ] [Obsolete] forwarding types exist for all renamed public APIs

### Must Have
- .NET Standard 2.0 compatibility for all core modules (Rpc, Abstractions, Protocol, Extensions.*)
- Binary protocol unchanged (AmpMessage header lengths, AmpProtocol byte positions)
- Serializer behavior unchanged (CodecType values, ISerializer contract)
- [Obsolete] forwarding for all renamed public types
- All existing tests continue passing

### Must NOT Have (Guardrails)
- Protocol changes to AmpMessage header or AmpProtocol encoding
- Serializer default changes or CodecType enum value changes
- Peach dependency version changes
- Mandatory AI runtime dependencies in core framework modules
- Breaking changes to public APIs without [Obsolete] forwarding
- Reformatting of unrelated files
- Collapsing documentation into AI-specific structures

---

## Verification Strategy (MANDATORY)

### Test Decision
- **Infrastructure exists**: YES (xUnit 2.4.x + Moq 4.18.2 + AutoMocker 3.4.0)
- **Automated tests**: Tests-after (structural refactoring, not new features)
- **Framework**: xUnit (via `dotnet test`)
- **Verification**: Run full test suite after each phase boundary

### QA Policy
Every task MUST include agent-executed QA scenarios.
Evidence saved to `.sisyphus/evidence/task-{N}-{scenario-slug}.{ext}`.

- **Build verification**: `dotnet build DotBPE.sln` — must exit 0
- **Test verification**: `dotnet test` — all tests must pass
- **Documentation**: Verify all internal links resolve
- **Protocol**: Verify AmpMessage header constants unchanged (grep for VERSION_0_HEAD_LENGTH, VERSION_1_HEAD_LENGTH)

---

## Execution Strategy

### Parallel Execution Waves

```
Wave 1 (Start Immediately — documentation + config, fully independent):
├── Task 1:  Create docs/index.md with full navigation [writing]
├── Task 2:  Create docs/architecture/ (3 files) [writing]
├── Task 3:  Create docs/conventions/ (5 files) [writing]
├── Task 4:  Create docs/decisions/ + docs/migration/ [writing]
├── Task 5:  Create docs/guides/ + docs/reference/ placeholders [quick]
└── Task 6:  Create .codex/config.toml [quick]

Wave 2 (After Wave 1 — source extraction):
├── Task 7:  Create DotBPE.Abstractions project + extract interfaces [deep]
├── Task 8:  Create DotBPE.Protocol project + extract protocol code [deep]
├── Task 9:  Create protocol compatibility baseline tests [unspecified-high]
└── Task 10: Update solution file and project references [quick]

Wave 3 (After Task 7 — rename Extra→Extensions):
├── Task 11: Rename Extra.Protobuf → Extensions.Protobuf [deep]
├── Task 12: Rename Extra.MessagePack → Extensions.MessagePack [deep]
├── Task 13: Rename Extra.Json → Extensions.Json [deep]
├── Task 14: Rename Extra.JsonNet → Extensions.JsonNet [deep]
└── Task 15: Rename Extra.Castle → Extensions.Castle [deep]

Wave 4 (After Wave 3 — integration + validation):
├── Task 16: Update all sample project references [quick]
├── Task 17: Update .sln file with all renamed/new projects [quick]
├── Task 18: Full build + test verification [unspecified-high]
└── Task 19: Update docs/ with final module descriptions [writing]

Wave 5 (After Task 18 — test reorganization):
├── Task 20: Create tests/unit/ structure + migrate [unspecified-high]
├── Task 21: Create tests/integration/ structure [quick]
├── Task 22: Create tests/compatibility/ + serializer compat tests [unspecified-high]
└── Task 23: Create tests/perf/ placeholder [quick]

Wave 6 (After Wave 5 — optional agent scaffolding):
├── Task 24: Create DotBPE.Tooling project (optional) [unspecified-low]
├── Task 25: Create scripts/agent/ directory [quick]
└── Task 26: Create samples/ai-assisted/ placeholder [quick]

Critical Path: T1 → T7 → T10 → T11-T15 → T18 → T20-T22 → T24
Parallel Speedup: ~65% faster than sequential
Max Concurrent: 6 (Wave 1), 4 (Wave 2), 5 (Wave 3)
```

### Dependency Matrix

| Task | Depends On | Blocks |
|------|-----------|--------|
| 1-6  | None | Phase 2 exit criteria |
| 7    | None | 10, 11-15, 20 |
| 8    | None | 10, 22 |
| 9    | None | 22 |
| 10   | 7, 8 | 16, 17, 18 |
| 11-15| 7 | 16, 17, 18 |
| 16   | 10, 11-15 | 18 |
| 17   | 10, 11-15 | 18 |
| 18   | 16, 17 | 19, 20-23 |
| 19   | 18 | None |
| 20   | 7, 18 | None |
| 21   | None | None |
| 22   | 8, 9, 18 | None |
| 23   | None | None |
| 24-26| None | None |

### Agent Dispatch Summary

- **Wave 1**: 6 tasks → T1-T4 `writing`, T5 `quick`, T6 `quick`
- **Wave 2**: 4 tasks → T7 `deep`, T8 `deep`, T9 `unspecified-high`, T10 `quick`
- **Wave 3**: 5 tasks → T11-T15 all `deep`
- **Wave 4**: 4 tasks → T16 `quick`, T17 `quick`, T18 `unspecified-high`, T19 `writing`
- **Wave 5**: 4 tasks → T20 `unspecified-high`, T21 `quick`, T22 `unspecified-high`, T23 `quick`
- **Wave 6**: 3 tasks → T24 `unspecified-low`, T25 `quick`, T26 `quick`

---

## TODOs

- [ ] 1. Create docs/index.md with full navigation

  **What to do**:
  - Create `docs/index.md` as the single documentation entry point
  - Include navigation sections: Architecture, Conventions, Decisions, Guides, Migration, Reference
  - Link to AGENTS.md and PLAN.md from root
  - Each section should have a brief description and link to its contents
  - Include a "Getting Started" section pointing to samples/

  **Must NOT do**:
  - Do not duplicate content from AGENTS.md (link to it instead)
  - Do not include implementation details (that belongs in sub-pages)

  **Recommended Agent Profile**:
  - **Category**: `writing`
    - Reason: Documentation creation requires clear prose and structural thinking
  - **Skills**: []
    - No specific skills needed
  - **Skills Evaluated but Omitted**: None needed for markdown creation

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 1 (with Tasks 2-10)
  - **Blocks**: None (navigation hub, not a dependency)
  - **Blocked By**: None (can start immediately)

  **References**:
  - `AGENTS.md` — Understand existing constraints and navigation expectations
  - `PLAN.md` — Understand target structure and what sections to link

  **Acceptance Criteria**:
  - [ ] File exists: `docs/index.md`
  - [ ] All navigation sections present (Architecture, Conventions, Decisions, Guides, Migration, Reference)
  - [ ] Links to AGENTS.md and PLAN.md present

  **QA Scenarios**:
  ```
  Scenario: docs/index.md renders as valid markdown
    Tool: Bash (cat)
    Preconditions: docs/index.md exists
    Steps:
      1. cat docs/index.md
      2. Verify all section headers present
      3. Verify no broken markdown syntax
    Expected Result: Valid markdown with all 6 section headers
    Evidence: .sisyphus/evidence/task-1-index-render.txt
  ```

  **Commit**: YES (group with 2-10)
  - Message: `docs(repo): add docs/index.md with full navigation`
  - Files: `docs/index.md`

---

- [ ] 2. Create docs/architecture/ overview and protocol documentation

  **What to do**:
  - Create `docs/architecture/overview.md` — High-level module diagram, dependency direction, key abstractions
  - Create `docs/architecture/protocol.md` — AMP binary protocol specification (from existing docs/amp协议.md)
  - Create `docs/architecture/serialization.md` — Serializer interface design, CodecType enum, supported formats
  - Reference actual source files for accuracy: AmpMessage.cs, AmpProtocol.cs, ISerializer.cs
  - Document module responsibilities: Baseline, Rpc, Gateway, Extensions

  **Must NOT do**:
  - Do not document intended future behavior — only document current behavior
  - Do not change protocol documentation from what amp协议.md currently says
  - Do not include implementation code snippets (reference file paths instead)

  **Recommended Agent Profile**:
  - **Category**: `writing`
    - Reason: Technical documentation requires clear architecture description
  - **Skills**: []
    - No specific skills needed

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 1 (with Tasks 1, 3-10)
  - **Blocks**: None
  - **Blocked By**: None

  **References**:
  - `docs/amp协议.md` — Existing protocol documentation (source for protocol.md)
  - `src/DotBPE.Rpc/Protocols/AmpMessage.cs` — Binary header structure (VERSION_0_HEAD_LENGTH=18, VERSION_1_HEAD_LENGTH=21)
  - `src/DotBPE.Rpc/Protocols/AmpProtocol.cs` — Encoding/decoding logic
  - `src/DotBPE.Rpc/Codec/ISerializer.cs` — Serializer interface
  - `src/DotBPE.Rpc/Codec/CodecType.cs` — Serializer type enum

  **Acceptance Criteria**:
  - [ ] `docs/architecture/overview.md` exists with module diagram and responsibilities
  - [ ] `docs/architecture/protocol.md` exists with AMP protocol spec
  - [ ] `docs/architecture/serialization.md` exists with serializer design

  **QA Scenarios**:
  ```
  Scenario: Architecture docs contain accurate protocol constants
    Tool: Bash (grep)
    Preconditions: docs/architecture/protocol.md exists
    Steps:
      1. grep "18" docs/architecture/protocol.md
      2. grep "21" docs/architecture/protocol.md
      3. Verify header length values match source code
    Expected Result: Protocol constants match AmpMessage.cs (18 and 21 bytes)
    Evidence: .sisyphus/evidence/task-2-protocol-accuracy.txt
  ```

  **Commit**: YES (group with 1, 3-10)
  - Message: `docs(repo): add architecture and conventions documentation`
  - Files: `docs/architecture/`

---

- [ ] 3. Create docs/conventions/ (5 files)

  **What to do**:
  - Create `docs/conventions/public-api-boundary.md` — Rules for public API changes, deprecation process
  - Create `docs/conventions/netstandard20-compat.md` — .NET Standard 2.0 compatibility rules, forbidden APIs
  - Create `docs/conventions/protocol-safety.md` — Protocol modification rules, review requirements
  - Create `docs/conventions/serializer-safety.md` — Serializer behavior preservation rules
  - Create `docs/conventions/testing-policy.md` — Test organization, coverage expectations, TDD policy

  **Must NOT do**:
  - Do not make rules aspirational — document current enforcement (or lack thereof)
  - Do not reference tools or processes that don't exist yet

  **Recommended Agent Profile**:
  - **Category**: `writing`
    - Reason: Convention documentation needs precise, actionable language
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 1
  - **Blocks**: None
  - **Blocked By**: None

  **References**:
  - `AGENTS.md` — Existing hard constraints to codify
  - `PLAN.md` — Non-goals and success criteria for conventions
  - `build/version.props` — Package configuration reference

  **Acceptance Criteria**:
  - [ ] All 5 convention files exist
  - [ ] Each file has concrete, enforceable rules (not vague guidelines)
  - [ ] netstandard20-compat.md lists forbidden API categories

  **QA Scenarios**:
  ```
  Scenario: Convention files contain actionable rules
    Tool: Bash (grep)
    Preconditions: All 5 files exist
    Steps:
      1. grep -c "MUST\|SHALL\|DO NOT" docs/conventions/*.md
      2. Verify each file has at least 3 enforceable rules
    Expected Result: Each file contains imperative rules
    Evidence: .sisyphus/evidence/task-3-conventions-rules.txt
  ```

  **Commit**: YES (group with 1-2, 4-10)

---

- [ ] 4. Create docs/decisions/ and docs/migration/ documentation

  **What to do**:
  - Create `docs/decisions/adr-0001-agent-boundary.md` — ADR documenting why AI tooling must be separated from core runtime
  - Create `docs/decisions/adr-0002-extra-to-extensions.md` — ADR for the Extra→Extensions rename decision
  - Create `docs/migration/repo-restructure-plan.md` — Migration notes for consumers affected by restructuring
  - Use standard ADR format: Title, Status, Context, Decision, Consequences

  **Must NOT do**:
  - Do not include future plans as if they've already happened
  - Do not make ADRs longer than necessary (1-2 pages max each)

  **Recommended Agent Profile**:
  - **Category**: `writing`
    - Reason: ADR format requires structured technical writing
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 1
  - **Blocks**: None
  - **Blocked By**: None

  **References**:
  - `PLAN.md` — Risk register and non-goals for ADR context
  - `AGENTS.md` — AI separation principles

  **Acceptance Criteria**:
  - [ ] `docs/decisions/adr-0001-agent-boundary.md` exists with standard ADR format
  - [ ] `docs/decisions/adr-0002-extra-to-extensions.md` exists
  - [ ] `docs/migration/repo-restructure-plan.md` exists

  **QA Scenarios**:
  ```
  Scenario: ADRs follow standard format
    Tool: Bash (grep)
    Preconditions: ADR files exist
    Steps:
      1. grep "## Status" docs/decisions/adr-*.md
      2. grep "## Context" docs/decisions/adr-*.md
      3. grep "## Decision" docs/decisions/adr-*.md
      4. grep "## Consequences" docs/decisions/adr-*.md
    Expected Result: All 4 sections present in each ADR
    Evidence: .sisyphus/evidence/task-4-adr-format.txt
  ```

  **Commit**: YES (group with 1-3, 5-10)

---

- [ ] 5. Create docs/guides/ and docs/reference/ placeholders

  **What to do**:
  - Create `docs/guides/README.md` — Placeholder explaining guides will be added as workflows mature
  - Create `docs/reference/README.md` — Placeholder listing future API reference structure
  - Keep both minimal — these are structural placeholders for future content

  **Must NOT do**:
  - Do not create placeholder content that implies features exist
  - Do not auto-generate API docs from code (that's a future task)

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Simple placeholder creation
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 1
  - **Blocks**: None
  - **Blocked By**: None

  **References**: None needed — minimal placeholders

  **Acceptance Criteria**:
  - [ ] `docs/guides/README.md` exists
  - [ ] `docs/reference/README.md` exists

  **QA Scenarios**:
  ```
  Scenario: Placeholder files exist and are valid
    Tool: Bash (ls + cat)
    Preconditions: None
    Steps:
      1. ls docs/guides/README.md
      2. ls docs/reference/README.md
      3. Verify files are non-empty
    Expected Result: Both files exist with content
    Evidence: .sisyphus/evidence/task-5-placeholders.txt
  ```

  **Commit**: YES (group with 1-4, 6-10)

---

- [ ] 6. Create .codex/config.toml

  **What to do**:
  - Create `.codex/` directory
  - Create `.codex/config.toml` with sandbox mode, approval policy, project root markers
  - Keep config minimal — runtime/config concerns only
  - Do not duplicate repository rules from AGENTS.md

  **Must NOT do**:
  - Do not create a second documentation tree under .codex/
  - Do not duplicate rules from AGENTS.md
  - Do not add .codex/ to .gitignore

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Simple config file creation
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 1
  - **Blocks**: None
  - **Blocked By**: None

  **References**:
  - `AGENTS.md` — Understand what constraints exist at root level (to avoid duplicating)

  **Acceptance Criteria**:
  - [ ] `.codex/config.toml` exists
  - [ ] Contains sandbox mode setting
  - [ ] Contains approval policy setting
  - [ ] Contains project root markers

  **QA Scenarios**:
  ```
  Scenario: .codex/config.toml is valid TOML
    Tool: Bash (cat)
    Preconditions: File exists
    Steps:
      1. cat .codex/config.toml
      2. Verify TOML syntax (key = value format)
      3. Verify no duplicate rules from AGENTS.md
    Expected Result: Valid minimal TOML config
    Evidence: .sisyphus/evidence/task-6-codex-config.txt
  ```

  **Commit**: YES (group with 1-5, 7-10)

---

- [ ] 7. Create DotBPE.Abstractions project

  **What to do**:
  - Create `src/DotBPE.Abstractions/` directory with .csproj targeting netstandard2.0
  - Extract shared interfaces from DotBPE.Rpc: IRpcMessage, IMethod, IRpcContext, RpcResult<T>
  - Move ISerializer interface to Abstractions (keep implementation in Rpc for now)
  - Add ProjectReference from DotBPE.Rpc → DotBPE.Abstractions
  - Update all Extra.* projects to reference Abstractions
  - Create forwarding types with [Obsolete] in original namespaces

  **Must NOT do**:
  - Do not move protocol implementation (AmpMessage, AmpProtocol) — that's Task 8
  - Do not change any interface signatures
  - Do not change Peach dependency version (must remain 0.2.2)
  - Do not move DotBPE.Baseline contents (keep it separate)

  **Recommended Agent Profile**:
  - **Category**: `deep`
    - Reason: Requires careful dependency analysis and interface extraction
  - **Skills**: ['dotnet-best-practices']
    - `dotnet-best-practices`: Ensure .NET Standard 2.0 compatibility for extracted interfaces

  **Parallelization**:
  - **Can Run In Parallel**: NO
  - **Parallel Group**: Wave 2 (with Tasks 8-10)
  - **Blocks**: Tasks 11-15 (rename tasks depend on abstractions being extracted)
  - **Blocked By**: None (can start immediately, but solution file update depends on this)

  **References**:
  - `src/DotBPE.Rpc/DotBPE.Rpc.csproj` — Current project structure and dependencies
  - `src/DotBPE.Rpc/Protocols/IRpcMessage.cs` — Interface to extract
  - `src/DotBPE.Rpc/IMethod.cs` — Interface to extract
  - `src/DotBPE.Rpc/IRpcContext.cs` — Interface to extract
  - `src/DotBPE.Rpc/RpcResult.cs` — Type to extract
  - `src/DotBPE.Rpc/Codec/ISerializer.cs` — Interface to extract

  **Acceptance Criteria**:
  - [ ] `src/DotBPE.Abstractions/DotBPE.Abstractions.csproj` exists targeting netstandard2.0
  - [ ] Extracted interfaces compile successfully
  - [ ] DotBPE.Rpc references DotBPE.Abstractions
  - [ ] `dotnet build DotBPE.sln` succeeds
  - [ ] [Obsolete] forwarding types exist in original namespaces

  **QA Scenarios**:
  ```
  Scenario: Abstractions project builds and references resolve
    Tool: Bash (dotnet build)
    Preconditions: Project created
    Steps:
      1. dotnet build src/DotBPE.Abstractions/DotBPE.Abstractions.csproj
      2. dotnet build DotBPE.sln
      3. Verify exit code 0 for both
    Expected Result: Both builds succeed, no errors
    Evidence: .sisyphus/evidence/task-7-abstractions-build.txt

  Scenario: Extracted interfaces have correct namespace
    Tool: Bash (grep)
    Preconditions: Build succeeds
    Steps:
      1. grep "namespace DotBPE.Abstractions" src/DotBPE.Abstractions/*.cs
      2. Verify forwarding [Obsolete] types in src/DotBPE.Rpc/
    Expected Result: Interfaces in DotBPE.Abstractions namespace, forwarding types exist
    Evidence: .sisyphus/evidence/task-7-namespaces.txt
  ```

  **Commit**: YES (group with 8-10)
  - Message: `refactor(src): extract DotBPE.Abstractions from DotBPE.Rpc`
  - Files: `src/DotBPE.Abstractions/`, `src/DotBPE.Rpc/`

---

- [ ] 8. Create DotBPE.Protocol project

  **What to do**:
  - Create `src/DotBPE.Protocol/` directory with .csproj targeting netstandard2.0
  - Move protocol code from DotBPE.Rpc: AmpMessage.cs, AmpProtocol.cs, AmpEncodeHandler.cs, AmpDecodeHandler.cs
  - Move CodecType enum to Protocol project
  - Add ProjectReference from DotBPE.Rpc → DotBPE.Protocol
  - Create [Obsolete] forwarding types in original DotBPE.Rpc namespaces
  - Preserve Peach and DotNetty dependencies

  **Must NOT do**:
  - Do NOT change AmpMessage header constants (VERSION_0_HEAD_LENGTH=18, VERSION_1_HEAD_LENGTH=21)
  - Do NOT change CodecType enum values (Protobuf=0, MessagePack=1, JSON=2)
  - Do NOT change AmpProtocol encoding/decoding byte positions
  - Do NOT change Peach dependency version (0.2.2)
  - Do NOT change DotNetty dependency versions

  **Recommended Agent Profile**:
  - **Category**: `deep`
    - Reason: Protocol code is highly sensitive — requires careful extraction without behavior change
  - **Skills**: ['dotnet-best-practices']
    - `dotnet-best-practices`: Ensure protocol extraction maintains binary compatibility

  **Parallelization**:
  - **Can Run In Parallel**: NO (must verify Abstractions doesn't conflict)
  - **Parallel Group**: Wave 2 (after Task 7 completes or in parallel with caution)
  - **Blocks**: Tasks 11-15 (compatibility tests depend on protocol location)
  - **Blocked By**: None (but solution file update depends on this)

  **References**:
  - `src/DotBPE.Rpc/Protocols/AmpMessage.cs` — Binary header structure (MUST NOT CHANGE)
  - `src/DotBPE.Rpc/Protocols/AmpProtocol.cs` — Encoding/decoding (MUST NOT CHANGE)
  - `src/DotBPE.Rpc/Protocols/AmpEncodeHandler.cs` — Encoder (move)
  - `src/DotBPE.Rpc/Protocols/AmpDecodeHandler.cs` — Decoder (move)
  - `src/DotBPE.Rpc/Codec/CodecType.cs` — Enum (move, values MUST NOT CHANGE)

  **Acceptance Criteria**:
  - [ ] `src/DotBPE.Protocol/DotBPE.Protocol.csproj` exists targeting netstandard2.0
  - [ ] Protocol files moved to DotBPE.Protocol
  - [ ] AmpMessage header constants unchanged (verify: grep for 18 and 21)
  - [ ] CodecType enum values unchanged (verify: grep for Protobuf=0)
  - [ ] `dotnet build DotBPE.sln` succeeds
  - [ ] [Obsolete] forwarding types exist

  **QA Scenarios**:
  ```
  Scenario: Protocol constants unchanged after extraction
    Tool: Bash (grep)
    Preconditions: Build succeeds
    Steps:
      1. grep "VERSION_0_HEAD_LENGTH = 18" src/DotBPE.Protocol/Protocols/AmpMessage.cs
      2. grep "VERSION_1_HEAD_LENGTH = 21" src/DotBPE.Protocol/Protocols/AmpMessage.cs
      3. grep "Protobuf = 0" src/DotBPE.Protocol/Codec/CodecType.cs
    Expected Result: All constants match original values
    Evidence: .sisyphus/evidence/task-8-protocol-constants.txt

  Scenario: Solution builds with protocol extraction
    Tool: Bash (dotnet build)
    Preconditions: Project created
    Steps:
      1. dotnet build src/DotBPE.Protocol/DotBPE.Protocol.csproj
      2. dotnet build DotBPE.sln
    Expected Result: Both builds succeed
    Evidence: .sisyphus/evidence/task-8-protocol-build.txt
  ```

  **Commit**: YES (group with 7, 9-10)
  - Message: `refactor(src): extract DotBPE.Protocol from DotBPE.Rpc`
  - Files: `src/DotBPE.Protocol/`, `src/DotBPE.Rpc/`

---

- [ ] 9. Create protocol compatibility baseline tests

  **What to do**:
  - Create `tests/compatibility/` directory structure
  - Create `tests/compatibility/DotBPE.ProtocolCompatibility.Tests/` project
  - Write tests that verify: AmpMessage header encoding produces expected byte layout
  - Write tests that verify: CodecType enum values are stable
  - Write tests that verify: Round-trip encode/decode produces identical message
  - These tests serve as a safety net for any future refactoring

  **Must NOT do**:
  - Do not test business logic — only test binary protocol stability
  - Do not depend on network connectivity — use in-memory encode/decode

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
    - Reason: Requires understanding of binary protocol to write meaningful assertions
  - **Skills**: ['dotnet-best-practices']
    - `dotnet-best-practices`: Test patterns for .NET

  **Parallelization**:
  - **Can Run In Parallel**: NO (needs Protocol project from Task 8)
  - **Parallel Group**: Wave 2 (after Task 8)
  - **Blocks**: Tasks 16+ (compatibility test structure needed for test reorg)
  - **Blocked By**: Task 8 (Protocol project must exist)

  **References**:
  - `src/DotBPE.Protocol/Protocols/AmpMessage.cs` — Message structure to test
  - `src/DotBPE.Protocol/Protocols/AmpProtocol.cs` — Encode/decode logic to test
  - `src/DotBPE.Protocol/Codec/CodecType.cs` — Enum values to verify
  - `tests/DotBPE.Rpc.Tests/` — Existing test patterns (xUnit + AutoMocker)

  **Acceptance Criteria**:
  - [ ] `tests/compatibility/DotBPE.ProtocolCompatibility.Tests/` project exists
  - [ ] At least 3 protocol stability tests: header encoding, enum values, round-trip
  - [ ] `dotnet test tests/compatibility/` passes

  **QA Scenarios**:
  ```
  Scenario: Protocol compatibility tests pass
    Tool: Bash (dotnet test)
    Preconditions: Test project created
    Steps:
      1. dotnet test tests/compatibility/DotBPE.ProtocolCompatibility.Tests/
      2. Verify all tests pass
      3. Verify at least 3 test cases exist
    Expected Result: All protocol compatibility tests pass
    Evidence: .sisyphus/evidence/task-9-compat-tests.txt
  ```

  **Commit**: YES (group with 7-8, 10)

---

- [ ] 10. Update DotBPE.sln and project references

  **What to do**:
  - Add new projects to DotBPE.sln: DotBPE.Abstractions, DotBPE.Protocol
  - Update ProjectReference in DotBPE.Rpc → point to new projects
  - Update ProjectReference in all Extra.* projects → add Abstractions reference
  - Verify solution file is valid with `dotnet sln list`

  **Must NOT do**:
  - Do not remove DotBPE.Rpc from solution (it's still the main runtime)
  - Do not change NuGet package references

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Solution file and reference updates are mechanical
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (depends on Tasks 7-8)
  - **Parallel Group**: Wave 2 (after Tasks 7-8)
  - **Blocks**: Tasks 11-15 (rename tasks need correct references)
  - **Blocked By**: Tasks 7-8

  **References**:
  - `DotBPE.sln` — Solution file to update
  - `src/DotBPE.Rpc/DotBPE.Rpc.csproj` — Add ProjectReference to Abstractions and Protocol

  **Acceptance Criteria**:
  - [ ] `dotnet sln list` shows all projects including new ones
  - [ ] All ProjectReferences resolve
  - [ ] `dotnet build DotBPE.sln` succeeds

  **QA Scenarios**:
  ```
  Scenario: Solution includes all projects
    Tool: Bash (dotnet sln list)
    Preconditions: Solution updated
    Steps:
      1. dotnet sln list
      2. Verify DotBPE.Abstractions appears
      3. Verify DotBPE.Protocol appears
    Expected Result: All projects listed
    Evidence: .sisyphus/evidence/task-10-sln-list.txt
  ```

  **Commit**: YES (group with 7-9)

---

- [ ] 11. Rename DotBPE.Extra.Protobuf → DotBPE.Extensions.Protobuf

  **What to do**:
  - Rename directory: `src/DotBPE.Extra.Protobuf/` → `src/DotBPE.Extensions.Protobuf/`
  - Rename .csproj file and update internal name
  - Change namespace from `DotBPE.Extra.Protobuf` to `DotBPE.Extensions.Protobuf`
  - Create forwarding types with `[Obsolete("Use DotBPE.Extensions.Protobuf instead")]` in original namespace
  - Update DotBPE.sln
  - Update any sample/test references

  **Must NOT do**:
  - Do NOT change ProtobufSerializer implementation logic
  - Do NOT change any serialization behavior
  - Do NOT change NuGet package dependencies

  **Recommended Agent Profile**:
  - **Category**: `deep`
    - Reason: Rename requires careful namespace handling with backward-compatible forwarding
  - **Skills**: ['dotnet-best-practices']
    - `dotnet-best-practices`: Obsolete attribute usage and namespace forwarding patterns

  **Parallelization**:
  - **Can Run In Parallel**: YES (all 5 rename tasks are independent)
  - **Parallel Group**: Wave 3 (with Tasks 12-15)
  - **Blocks**: Tasks 16-17 (sample/reference updates)
  - **Blocked By**: Task 7 (Abstractions must exist)

  **References**:
  - `src/DotBPE.Extra.Protobuf/DotBPE.Extra.Protobuf.csproj` — Project to rename
  - `src/DotBPE.Extra.Protobuf/ProtobufSerializer.cs` — Main implementation file
  - `tests/DotBPE.Extra.Castle.Tests/` — Pattern reference for [Obsolete] forwarding

  **Acceptance Criteria**:
  - [ ] `src/DotBPE.Extensions.Protobuf/` exists
  - [ ] Old namespace forwarding types with [Obsolete] exist
  - [ ] `dotnet build` succeeds
  - [ ] No behavior change in serialization

  **QA Scenarios**:
  ```
  Scenario: Renamed project builds and old namespace still resolves
    Tool: Bash (dotnet build)
    Preconditions: Rename complete
    Steps:
      1. dotnet build src/DotBPE.Extensions.Protobuf/DotBPE.Extensions.Protobuf.csproj
      2. dotnet build DotBPE.sln
    Expected Result: Both builds succeed
    Evidence: .sisyphus/evidence/task-11-protobuf-rename.txt
  ```

  **Commit**: YES (group with 12-15)
  - Message: `refactor(src): rename Extra.Protobuf → Extensions.Protobuf`

---

- [ ] 12. Rename DotBPE.Extra.MessagePack → DotBPE.Extensions.MessagePack

  **What to do**:
  - Rename directory: `src/DotBPE.Extra.MessagePack/` → `src/DotBPE.Extensions.MessagePack/`
  - Rename .csproj file and update internal name
  - Change namespace with [Obsolete] forwarding
  - Update solution and references

  **Must NOT do**:
  - Do NOT change MessagePackSerializer implementation logic
  - Do NOT change serialization behavior

  **Recommended Agent Profile**:
  - **Category**: `deep`
    - Reason: Same as Task 11 — careful namespace forwarding
  - **Skills**: ['dotnet-best-practices']

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 3 (with Tasks 11, 13-15)
  - **Blocks**: Tasks 16-17
  - **Blocked By**: Task 7

  **References**:
  - `src/DotBPE.Extra.MessagePack/DotBPE.Extra.MessagePack.csproj`
  - `src/DotBPE.Extra.MessagePack/MessagePackSerializer.cs`

  **Acceptance Criteria**:
  - [ ] `src/DotBPE.Extensions.MessagePack/` exists
  - [ ] Old namespace forwarding types with [Obsolete] exist
  - [ ] `dotnet build` succeeds

  **QA Scenarios**:
  ```
  Scenario: MessagePack rename builds correctly
    Tool: Bash (dotnet build)
    Preconditions: Rename complete
    Steps:
      1. dotnet build src/DotBPE.Extensions.MessagePack/
      2. dotnet build DotBPE.sln
    Expected Result: Both builds succeed
    Evidence: .sisyphus/evidence/task-12-messagepack-rename.txt
  ```

  **Commit**: YES (group with 11, 13-15)

---

- [ ] 13. Rename DotBPE.Extra.Json → DotBPE.Extensions.Json

  **What to do**:
  - Rename directory: `src/DotBPE.Extra.Json/` → `src/DotBPE.Extensions.Json/`
  - Rename .csproj and update namespace with [Obsolete] forwarding
  - Note: This project targets netcoreapp3.1 (different from standard netstandard2.0)
  - Do NOT change target framework in this task

  **Must NOT do**:
  - Do NOT change target framework (netcoreapp3.1 stays)
  - Do NOT change JsonSerializer implementation logic

  **Recommended Agent Profile**:
  - **Category**: `deep`
    - Reason: Different target framework adds complexity
  - **Skills**: ['dotnet-best-practices']

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 3
  - **Blocks**: Tasks 16-17
  - **Blocked By**: Task 7

  **References**:
  - `src/DotBPE.Extra.Json/DotBPE.Extra.Json.csproj` — Note: netcoreapp3.1 target
  - `src/DotBPE.Extra.Json/JsonSerializer.cs`

  **Acceptance Criteria**:
  - [ ] `src/DotBPE.Extensions.Json/` exists
  - [ ] Target framework remains netcoreapp3.1
  - [ ] Old namespace forwarding types with [Obsolete] exist
  - [ ] `dotnet build` succeeds

  **QA Scenarios**:
  ```
  Scenario: Json rename preserves target framework
    Tool: Bash (grep + dotnet build)
    Preconditions: Rename complete
    Steps:
      1. grep "netcoreapp3.1" src/DotBPE.Extensions.Json/DotBPE.Extensions.Json.csproj
      2. dotnet build DotBPE.sln
    Expected Result: Target framework preserved, build succeeds
    Evidence: .sisyphus/evidence/task-13-json-rename.txt
  ```

  **Commit**: YES (group with 11-12, 14-15)

---

- [ ] 14. Rename DotBPE.Extra.JsonNet → DotBPE.Extensions.JsonNet

  **What to do**:
  - Rename directory: `src/DotBPE.Extra.JsonNet/` → `src/DotBPE.Extensions.JsonNet/`
  - Rename .csproj and update namespace with [Obsolete] forwarding

  **Must NOT do**:
  - Do NOT change JsonSerializer implementation logic

  **Recommended Agent Profile**:
  - **Category**: `deep`
    - Reason: Same rename pattern as others
  - **Skills**: ['dotnet-best-practices']

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 3
  - **Blocks**: Tasks 16-17
  - **Blocked By**: Task 7

  **References**:
  - `src/DotBPE.Extra.JsonNet/DotBPE.Extra.JsonNet.csproj`
  - `src/DotBPE.Extra.JsonNet/JsonSerializer.cs`

  **Acceptance Criteria**:
  - [ ] `src/DotBPE.Extensions.JsonNet/` exists
  - [ ] Old namespace forwarding types with [Obsolete] exist
  - [ ] `dotnet build` succeeds

  **QA Scenarios**:
  ```
  Scenario: JsonNet rename builds correctly
    Tool: Bash (dotnet build)
    Preconditions: Rename complete
    Steps:
      1. dotnet build DotBPE.sln
    Expected Result: Build succeeds
    Evidence: .sisyphus/evidence/task-14-jsonnet-rename.txt
  ```

  **Commit**: YES (group with 11-13, 15)

---

- [ ] 15. Rename DotBPE.Extra.Castle → DotBPE.Extensions.Castle

  **What to do**:
  - Rename directory: `src/DotBPE.Extra.Castle/` → `src/DotBPE.Extensions.Castle/`
  - Rename .csproj and update namespace with [Obsolete] forwarding
  - Update Castle test project reference (tests/DotBPE.Extra.Castle.Tests/)

  **Must NOT do**:
  - Do NOT change Castle DynamicProxy behavior
  - Do NOT change AOP interception logic

  **Recommended Agent Profile**:
  - **Category**: `deep`
    - Reason: Castle integration is complex — need to verify DynamicProxy still works
  - **Skills**: ['dotnet-best-practices']

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 3
  - **Blocks**: Tasks 16-17
  - **Blocked By**: Task 7

  **References**:
  - `src/DotBPE.Extra.Castle/DotBPE.Extra.Castle.csproj`
  - `src/DotBPE.Extra.Castle/` — Castle DI/AOP extension files
  - `tests/DotBPE.Extra.Castle.Tests/` — Test project reference to update

  **Acceptance Criteria**:
  - [ ] `src/DotBPE.Extensions.Castle/` exists
  - [ ] Old namespace forwarding types with [Obsolete] exist
  - [ ] `dotnet build` succeeds
  - [ ] Castle tests still pass

  **QA Scenarios**:
  ```
  Scenario: Castle rename with tests still passing
    Tool: Bash (dotnet test)
    Preconditions: Rename complete
    Steps:
      1. dotnet build DotBPE.sln
      2. dotnet test tests/DotBPE.Extra.Castle.Tests/
    Expected Result: Build succeeds, Castle tests pass
    Evidence: .sisyphus/evidence/task-15-castle-rename.txt
  ```

  **Commit**: YES (group with 11-14)

---

- [ ] 16. Update all sample project references

  **What to do**:
  - Update ProjectReference in all 5 sample projects to point to new project locations
  - Update namespace references in sample code (using DotBPE.Extensions.* instead of DotBPE.Extra.*)
  - Verify each sample builds individually

  **Must NOT do**:
  - Do not change sample logic or behavior
  - Do not change target frameworks of samples

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Mechanical reference updates
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (depends on rename tasks 11-15)
  - **Parallel Group**: Wave 4
  - **Blocks**: Task 18 (full build verification)
  - **Blocked By**: Tasks 11-15

  **References**:
  - `samples/MathService.Definition/` — Shared definitions
  - `samples/MathService/` — Service sample
  - `samples/MathClient/` — Client sample
  - `samples/MathHttpClient/` — HTTP client sample
  - `samples/GreeterHttpService/` — HTTP service sample

  **Acceptance Criteria**:
  - [ ] All 5 sample projects build successfully
  - [ ] `dotnet build samples/MathService/MathService.csproj` succeeds
  - [ ] `dotnet build samples/MathClient/MathClient.csproj` succeeds

  **QA Scenarios**:
  ```
  Scenario: All samples build after reference updates
    Tool: Bash (dotnet build)
    Preconditions: Rename tasks complete
    Steps:
      1. dotnet build samples/MathService.Definition/MathService.Definition.csproj
      2. dotnet build samples/MathService/MathService.csproj
      3. dotnet build samples/MathClient/MathClient.csproj
    Expected Result: All builds succeed
    Evidence: .sisyphus/evidence/task-16-samples-build.txt
  ```

  **Commit**: YES (group with 17)
  - Message: `build(repo): update solution and sample references`

---

- [ ] 17. Update .sln file with all renamed/new projects

  **What to do**:
  - Remove old Extra.* project entries from DotBPE.sln
  - Add Extensions.* project entries
  - Add Abstractions and Protocol project entries
  - Verify with `dotnet sln list`

  **Must NOT do**:
  - Do not remove test projects from solution

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Solution file update is mechanical
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (depends on Tasks 11-15)
  - **Parallel Group**: Wave 4
  - **Blocks**: Task 18
  - **Blocked By**: Tasks 11-15

  **References**:
  - `DotBPE.sln` — Solution file to update

  **Acceptance Criteria**:
  - [ ] `dotnet sln list` shows all projects with correct names
  - [ ] No old Extra.* project entries remain
  - [ ] All Extensions.* projects listed

  **QA Scenarios**:
  ```
  Scenario: Solution file has correct project list
    Tool: Bash (dotnet sln list)
    Preconditions: Solution updated
    Steps:
      1. dotnet sln list
      2. Verify "DotBPE.Extensions.Protobuf" appears
      3. Verify "DotBPE.Extra.Protobuf" does NOT appear
      4. Verify "DotBPE.Abstractions" appears
      5. Verify "DotBPE.Protocol" appears
    Expected Result: Correct project list
    Evidence: .sisyphus/evidence/task-17-sln-verify.txt
  ```

  **Commit**: YES (group with 16)

---

- [ ] 18. Full build and test verification

  **What to do**:
  - Run `dotnet build DotBPE.sln -c Release` — must succeed
  - Run `dotnet test` — all existing tests must pass
  - Run `dotnet build` for each sample individually
  - Verify protocol constants unchanged (grep VERSION_0_HEAD_LENGTH, VERSION_1_HEAD_LENGTH)
  - Verify CodecType enum values unchanged
  - Verify Peach dependency version unchanged

  **Must NOT do**:
  - Do not fix unrelated test failures (report them)
  - Do not suppress warnings to pass build

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
    - Reason: Comprehensive verification requires running all checks
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (depends on all previous tasks)
  - **Parallel Group**: Wave 4
  - **Blocks**: Tasks 19-23 (test reorg and docs update)
  - **Blocked By**: Tasks 16-17

  **References**: All source and test projects

  **Acceptance Criteria**:
  - [ ] `dotnet build DotBPE.sln -c Release` exits 0
  - [ ] `dotnet test` — all tests pass
  - [ ] Protocol constants verified unchanged
  - [ ] Serializer constants verified unchanged
  - [ ] Peach dependency verified unchanged

  **QA Scenarios**:
  ```
  Scenario: Full solution builds in Release mode
    Tool: Bash (dotnet build)
    Preconditions: All rename/reference updates complete
    Steps:
      1. dotnet build DotBPE.sln -c Release
      2. Verify exit code 0
    Expected Result: Build succeeds with no errors
    Evidence: .sisyphus/evidence/task-18-build-release.txt

  Scenario: All tests pass
    Tool: Bash (dotnet test)
    Preconditions: Build succeeds
    Steps:
      1. dotnet test --no-build --verbosity normal
      2. Verify all tests pass
      3. Verify test count >= current baseline
    Expected Result: All tests pass, no regressions
    Evidence: .sisyphus/evidence/task-18-tests-all.txt

  Scenario: Protocol constants unchanged
    Tool: Bash (grep)
    Preconditions: Build succeeds
    Steps:
      1. grep "VERSION_0_HEAD_LENGTH = 18" src/DotBPE.Protocol/Protocols/AmpMessage.cs
      2. grep "VERSION_1_HEAD_LENGTH = 21" src/DotBPE.Protocol/Protocols/AmpMessage.cs
      3. grep "Protobuf = 0" src/DotBPE.Protocol/Codec/CodecType.cs
    Expected Result: All constants match original values
    Evidence: .sisyphus/evidence/task-18-protocol-check.txt
  ```

  **Commit**: YES (standalone)
  - Message: `build(repo): verify full solution builds and tests pass`

---

- [ ] 19. Update docs/ with final module descriptions

  **What to do**:
  - Update docs/architecture/overview.md with final module structure (Abstractions, Protocol, Extensions)
  - Update docs/architecture/serialization.md with Extensions.* names
  - Update docs/migration/repo-restructure-plan.md with actual changes made
  - Update docs/index.md if new sections were added

  **Must NOT do**:
  - Do not add speculative future plans
  - Do not document changes that weren't made

  **Recommended Agent Profile**:
  - **Category**: `writing`
    - Reason: Documentation update after structural changes
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (depends on Task 18 verification)
  - **Parallel Group**: Wave 4
  - **Blocks**: None
  - **Blocked By**: Task 18

  **References**:
  - All docs/ files created in Wave 1
  - Actual source tree after refactoring

  **Acceptance Criteria**:
  - [ ] docs/architecture/overview.md reflects actual module structure
  - [ ] Module names match actual project names
  - [ ] Migration notes reflect actual changes

  **QA Scenarios**:
  ```
  Scenario: Documentation matches actual structure
    Tool: Bash (grep)
    Preconditions: Refactoring complete
    Steps:
      1. grep "DotBPE.Extensions" docs/architecture/overview.md
      2. grep "DotBPE.Abstractions" docs/architecture/overview.md
      3. grep "DotBPE.Protocol" docs/architecture/overview.md
    Expected Result: All new module names appear in docs
    Evidence: .sisyphus/evidence/task-19-docs-update.txt
  ```

  **Commit**: YES (standalone)
  - Message: `docs(repo): update module descriptions after refactoring`

---

- [ ] 20. Create tests/unit/ structure and migrate existing unit tests

  **What to do**:
  - Create `tests/unit/` directory
  - Move `tests/DotBPE.Rpc.Tests/` → `tests/unit/DotBPE.Rpc.Tests/`
  - Move `tests/DotBPE.Extra.Castle.Tests/` → `tests/unit/DotBPE.Extensions.Castle.Tests/` (rename too)
  - Move `tests/DotBPE.Gateway.Tests/` → `tests/unit/DotBPE.Gateway.Tests/`
  - Move `tests/DotBPE.TestBase/` → `tests/unit/DotBPE.TestBase/`
  - Update solution file
  - Verify all tests still pass

  **Must NOT do**:
  - Do not change test logic or assertions
  - Do not remove or add tests (just move)
  - Do not change target frameworks

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
    - Reason: Moving test projects requires updating many references
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: NO (depends on Task 18 verification)
  - **Parallel Group**: Wave 5 (with Tasks 21-23)
  - **Blocks**: Task 23 (final test verification)
  - **Blocked By**: Task 18

  **References**:
  - `tests/DotBPE.Rpc.Tests/` — Test project to move
  - `tests/DotBPE.Gateway.Tests/` — Test project to move
  - `tests/DotBPE.Extra.Castle.Tests/` — Test project to move and rename
  - `tests/DotBPE.TestBase/` — Test base to move

  **Acceptance Criteria**:
  - [ ] `tests/unit/` contains all 4 test projects
  - [ ] `dotnet build DotBPE.sln` succeeds
  - [ ] `dotnet test tests/unit/` passes

  **QA Scenarios**:
  ```
  Scenario: Unit tests pass after migration
    Tool: Bash (dotnet test)
    Preconditions: Test projects moved
    Steps:
      1. dotnet build DotBPE.sln
      2. dotnet test tests/unit/ --no-build
    Expected Result: All unit tests pass
    Evidence: .sisyphus/evidence/task-20-unit-tests.txt
  ```

  **Commit**: YES (group with 21-23)
  - Message: `test(repo): reorganize tests into unit directory`

---

- [ ] 21. Create tests/integration/ structure

  **What to do**:
  - Create `tests/integration/` directory
  - Create `tests/integration/README.md` explaining integration test criteria
  - No existing tests to migrate — this is scaffolding for future integration tests
  - Integration tests should test cross-module behavior (e.g., client→server round-trip)

  **Must NOT do**:
  - Do not move unit tests into integration/
  - Do not create fake integration tests just to fill the directory

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Directory scaffolding + README
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 5
  - **Blocks**: None
  - **Blocked By**: None

  **References**: None — new scaffolding

  **Acceptance Criteria**:
  - [ ] `tests/integration/` directory exists
  - [ ] `tests/integration/README.md` exists with criteria for integration tests

  **QA Scenarios**:
  ```
  Scenario: Integration test directory exists with documentation
    Tool: Bash (ls + cat)
    Preconditions: None
    Steps:
      1. ls tests/integration/
      2. cat tests/integration/README.md
    Expected Result: Directory and README exist
    Evidence: .sisyphus/evidence/task-21-integration-scaffold.txt
  ```

  **Commit**: YES (group with 20, 22-23)

---

- [ ] 22. Create tests/compatibility/ with serializer compatibility tests

  **What to do**:
  - Move protocol compatibility tests from Task 9 to `tests/compatibility/`
  - Create `tests/compatibility/DotBPE.SerializerCompatibility.Tests/` project
  - Write serializer compatibility tests: verify each serializer produces identical output for sample messages
  - Create `tests/compatibility/DotBPE.ContractCompatibility.Tests/` project (scaffold)
  - Contract compatibility tests verify public API surface hasn't changed

  **Must NOT do**:
  - Do not test serialization of types not currently used in production
  - Do not create brittle tests that depend on exact byte output (test structure instead)

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
    - Reason: Compatibility tests require deep understanding of serialization contracts
  - **Skills**: ['dotnet-best-practices']
    - `dotnet-best-practices`: .NET testing patterns

  **Parallelization**:
  - **Can Run In Parallel**: NO (needs protocol tests from Task 9)
  - **Parallel Group**: Wave 5
  - **Blocks**: None
  - **Blocked By**: Tasks 9, 20

  **References**:
  - `src/DotBPE.Protocol/Codec/ISerializer.cs` — Interface to test against
  - `src/DotBPE.Extensions.Protobuf/ProtobufSerializer.cs` — Serializer to verify
  - `tests/compatibility/DotBPE.ProtocolCompatibility.Tests/` — Existing compat tests from Task 9

  **Acceptance Criteria**:
  - [ ] `tests/compatibility/DotBPE.SerializerCompatibility.Tests/` exists
  - [ ] At least 2 serializer compatibility tests per serializer
  - [ ] `tests/compatibility/DotBPE.ContractCompatibility.Tests/` scaffold exists
  - [ ] `dotnet test tests/compatibility/` passes

  **QA Scenarios**:
  ```
  Scenario: Compatibility tests pass
    Tool: Bash (dotnet test)
    Preconditions: Test projects created
    Steps:
      1. dotnet build tests/compatibility/
      2. dotnet test tests/compatibility/ --no-build
    Expected Result: All compatibility tests pass
    Evidence: .sisyphus/evidence/task-22-compat-tests.txt
  ```

  **Commit**: YES (group with 20-21, 23)

---

- [ ] 23. Create tests/perf/ placeholder

  **What to do**:
  - Create `tests/perf/` directory
  - Create `tests/perf/README.md` explaining performance test criteria
  - No existing perf tests — this is scaffolding

  **Must NOT do**:
  - Do not create fake benchmarks

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Directory scaffolding
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 5
  - **Blocks**: None
  - **Blocked By**: None

  **References**: None

  **Acceptance Criteria**:
  - [ ] `tests/perf/` directory exists
  - [ ] `tests/perf/README.md` exists

  **QA Scenarios**:
  ```
  Scenario: Perf test directory exists
    Tool: Bash (ls)
    Preconditions: None
    Steps:
      1. ls tests/perf/
    Expected Result: Directory exists with README
    Evidence: .sisyphus/evidence/task-23-perf-scaffold.txt
  ```

  **Commit**: YES (group with 20-22)
  - Message: `test(repo): reorganize tests by purpose`

---

- [ ] 24. Create DotBPE.Tooling project (optional agent scaffolding)

  **What to do**:
  - Create `src/DotBPE.Tooling/` directory with .csproj targeting netstandard2.0
  - This is a placeholder for future AI-assisted development tools
  - Create minimal project file and README explaining purpose
  - Do NOT add runtime dependencies — this is purely optional tooling

  **Must NOT do**:
  - Do not add dependencies from Tooling to core runtime
  - Do not make core runtime depend on Tooling
  - Do not implement actual tooling features (scaffold only)

  **Recommended Agent Profile**:
  - **Category**: `unspecified-low`
    - Reason: Simple scaffolding
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 6
  - **Blocks**: None
  - **Blocked By**: None

  **References**: None — new optional module

  **Acceptance Criteria**:
  - [ ] `src/DotBPE.Tooling/DotBPE.Tooling.csproj` exists
  - [ ] Project builds independently
  - [ ] No dependencies on core runtime (one-way only if any)

  **QA Scenarios**:
  ```
  Scenario: Tooling project builds independently
    Tool: Bash (dotnet build)
    Preconditions: Project created
    Steps:
      1. dotnet build src/DotBPE.Tooling/DotBPE.Tooling.csproj
    Expected Result: Build succeeds
    Evidence: .sisyphus/evidence/task-24-tooling-build.txt
  ```

  **Commit**: YES (group with 25-26)
  - Message: `feat(agent): add optional agent tooling scaffolding`

---

- [ ] 25. Create scripts/agent/ directory

  **What to do**:
  - Create `scripts/agent/` directory
  - Create `scripts/agent/README.md` explaining agent script conventions
  - Create `scripts/agent/verify-protocol.sh` — script to verify protocol constants haven't changed
  - Keep scripts minimal and useful

  **Must NOT do**:
  - Do not create scripts that modify source code
  - Do not create scripts with side effects

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Simple script creation
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 6
  - **Blocks**: None
  - **Blocked By**: None

  **References**:
  - `scripts/build.sh` — Existing script pattern to follow

  **Acceptance Criteria**:
  - [ ] `scripts/agent/` directory exists
  - [ ] `scripts/agent/verify-protocol.sh` exists and is executable

  **QA Scenarios**:
  ```
  Scenario: Agent scripts exist and are valid
    Tool: Bash (ls + bash -n)
    Preconditions: Scripts created
    Steps:
      1. ls scripts/agent/
      2. bash -n scripts/agent/verify-protocol.sh
    Expected Result: Scripts exist and have valid syntax
    Evidence: .sisyphus/evidence/task-25-agent-scripts.txt
  ```

  **Commit**: YES (group with 24, 26)

---

- [ ] 26. Create samples/ai-assisted/ placeholder

  **What to do**:
  - Create `samples/ai-assisted/` directory
  - Create `samples/ai-assisted/README.md` explaining purpose (future AI-assisted workflow examples)
  - Keep minimal — this is a structural placeholder

  **Must NOT do**:
  - Do not create sample code that implies features exist

  **Recommended Agent Profile**:
  - **Category**: `quick`
    - Reason: Directory scaffolding
  - **Skills**: []

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 6
  - **Blocks**: None
  - **Blocked By**: None

  **References**: None

  **Acceptance Criteria**:
  - [ ] `samples/ai-assisted/` directory exists
  - [ ] `samples/ai-assisted/README.md` exists

  **QA Scenarios**:
  ```
  Scenario: AI-assisted samples directory exists
    Tool: Bash (ls)
    Preconditions: None
    Steps:
      1. ls samples/ai-assisted/
    Expected Result: Directory exists with README
    Evidence: .sisyphus/evidence/task-26-ai-samples.txt
  ```

  **Commit**: YES (group with 24-25)

---

## Final Verification Wave (MANDATORY — after ALL implementation tasks)

> 4 review agents run in PARALLEL. ALL must APPROVE. Present consolidated results to user and get explicit "okay" before completing.

- [ ] F1. **Plan Compliance Audit** — `oracle`
  Read the plan end-to-end. For each "Must Have": verify implementation exists (read file, dotnet build, run command). For each "Must NOT Have": search codebase for forbidden patterns — reject with file:line if found. Check evidence files exist in .sisyphus/evidence/. Compare deliverables against plan.
  Output: `Must Have [N/N] | Must NOT Have [N/N] | Tasks [N/N] | VERDICT: APPROVE/REJECT`

- [ ] F2. **Build & Test Verification** — `unspecified-high`
  Run `dotnet build DotBPE.sln -c Release` — must exit 0. Run `dotnet test` — all tests pass. Check for compiler warnings. Verify all project references resolve. Check solution file includes all new/renamed projects.
  Output: `Build [PASS/FAIL] | Tests [N pass/N fail] | Warnings [N] | Solution [VALID/INVALID] | VERDICT`

- [ ] F3. **Documentation Quality Review** — `writing`
  Read docs/index.md — verify all links resolve. Check each docs/ file for completeness. Verify architecture docs match actual code structure. Check conventions docs contain actionable rules. Verify ADR is properly formatted.
  Output: `Docs [N/N complete] | Links [N/N valid] | Content [ACCURATE/ISSUES] | VERDICT`

- [ ] F4. **Protocol & Serializer Safety Check** — `deep`
  Verify AmpMessage header constants unchanged (grep VERSION_0_HEAD_LENGTH=18, VERSION_1_HEAD_LENGTH=21). Verify CodecType enum values unchanged (Protobuf=0, MessagePack=1, JSON=2). Verify ISerializer interface signature unchanged. Verify Peach dependency version unchanged (0.2.2). Verify protocol compatibility tests pass.
  Output: `Protocol [SAFE/DRIFT] | Serializer [SAFE/DRIFT] | Dependencies [SAFE/CHANGED] | VERDICT`

---

## Commit Strategy

- **1**: `docs(repo): add docs/index.md with full navigation` — docs/index.md
- **2**: `docs(repo): add architecture and conventions documentation` — docs/architecture/*, docs/conventions/*
- **3**: `docs(repo): add ADR and migration documentation` — docs/decisions/*, docs/migration/*
- **4**: `build(repo): add .codex/config.toml` — .codex/config.toml
- **5**: `refactor(src): extract DotBPE.Abstractions from DotBPE.Rpc` — src/DotBPE.Abstractions/, src/DotBPE.Rpc/
- **6**: `refactor(src): extract DotBPE.Protocol from DotBPE.Rpc` — src/DotBPE.Protocol/, src/DotBPE.Rpc/
- **7**: `test(repo): add protocol compatibility baseline tests` — tests/compatibility/
- **8**: `refactor(src): rename Extra→Extensions with [Obsolete] forwarding` — src/DotBPE.Extensions.*/
- **9**: `build(repo): update solution and sample references` — DotBPE.sln, samples/
- **10**: `test(repo): reorganize tests by purpose` — tests/unit/, tests/integration/, tests/compatibility/, tests/perf/
- **11**: `feat(agent): add optional agent tooling scaffolding` — src/DotBPE.Tooling/, scripts/agent/, samples/ai-assisted/

---

## Success Criteria

### Verification Commands
```bash
dotnet build DotBPE.sln -c Release          # Expected: Build succeeded, exit 0
dotnet test                                  # Expected: All tests pass
grep "VERSION_0_HEAD_LENGTH = 18" src/DotBPE.Protocol/Protocols/AmpMessage.cs  # Expected: match
grep "VERSION_1_HEAD_LENGTH = 21" src/DotBPE.Protocol/Protocols/AmpMessage.cs  # Expected: match
grep "Protobuf = 0" src/DotBPE.Protocol/Codec/CodecType.cs                     # Expected: match
grep "Peach" src/DotBPE.Abstractions/DotBPE.Abstractions.csproj                # Expected: 0.2.2
```

### Final Checklist
- [ ] docs/index.md exists with full navigation
- [ ] All docs/ subdirectories populated
- [ ] .codex/config.toml exists
- [ ] DotBPE.Abstractions project exists and builds
- [ ] DotBPE.Protocol project exists and builds
- [ ] All DotBPE.Extra.* renamed to DotBPE.Extensions.*
- [ ] [Obsolete] forwarding types exist for renamed public APIs
- [ ] Tests reorganized into unit/integration/compatibility/perf
- [ ] dotnet build succeeds
- [ ] dotnet test passes
- [ ] Protocol constants unchanged
- [ ] Serializer behavior unchanged
