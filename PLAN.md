
````md
# PLAN.md

## Title
Restructure dotbpe into an agent-friendly repository without weakening runtime stability

## Status
Proposed

## Goal
Refactor the dotbpe repository into a structure that is easier for AI agents and human contributors to work with, while preserving:

- .NET Standard 2.0 compatibility
- binary protocol stability
- serializer behavior stability
- backward-compatible public API evolution
- separation between core runtime and AI-assisted tooling

This plan is intended to be executable by AI agents in small, reviewable steps.

---

## Non-goals

The following are explicitly out of scope unless later requested:

- redesigning the RPC protocol
- changing the default serializer behavior
- upgrading target frameworks beyond current compatibility requirements
- introducing mandatory AI runtime dependencies into core framework modules
- large-scale business logic rewrites unrelated to repository structure
- opportunistic package upgrades
- unrelated formatting-only changes across the repository

---

## Success criteria

This plan is considered successful if the repository reaches the following state:

1. The repository has a clear documentation entry point at `docs/index.md`.
2. The repository has an `AGENTS.md` file with concise hard constraints and execution expectations.
3. Documentation is organized into stable sections:
   - `docs/architecture/`
   - `docs/conventions/`
   - `docs/decisions/`
   - `docs/guides/`
   - `docs/migration/`
   - `docs/reference/`
4. AI-tool-specific configuration is minimized and isolated under `.codex/`.
5. Source projects are reorganized toward clearer boundaries:
   - abstractions
   - core utilities
   - protocol
   - serialization
   - rpc runtime
   - gateway
   - extensions
   - optional agent/tooling modules
6. Tests are reorganized by purpose:
   - unit
   - integration
   - compatibility
   - perf
7. Compatibility-sensitive code remains protected by explicit conventions and tests.
8. Each phase is independently reviewable and reversible.

---

## Core principles

Agents must follow these principles during execution:

1. Prefer small, localized changes.
2. Preserve behavior unless a behavior change is explicitly requested.
3. Prefer additive changes over breaking changes.
4. Do not mix AI-specific concerns into core protocol/runtime layers without explicit justification.
5. Keep documentation and structure changes separate from functional changes whenever possible.
6. Update the nearest relevant document whenever structure or responsibilities change.
7. Make the repository easier for both humans and AI agents to navigate.

---

## Hard constraints

Agents must not violate any of the following unless explicitly instructed:

- Do not introduce APIs unavailable in .NET Standard 2.0.
- Do not change binary protocol layout or message semantics.
- Do not silently change serializer defaults or serialization contracts.
- Do not rename or remove public APIs without clearly flagging the break.
- Do not move unrelated code just to “clean things up”.
- Do not upgrade packages unless the task explicitly includes dependency work.
- Do not reformat unrelated files.
- Do not collapse documentation into AI-specific private structures.
- Do not add mandatory AI runtime dependencies to core framework modules.

---

## Repository target shape

The repository should evolve toward the following shape:

