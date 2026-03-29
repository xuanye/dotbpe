# DotBPE Architecture Overview

## Module Structure

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Application Layer                           │
│  (User services, Gateway endpoints, Client proxies)                 │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                         DotBPE.Gateway                              │
│  HTTP Gateway - Maps HTTP endpoints to RPC services                 │
│  - HttpApiCallHandler: Request/response processing                  │
│  - ApiRouteBuilder: Route discovery and registration                │
│  - Swagger: API documentation generation                            │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                         DotBPE.Rpc (Core)                           │
│  RPC Framework Runtime                                              │
│  - Server: Service hosting, actor model, message handling           │
│  - Client: Connection management, routing, call invocation          │
│  References: Abstractions + Protocol                                │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                    ┌───────────────┴───────────────┐
                    ▼                               ▼
┌──────────────────────────────────┐ ┌──────────────────────────────────┐
│       DotBPE.Abstractions        │ │        DotBPE.Protocol           │
│  Shared Contracts & Interfaces   │ │  Binary Protocol Implementation  │
│  - IRpcMessage: Message contract │ │  - AmpMessage: AMP message type  │
│  - ISerializer: Serializer iface │ │  - AmpProtocol: Encode/decode    │
│  - IMethod: Service method def   │ │  - CodecType: Serializer enum    │
│  - IRpcContext: Call context     │ │  - AmpDecodeHandler: Decoder     │
│  - RpcResult: Response wrapper   │ │  - AmpEncodeHandler: Encoder     │
└──────────────────────────────────┘ └──────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                         DotBPE.Baseline                             │
│  Foundation Utilities                                               │
│  - Collections: ObservableDictionary, SettingsDictionary            │
│  - Extensions: String, Task, Collection helpers                     │
│  - Utility: ObjectId, CryptographyManager                           │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                         Peach (External)                            │
│  Socket abstraction layer (DotNetty-based)                          │
└─────────────────────────────────────────────────────────────────────┘
```

## Extension Modules

```
┌─────────────────────────────────────────────────────────────────────┐
│                       DotBPE.Extensions.*                           │
│  Serializer Implementations & Cross-cutting Concerns                │
├─────────────────────────────────────────────────────────────────────┤
│  DotBPE.Extensions.Protobuf    - Protobuf serializer (CodecType=0)  │
│  DotBPE.Extensions.MessagePack - MessagePack serializer (CodecType=1)│
│  DotBPE.Extensions.Json        - System.Text.Json serializer (=2)   │
│  DotBPE.Extensions.JsonNet     - Newtonsoft.Json serializer (=2)    │
│  DotBPE.Extensions.Castle      - AOP interceptors, dynamic proxies  │
└─────────────────────────────────────────────────────────────────────┘
```

## Other Modules

```
┌─────────────────────────────────────────────────────────────────────┐
│  DotBPE.BestPractice  - Best practices and recommended patterns     │
└─────────────────────────────────────────────────────────────────────┘
```

## Dependency Direction

```
Extensions.* ──► Rpc ──► Abstractions
                       ──► Protocol
                       ──► Baseline
                       ──► Peach (external)
