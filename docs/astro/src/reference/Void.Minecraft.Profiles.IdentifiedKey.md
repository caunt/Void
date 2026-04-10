# <a id="Void_Minecraft_Profiles_IdentifiedKey"></a> Class IdentifiedKey

Namespace: [Void.Minecraft.Profiles](Void.Minecraft.Profiles.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record IdentifiedKey : IEquatable<IdentifiedKey>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[IdentifiedKey](Void.Minecraft.Profiles.IdentifiedKey.md)

#### Implements

[IEquatable<IdentifiedKey\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Profiles_IdentifiedKey__ctor_Void_Minecraft_Profiles_IdentifiedKeyRevision_System_Int64_System_Byte___System_Byte___"></a> IdentifiedKey\(IdentifiedKeyRevision, long, byte\[\], byte\[\]\)

```csharp
public IdentifiedKey(IdentifiedKeyRevision Revision, long ExpiresAt, byte[] PublicKey, byte[] Signature)
```

#### Parameters

`Revision` [IdentifiedKeyRevision](Void.Minecraft.Profiles.IdentifiedKeyRevision.md)

`ExpiresAt` [long](https://learn.microsoft.com/dotnet/api/system.int64)

`PublicKey` [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

`Signature` [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

## Fields

### <a id="Void_Minecraft_Profiles_IdentifiedKey_YggdrasilSessionPublicKey"></a> YggdrasilSessionPublicKey

```csharp
public static readonly byte[] YggdrasilSessionPublicKey
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

## Properties

### <a id="Void_Minecraft_Profiles_IdentifiedKey_ExpiresAt"></a> ExpiresAt

```csharp
public long ExpiresAt { get; init; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Profiles_IdentifiedKey_IsSignatureValid"></a> IsSignatureValid

```csharp
public bool IsSignatureValid { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Profiles_IdentifiedKey_ProfileUuid"></a> ProfileUuid

```csharp
public Uuid ProfileUuid { get; set; }
```

#### Property Value

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

### <a id="Void_Minecraft_Profiles_IdentifiedKey_PublicKey"></a> PublicKey

```csharp
public byte[] PublicKey { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

### <a id="Void_Minecraft_Profiles_IdentifiedKey_Revision"></a> Revision

```csharp
public IdentifiedKeyRevision Revision { get; init; }
```

#### Property Value

 [IdentifiedKeyRevision](Void.Minecraft.Profiles.IdentifiedKeyRevision.md)

### <a id="Void_Minecraft_Profiles_IdentifiedKey_Signature"></a> Signature

```csharp
public byte[] Signature { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

## Methods

### <a id="Void_Minecraft_Profiles_IdentifiedKey_AddUuid_Void_Minecraft_Profiles_Uuid_"></a> AddUuid\(Uuid\)

```csharp
public bool AddUuid(Uuid uuid)
```

#### Parameters

`uuid` [Uuid](Void.Minecraft.Profiles.Uuid.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Profiles_IdentifiedKey_VerifyDataSignature_System_ReadOnlySpan_System_Byte__System_ReadOnlySpan_System_Byte__"></a> VerifyDataSignature\(ReadOnlySpan<byte\>, params ReadOnlySpan<byte\>\)

```csharp
public bool VerifyDataSignature(ReadOnlySpan<byte> signature, params ReadOnlySpan<byte> data)
```

#### Parameters

`signature` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

`data` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