```text
dotbpe/
├─ AGENTS.md
├─ PLAN.md
├─ .codex/
│  └─ config.toml
├─ docs/
│  ├─ index.md
│  ├─ architecture/
│  ├─ conventions/
│  ├─ decisions/
│  ├─ guides/
│  ├─ migration/
│  └─ reference/
├─ build/
├─ scripts/
│  ├─ ci/
│  ├─ dev/
│  └─ agent/
├─ samples/
│  ├─ basic/
│  ├─ gateway/
│  ├─ serialization/
│  ├─ extensions/
│  └─ ai-assisted/
├─ src/
│  ├─ DotBPE.Abstractions/
│  ├─ DotBPE.Core/
│  ├─ DotBPE.Protocol/
│  ├─ DotBPE.Serialization/
│  ├─ DotBPE.Rpc/
│  ├─ DotBPE.Gateway/
│  ├─ DotBPE.Extensions/
│  ├─ DotBPE.Tooling/
│  └─ DotBPE.Agent/
└─ tests/
   ├─ unit/
   ├─ integration/
   ├─ compatibility/
   └─ perf/
````

This is the target direction, not a requirement for one-shot completion.

---

## Execution strategy

The plan must be executed in phases. Each phase should be committed independently.

### Phase 0 - Baseline understanding

Objective:

* understand current repository structure
* identify public API boundaries
* identify protocol-sensitive code
* identify serializer-sensitive code
* identify current extension layout
* identify current test layout

Deliverables:

* a short inventory of current top-level structure
* a short list of protocol-sensitive files/modules
* a short list of serialization-sensitive files/modules
* a short list of public API-heavy modules
* a recommended move map for later phases

Rules:

* no behavior changes
* no package changes
* no directory moves yet

Exit criteria:

* enough knowledge exists to proceed without guesswork

---

### Phase 1 - Add governance entry points

Objective:
Introduce repository-level guidance for humans and AI agents.

Tasks:

* add `AGENTS.md`
* add `PLAN.md`
* add `docs/index.md`
* add empty or initial docs subfolders:

  * `docs/architecture/`
  * `docs/conventions/`
  * `docs/decisions/`
  * `docs/guides/`
  * `docs/migration/`
  * `docs/reference/`

Deliverables:

* `AGENTS.md`
* `PLAN.md`
* `docs/index.md`

Rules:

* keep `AGENTS.md` concise
* navigation belongs in `docs/index.md`, not in `AGENTS.md`
* do not move code in this phase

Exit criteria:

* repository has a single documentation entry point
* repository has AI-agent execution constraints documented

---

### Phase 2 - Normalize documentation structure

Objective:
Move project knowledge into a consistent documentation layout.

Tasks:

* create or migrate architecture docs
* create or migrate compatibility and engineering rules into `docs/conventions/`
* create initial ADRs under `docs/decisions/`
* create migration notes under `docs/migration/`

Recommended starter files:

* `docs/architecture/overview.md`
* `docs/architecture/runtime-layering.md`
* `docs/architecture/protocol.md`
* `docs/architecture/serialization.md`
* `docs/conventions/public-api-boundary.md`
* `docs/conventions/netstandard20-compat.md`
* `docs/conventions/protocol-safety.md`
* `docs/conventions/serializer-safety.md`
* `docs/conventions/testing-policy.md`
* `docs/decisions/adr-0001-agent-boundary.md`
* `docs/migration/repo-restructure-plan.md`

Rules:

* prefer moving or rewriting existing knowledge into docs instead of duplicating it
* protocol documentation must reflect current behavior, not intended future behavior
* documentation must not claim changes that have not happened yet

Exit criteria:

* docs structure is useful to both humans and AI agents
* compatibility rules are documented in stable locations

---

### Phase 3 - Isolate tool-specific configuration

Objective:
Minimize AI-tool-specific configuration and keep it separate from repository knowledge.

Tasks:

* create `.codex/config.toml`
* keep Codex-specific settings limited to runtime/config concerns
* do not duplicate repository rules in `.codex/config.toml`

Configuration should focus on:

* sandbox mode
* approval policy
* project root markers
* instructions file if needed

Rules:

* `AGENTS.md` remains the main rule entry point
* `docs/index.md` remains the main navigation entry point
* `.codex/` must not become a second documentation tree

Exit criteria:

* Codex can operate with project-level config
* repository knowledge remains tool-neutral

---

### Phase 4 - Source tree boundary refactor

Objective:
Move toward a clearer source layout without changing behavior.

Tasks:

* introduce or confirm module boundaries for:

  * abstractions
  * core
  * protocol
  * serialization
  * runtime
  * gateway
  * extensions
  * optional tooling/agent modules
* rename `Extra.*` style modules toward `Extensions.*` if appropriate
* move protocol-specific code toward `DotBPE.Protocol`
* move serializer abstractions toward `DotBPE.Serialization`
* move shared contracts toward `DotBPE.Abstractions`
* keep behavior-preserving moves separate from logic changes

Rules:

* prefer move-only commits when possible
* avoid mixed refactor + behavior-change commits
* preserve namespace and assembly compatibility unless a deliberate breaking change is planned
* do not move code blindly; verify dependency directions first

Exit criteria:

* boundaries are clearer
* protocol and serialization responsibilities are less entangled with runtime orchestration

---

### Phase 5 - Reorganize tests by purpose

Objective:
Make test intent clear and improve compatibility protection.

Tasks:

* create:

  * `tests/unit/`
  * `tests/integration/`
  * `tests/compatibility/`
  * `tests/perf/`
* migrate or create test projects according to purpose
* ensure protocol, serializer, and contract changes can be covered distinctly

Recommended compatibility test projects:

* `DotBPE.ContractCompatibility.Tests`
* `DotBPE.ProtocolCompatibility.Tests`
* `DotBPE.SerializerCompatibility.Tests`

Rules:

* do not weaken existing coverage while moving tests
* if test moves are large, prefer structure-only moves before test rewrites
* compatibility tests should protect behavior that users depend on

Exit criteria:

* test layout reflects responsibility
* compatibility-sensitive areas are easier to validate

---

### Phase 6 - Introduce optional agent/tooling scaffolding

Objective:
Add optional AI-assisted tooling without contaminating core runtime.

Tasks:

* create optional agent/tooling modules only if needed
* create agent scripts under `scripts/agent/`
* create sample workflows under `samples/ai-assisted/`

Possible modules:

* `DotBPE.Tooling`
* `DotBPE.Agent.Abstractions`
* `DotBPE.Agent.Workflows`
* `DotBPE.Agent.Cli`

Rules:

* agent-related code must remain optional
* core runtime must not require agent tooling to build or function
* agent features should assist design, review, migration, or generation workflows first

Exit criteria:

* repository can support AI-assisted workflows
* core framework remains cleanly separated from AI-specific tooling

---

## Task decomposition rules for agents

When executing any phase, agents must decompose work as follows:

1. Identify the smallest safe unit of change.
2. Check whether the change touches:

   * public API
   * protocol
   * serialization
   * build configuration
   * docs only
   * tests only
3. If a change touches multiple sensitive areas, split it unless the split would become misleading.
4. Prefer this order:

   * docs / planning
   * structure
   * tests
   * tooling
   * optional functional adjustments

---

## Required reporting format for each task

For every completed task or subtask, the agent must report:

* Summary
* Scope
* Files changed
* Behavior change: yes/no
* Public API impact: none/additive/breaking
* Protocol impact: none/potential/confirmed
* Serialization impact: none/potential/confirmed
* Tests run
* Tests recommended
* Risks or follow-up items

Suggested template:

```md
### Task report

