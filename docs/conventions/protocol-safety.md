# Protocol Safety

This document defines the rules for maintaining binary protocol stability in the dotbpe repository.

## Rationale

The dotbpe binary protocol (Amp protocol) defines the wire format for communication between clients and servers. Clients and servers built against different versions must be able to interoperate. Protocol changes require careful consideration and explicit approval.

## Current Protocol Layout

The current Amp protocol message header structure:

```
0        1 2 3 4   5 6 7 8     9     10 11 12 13   1415      16171819    20    <length>-21
+------------+----------+---------+------+-------------+---------+---------+--------+------------+
| <ver/argc> | <length> |  <seq>  |<type>| <serviceId> | <msgId> |  <code> | <codec>|   <data>   |
+------------+----------+---------+------+-------------+---------+---------+--------+------------+
```

- **ver/argc**: Version field, fixed value 1
- **length**: Total packet length
- **seq**: Request sequence number
- **type**: Message type (1=Request, 2=Response, 3=Notify, 4=InvokeWithoutResponse)
- **serviceId**: Service identifier
- **msgId**: Message identifier
- **code**: Response code (0 for requests, error code for responses)
- **codec**: Codec type (0=Protobuf, 1=MessagePack, 2=JSON)
- **data**: Actual payload data

## Enforceable Rules

### 1. Header Structure Stability

- You MUST NOT change the byte offset or length of any header field without explicit approval.
- You MUST NOT add, remove, or reorder header fields without explicit approval.
- You MUST NOT change the meaning of existing header field values without explicit approval.

### 2. Message Type and Code Values

- You MUST NOT change the numeric values of existing message types without explicit approval.
- You MUST NOT change the numeric values of existing response codes without explicit approval.
- You MAY add new message types or response codes as additive changes.

### 3. Codec Type Values

- You MUST NOT change the numeric values of existing CodecType enumerations.
- You MUST NOT change the default serializer behavior without explicit approval.
- You MAY add new codec types as additive changes.

### 4. Review Requirements for Protocol Changes

- You MUST document protocol changes in `docs/migration/` with version information.
- You MUST provide backward compatibility or a clear migration path for protocol changes.
- You SHOULD add protocol compatibility tests when making protocol-affecting changes.

### 5. Protocol Documentation

- You MUST ensure protocol documentation reflects current behavior, not intended future behavior.
- You MUST NOT claim protocol changes that have not been implemented.
- You SHOULD include protocol version information in documentation.

## Current Enforcement

These rules are currently enforced through:
- Code review requiring explicit justification for protocol changes
- No automated protocol validation is currently integrated
- Developers must manually verify protocol compatibility

## Related Documents

- [Serializer Safety](./serializer-safety.md)
- [Testing Policy](./testing-policy.md)