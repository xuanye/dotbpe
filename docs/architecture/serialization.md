# Serialization Architecture

## Overview

DotBPE uses a pluggable serialization architecture via the `ISerializer` interface. The serialization format is negotiated per-message using the `CodecType` field in the AMP protocol header (Version 1 only).

## ISerializer Interface

Defined in `src/DotBPE.Abstractions/ISerializer.cs`:

```csharp
public interface ISerializer
{
    T Deserialize<T>(byte[] data);
    byte[] Serialize<T>(T item);
    object Deserialize(byte[] data, Type type);
    byte[] Serialize(object item);
    byte CodecType { get; }
}
```

### Interface Methods

| Method | Purpose |
|--------|---------|
| `Serialize<T>(T item)` | Serialize typed object to bytes |
| `Serialize(object item)` | Serialize untyped object to bytes |
| `Deserialize<T>(byte[] data)` | Deserialize bytes to typed object |
| `Deserialize(byte[] data, Type type)` | Deserialize bytes to object of specified type |
| `CodecType` | Returns the codec type identifier for this serializer |

## CodecType Enum

Defined in `src/DotBPE.Protocol/Codec/CodecType.cs`:

```csharp
public enum CodecType : byte
{
    Protobuf = 0,
    MessagePack = 1,
    JSON = 2,
    Unknown = 9,
}
```

| Value | Name | Description |
|-------|------|-------------|
| 0 | Protobuf | Google Protocol Buffers (default) |
| 1 | MessagePack | MessagePack binary serialization |
| 2 | JSON | JSON text serialization |
| 9 | Unknown | Placeholder/heartbeat codec |

## Supported Serializer Implementations

### Protobuf (CodecType = 0)

**Package**: `DotBPE.Extensions.Protobuf`
**File**: `src/DotBPE.Extensions.Protobuf/ProtobufSerializer.cs`
**Dependency**: Google.Protobuf

- Uses Google.Protobuf's `IMessage` interface
- Requires message types to implement `Google.Protobuf.IMessage`
- Caches `MessageParser` instances per type for performance
- Default codec when using Version 0 protocol

### MessagePack (CodecType = 1)

**Package**: `DotBPE.Extensions.MessagePack`
**File**: `src/DotBPE.Extensions.MessagePack/MessagePackSerializer.cs`
**Dependency**: MessagePack

- Uses MessagePack-CSharp library
- Supports attribute-based contract configuration
- Binary format with compact size

### JSON via System.Text.Json (CodecType = 2)

**Package**: `DotBPE.Extensions.Json`
**File**: `src/DotBPE.Extensions.Json/JsonSerializer.cs`
**Class**: `TextJsonSerializer`
**Dependency**: System.Text.Json (built-in)

- Uses .NET's built-in System.Text.Json
- UTF-8 encoding for wire format
- No external dependencies

### JSON via Newtonsoft.Json (CodecType = 2)

**Package**: `DotBPE.Extensions.JsonNet`
**File**: `src/DotBPE.Extensions.JsonNet/JsonSerializer.cs`
**Class**: `JsonSerializer`
**Dependency**: Newtonsoft.Json

- Uses Newtonsoft.Json (Json.NET)
- UTF-8 encoding for wire format
- More flexible than System.Text.Json

**Note**: Both JSON implementations report `CodecType = 2`. Only one should be registered at a time.

## Serializer Registration

Serializer implementations are registered via dependency injection extension methods:

| Package | Extension Method |
|---------|------------------|
| DotBPE.Extensions.Protobuf | `ServiceCollectionExtensions.AddProtobufSerializer()` |
| DotBPE.Extensions.MessagePack | `ServiceCollectionExtensions.AddMessagePackSerializer()` |
| DotBPE.Extensions.Json | `ServiceCollectionExtensions.AddTextJsonSerializer()` |
| DotBPE.Extensions.JsonNet | `ServiceCollectionExtensions.AddJsonNetSerializer()` |

## Codec Negotiation

### Version 0 Protocol
- Always uses Protobuf
- Codec field is not present in header
- `AmpProtocol.Decode` sets `CodecType = Protobuf` regardless

### Version 1 Protocol
- Codec type is encoded in byte 20 of the header
- During decode, the codec byte is validated against the configured serializer
- Throws `RpcCodecException` if codec mismatch detected

## Usage in Message Flow

### Server-Side Decoding
1. `AmpDecodeHandler.Decode` receives raw bytes
2. Calls `AmpProtocol.Decode((CodecType)_serializer.CodecType, input)`
3. Protocol validates codec type matches configured serializer
4. Returns `AmpMessage` with raw `Data` bytes
5. `ActorCallHandler` deserializes `Data` using `_serializer.Deserialize()`

### Client-Side Encoding
1. `DefaultCallInvoker` serializes request: `_serializer.Serialize(request)`
2. Sets `reqMessage.Data` to serialized bytes
3. `AmpEncodeHandler.Encode` writes header + data to output buffer

## Constraints

- **No mixing**: All services in a deployment must use the same serializer
- **Protobuf requirement**: When using `.proto` file service definitions, only Protobuf is supported
- **Version 0 limitation**: Version 0 protocol only supports Protobuf
- **Single registration**: Only one serializer implementation should be registered per service

## Related Files

| File | Purpose |
|------|---------|
| `src/DotBPE.Abstractions/ISerializer.cs` | Interface definition |
| `src/DotBPE.Protocol/Codec/CodecType.cs` | Enum definition |
| `src/DotBPE.Extensions.Protobuf/ProtobufSerializer.cs` | Protobuf implementation |
| `src/DotBPE.Extensions.MessagePack/MessagePackSerializer.cs` | MessagePack implementation |
| `src/DotBPE.Extensions.Json/JsonSerializer.cs` | System.Text.Json implementation |
| `src/DotBPE.Extensions.JsonNet/JsonSerializer.cs` | Newtonsoft.Json implementation |
| `src/DotBPE.Protocol/Protocols/AmpDecodeHandler.cs` | Decode handler using serializer |
| `src/DotBPE.Protocol/Protocols/AmpEncodeHandler.cs` | Encode handler using serializer |