- Summary:
- Scope:
- Files changed:
- Behavior change:
- Public API impact:
- Protocol impact:
- Serialization impact:
- Tests run:
- Tests recommended:
- Risks / follow-up:
```

---

## Definition of done per commit

A commit is considered acceptable only if:

* it has a narrow and understandable purpose
* it does not mix unrelated changes
* it updates relevant documentation if responsibilities changed
* it does not introduce unreviewed breaking changes
* it keeps the repository buildable, or clearly explains why the phase is intentionally structural and temporarily incomplete

---

## Recommended commit sequence

The following commit order is recommended:

1. `chore(repo): add plan agents guidance and docs index`
2. `docs(repo): normalize architecture conventions and migration docs`
3. `build(repo): add codex project configuration and central build props`
4. `refactor(src): split protocol serialization and shared abstractions`
5. `test(repo): reorganize tests by purpose and add compatibility scaffolding`
6. `feat(agent): add optional agent tooling and ai-assisted samples`

This is guidance, not a hard requirement.

---

## Risk register

### Risk 1 - Accidental protocol drift

Mitigation:

* isolate protocol documentation early
* require protocol-sensitive review
* add protocol compatibility tests before major changes

### Risk 2 - Accidental serializer behavior drift

Mitigation:

* document serializer invariants
* add serializer compatibility tests
* avoid silent default changes

### Risk 3 - Public API breakage during moves

Mitigation:

* inventory public surface first
* prefer move-only refactors
* report API impact explicitly

### Risk 4 - Documentation drift

Mitigation:

* centralize navigation in `docs/index.md`
* keep `AGENTS.md` minimal
* update docs as part of each structure phase

### Risk 5 - AI-specific concerns leaking into runtime

Mitigation:

* document agent boundary in ADRs
* isolate agent/tooling modules
* treat AI integration as optional

---

## Agent decision rules when uncertain

If uncertain, agents must choose the option that:

1. preserves backward compatibility
2. minimizes protocol risk
3. minimizes serializer risk
4. keeps changes smaller
5. preserves tool-neutral documentation structure
6. avoids mixing AI concerns into core runtime


