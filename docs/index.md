# dotbpe Documentation

Welcome to the dotbpe documentation. This is the central entry point for all repository knowledge.

## Repository Governance

- [AGENTS.md](../AGENTS.md) - Constraints and execution expectations for AI agents
- [PLAN.md](../PLAN.md) - Repository restructuring plan and phased execution strategy

## Getting Started

New to dotbpe? Start with the samples to see the framework in action:

- [MathService.Definition/](../samples/MathService.Definition/) - Shared service definitions
- [MathService/](../samples/MathService/) - Basic RPC service example
- [MathClient/](../samples/MathClient/) - RPC client example
- [MathHttpClient/](../samples/MathHttpClient/) - HTTP client integration
- [GreeterHttpService/](../samples/GreeterHttpService/) - HTTP gateway integration
- [ai-assisted/](../samples/ai-assisted/) - AI-assisted workflow examples (placeholder)

## Architecture

System design, component boundaries, and runtime layering.

- [architecture/](./architecture/) - High-level architecture overview and module responsibilities

## Conventions

Engineering rules, compatibility requirements, and coding standards.

- [conventions/](./conventions/) - Public API boundaries, .NET Standard 2.0 compatibility, protocol safety, and testing policies

## Decisions

Architecture Decision Records (ADRs) documenting significant technical choices.

- [decisions/](./decisions/) - Why certain approaches were chosen and their trade-offs

## Guides

Step-by-step instructions for common development tasks.

- [guides/](./guides/) - How to add services, configure serialization, extend the gateway, and more

## Migration

Notes on transitioning between versions and repository structures.

- [migration/](./migration/) - Breaking changes, upgrade paths, and restructure notes

## Reference

Protocol specifications, API references, and technical details.

- [reference/](./reference/) - Binary protocol layout, message formats, error codes, and serializer contracts
- [amp协议.md](./amp协议.md) - AMP protocol specification (Chinese)
- [protobuf/](./protobuf/) - Protobuf definitions and generated code

## Existing Documentation

- [README.md](../README.md) - Project overview and feature list