Gateway ──► Rpc
BestPractice ──► Rpc
```

- **Extensions depend on Rpc**: Serializer implementations reference `ISerializer` from `DotBPE.Abstractions`
- **Rpc depends on Abstractions**: Uses shared interfaces and contracts
- **Rpc depends on Protocol**: Uses AMP binary protocol for message encoding
- **Rpc depends on Baseline**: Uses utility extensions and helper classes
- **Rpc depends on Peach**: Uses Peach for TCP socket abstraction
- **Gateway depends on Rpc**: Gateway invokes RPC services via client proxies

## Key Abstractions

### Abstractions Layer

| Interface/Class | Location | Purpose |
|-----------------|----------|---------|
| `IRpcMessage` | `src/DotBPE.Abstractions/Protocols/IRpcMessage.cs` | Base message contract |
| `ISerializer` | `src/DotBPE.Abstractions/ISerializer.cs` | Serializer abstraction |
| `IMethod` | `src/DotBPE.Abstractions/IMethod.cs` | Service method definition |
| `IRpcContext` | `src/DotBPE.Abstractions/IRpcContext.cs` | RPC call context |
| `RpcResult` | `src/DotBPE.Abstractions/RpcResult.cs` | Response wrapper |

### Protocol Layer

| Interface/Class | Location | Purpose |
|-----------------|----------|---------|
| `AmpMessage` | `src/DotBPE.Protocol/Protocols/AmpMessage.cs` | AMP protocol message implementation |
| `AmpProtocol` | `src/DotBPE.Protocol/Protocols/AmpProtocol.cs` | Static encode/decode methods |
| `CodecType` | `src/DotBPE.Protocol/Codec/CodecType.cs` | Serializer type enum |
| `AmpDecodeHandler` | `src/DotBPE.Protocol/Protocols/AmpDecodeHandler.cs` | Byte-to-message decoder |
| `AmpEncodeHandler` | `src/DotBPE.Protocol/Protocols/AmpEncodeHandler.cs` | Message-to-byte encoder |

### Server Layer (in DotBPE.Rpc)

| Interface/Class | Location | Purpose |
|-----------------|----------|---------|
| `IServiceActor` | `src/DotBPE.Rpc/Server/IServiceActor.cs` | Service actor marker interface |
| `IServiceActorHandler` | `src/DotBPE.Rpc/Server/IServiceActorHandler.cs` | Handles incoming messages |
| `IServiceActorLocator` | `src/DotBPE.Rpc/Server/IServiceActorLocator.cs` | Locates service actors by ID |
| `IMessageHandler` | `src/DotBPE.Rpc/Server/IMessageHandler.cs` | Server-side message dispatch |
| `IServerHost` | `src/DotBPE.Rpc/Server/IServerHost.cs` | Server lifecycle management |

### Client Layer (in DotBPE.Rpc)

| Interface/Class | Location | Purpose |
|-----------------|----------|---------|
| `IRpcClient` | `src/DotBPE.Rpc/Client/IRpcClient.cs` | Client send/close operations |
| `ICallInvoker` | `src/DotBPE.Rpc/Client/ICallInvoker.cs` | Typed RPC call invocation |
| `IClientProxy` | `src/DotBPE.Rpc/Client/IClientProxy.cs` | Service proxy creation |
| `ITransport` | `src/DotBPE.Rpc/Client/ITransport.cs` | Connection abstraction |
| `IServiceRouter` | `src/DotBPE.Rpc/Client/IServiceRouter.cs` | Service endpoint routing |
| `IRoutingPolicy` | `src/DotBPE.Rpc/Client/IRoutingPolicy.cs` | Load balancing strategy |

## Module Responsibilities

### DotBPE.Abstractions
Shared contracts and interfaces used across the framework. Defines `IRpcMessage`, `ISerializer`, `IMethod`, `IRpcContext`, and `RpcResult`. This module has no dependencies on other DotBPE modules.

### DotBPE.Protocol
Binary protocol implementation. Contains AMP message types, encode/decode handlers, codec type definitions, and protocol error codes. Depends only on Abstractions.

### DotBPE.Baseline
Foundation utilities with no business logic dependencies. Provides extension methods for common types, collection helpers, and utility classes like `ObjectId` generation.

### DotBPE.Rpc
Core RPC framework runtime containing:
- **Server**: Service hosting, actor model for request handling, service discovery
- **Client**: Connection pooling, transport management, routing, call invocation
- References Abstractions for interfaces and Protocol for binary encoding

### DotBPE.Gateway
Lightweight HTTP gateway that maps HTTP API endpoints to RPC services. Supports:
- Route pattern discovery via attributes
- Request/response serialization
- Swagger documentation generation
- Plugin architecture for request/response processing

### DotBPE.Extensions.*
Extension packages providing concrete implementations:
- **Serializer implementations**: Protobuf, MessagePack, JSON variants
- **AOP support**: Castle.DynamicProxy-based interceptors

### DotBPE.BestPractice
Best practices and recommended patterns for using the framework.

## Service Invocation Flow

### Server-Side
1. `PeachServerBootstrap` accepts TCP connections
2. `AmpDecodeHandler` decodes raw bytes to `AmpMessage`
3. `DefaultMessageHandler` dispatches to appropriate `IServiceActorHandler`
4. `ServiceActorHandler` invokes the service method
5. `AmpEncodeHandler` encodes response back to bytes

### Client-Side
1. `ICallInvoker.InvokerAsync` creates `AmpMessage` request
2. `IServiceRouter` resolves endpoint via `IRoutingPolicy`
3. `ITransportFactory` creates/reuses transport connection
4. `IRpcClient.SendAsync` sends message over transport
5. Response is matched by sequence number and returned
