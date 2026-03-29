# AMP Protocol Specification

## Overview

AMP (A Message Protocol) is the binary protocol used by DotBPE for RPC communication. It has two versions with different header sizes.

## Version 0 (Legacy)

### Binary Layout

```
      0        1 2 3 4   5 6 7 8     9     10 11     12 13   14151617 <length>-18
+------------+----------+---------+------+-------+---------+---------+------------+
| <ver/argc> | <length> |  <seq>  |<type>|<serId>| <msgId> |  <code> |   <data>   |
+------------+----------+---------+------+-------+---------+---------+------------+
```

**Header Length**: 18 bytes (`VERSION_0_HEAD_LENGTH`)

### Field Definitions

| Offset | Size | Field | Description |
|--------|------|-------|-------------|
| 0 | 1 byte | ver/argc | Version, fixed to 0 |
| 1 | 4 bytes | length | Total packet length (big-endian int) |
| 5 | 4 bytes | seq | Request sequence number (big-endian int) |
| 9 | 1 byte | type | Message type |
| 10 | 2 bytes | serId | Service ID (big-endian ushort) |
| 12 | 2 bytes | msgId | Message ID (big-endian ushort) |
| 14 | 4 bytes | code | Response status code |
| 18 | variable | data | Business payload |

### Message Types

| Value | Name | Description |
|-------|------|-------------|
| 1 | Request | Client request message |
| 2 | Response | Server response message |
| 3 | Notify | One-way notification (no response expected) |

### Codec

Version 0 always uses **Protobuf** serialization.

---

## Version 1 (Current)

### Binary Layout

```
      0        1 2 3 4   5 6 7 8     9     10 11 12 13   1415      16171819    20    <length>-21
+------------+----------+---------+------+-------------+---------+---------+--------+------------+
| <ver/argc> | <length> |  <seq>  |<type>| <serviceId> | <msgId> |  <code> | <codec>|   <data>   |
+------------+----------+---------+------+-------------+---------+---------+--------+------------+
```

**Header Length**: 21 bytes (`VERSION_1_HEAD_LENGTH`)

### Field Definitions

| Offset | Size | Field | Description |
|--------|------|-------|-------------|
| 0 | 1 byte | ver/argc | Version, fixed to 1 |
| 1 | 4 bytes | length | Total packet length (big-endian int) |
| 5 | 4 bytes | seq | Request sequence number (big-endian int) |
| 9 | 1 byte | type | Message type |
| 10 | 4 bytes | serviceId | Service ID (big-endian int) |
| 14 | 2 bytes | msgId | Message ID (big-endian ushort) |
| 16 | 4 bytes | code | Response status code |
| 20 | 1 byte | codec | Codec type identifier |
| 21 | variable | data | Business payload |

### Message Types

| Value | Name | Description |
|-------|------|-------------|
| 1 | Request | Client request message |
| 2 | Response | Server response message |
| 3 | Notify | One-way notification (no response expected) |
| 4 | InvokeWithoutResponse | One-way request (fire-and-forget) |

### Codec Types

| Value | Name | Description |
|-------|------|-------------|
| 0 | Protobuf | Google Protocol Buffers (default) |
| 1 | MessagePack | MessagePack binary format |
| 2 | JSON | JSON text format |

---

## Key Differences Between Versions

| Aspect | Version 0 | Version 1 |
|--------|-----------|-----------|
| Header Size | 18 bytes | 21 bytes |
| ServiceId Size | 2 bytes (ushort) | 4 bytes (int) |
| Codec Selection | Fixed (Protobuf) | Configurable via codec field |
| Message Types | 3 types | 4 types (added InvokeWithoutResponse) |

## Protocol Constants

Defined in `src/DotBPE.Rpc/Protocols/AmpMessage.cs`:

```csharp
public const int VERSION_0_HEAD_LENGTH = 18;
public const int VERSION_1_HEAD_LENGTH = 21;
```

Defined in `src/DotBPE.Rpc/Protocols/AmpProtocol.cs`:

```csharp
public const int INITIAL_BYTES_TO_STRIP = 0;
public const int LENGTH_ADJUSTMENT = -5;
public const int LENGTH_FIELD_LENGTH = 4;
public const int LENGTH_FIELD_OFFSET = 1;
public const int MAX_FRAME_LENGTH = int.MaxValue;
public const int HEARTBEAT_INTERVAL = 15; // seconds
```

## Heartbeat

Heartbeat messages are identified by `ServiceId == 0 && MessageId == 0`. The client sends a heartbeat if no new message is sent within `HEARTBEAT_INTERVAL` (15 seconds). The server timeout is `HEARTBEAT_INTERVAL * 2` (30 seconds).

## Status Codes

Defined in `src/DotBPE.Rpc/Protocols/RpcStatusCodes.cs`:

| Code | Name | Description |
|------|------|-------------|
| 0 | CODE_SUCCESS | Success |
| -10242500 | CODE_INTERNAL_ERROR | Internal server error |
| -10242403 | CODE_ACCESS_DENIED | Access denied |
| -10242404 | CODE_SERVICE_NOT_FOUND | Service not found |
| -10242409 | CODE_RATE_LIMITED | Rate limited |
| -10242504 | CODE_TIMEOUT | Request timeout |

## Framing

The protocol uses DotNetty's `LengthFieldBasedFrameDecoder` for message framing:

- **Length field offset**: 1 (byte 1)
- **Length field size**: 4 bytes
- **Length adjustment**: -5 (subtract bytes before length field + length field itself)
- **Initial bytes to strip**: 0

This ensures complete messages are reassembled before decoding.
