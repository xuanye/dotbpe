# .NET Standard 2.0 Compatibility

This document defines the rules for maintaining .NET Standard 2.0 compatibility in the dotbpe repository.

## Rationale

The core dotbpe modules target .NET Standard 2.0 to maximize compatibility across .NET implementations. This allows the framework to run on .NET Framework 4.6.1+, .NET Core 2.0+, and Mono 5.4+.

## Enforceable Rules

### 1. Target Framework Compliance

- You MUST ensure all core modules compile against .NET Standard 2.0.
- You MUST NOT introduce APIs unavailable in .NET Standard 2.0 unless explicitly requested.
- You MUST verify new code compiles for .NET Standard 2.0 before merging.

### 2. Forbidden API Categories

The following API categories are NOT available in .NET Standard 2.0 and MUST NOT be used in core modules:

- **System.Threading.Channels** - Use `System.Collections.Concurrent` collections instead
- **System.Threading.Tasks.Dataflow** - Use external packages or alternative patterns
- **System.IO.Pipelines** - Use classic streams and buffers
- **System.Text.Json** (built-in) - Use Newtonsoft.Json or other compatible serializers
- **Span\<T\> and Memory\<T\>** - Use ArraySegment\<T\> or custom buffers
- **System.Reflection.Metadata** - Use reflection-only load for metadata inspection
- **System.Runtime.Intrinsics** - Use portable algorithm implementations
- **System.DirectoryServices.Protocols** - Use alternative LDAP libraries
- **System.Drawing.Common** - Use ImageSharp or SkiaSharp for cross-platform graphics

### 3. Conditional Compilation

- You MAY use conditional compilation (`#if NETSTANDARD2_0`) for API differences between targets.
- You MUST NOT use conditional compilation to bypass .NET Standard 2.0 compatibility requirements.
- You SHOULD prefer abstraction over conditional compilation where possible.

### 4. NuGet Package Dependencies

- You MUST ensure all NuGet package dependencies support .NET Standard 2.0.
- You MUST NOT add dependencies that require .NET Standard 2.1 or .NET Core 3.0+ without explicit approval.
- You SHOULD minimize the number of dependencies in core modules.

### 5. Platform-Specific Code

- You MAY place platform-specific code in separate assemblies targeting specific frameworks.
- You MUST clearly document which APIs are platform-specific.
- You SHOULD keep platform-specific code minimal and isolated.

## Current Enforcement

These rules are currently enforced through:
- Project file target framework settings (netstandard2.0)
- Manual verification during code review
- No automated compatibility checking tool is currently integrated

## Related Documents

- [Public API Boundary](./public-api-boundary.md)
- [Testing Policy](./testing-policy.md)