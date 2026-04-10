# <a id="Void_Minecraft_Buffers_Extensions_IntExtensions"></a> Class IntExtensions

Namespace: [Void.Minecraft.Buffers.Extensions](Void.Minecraft.Buffers.Extensions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class IntExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[IntExtensions](Void.Minecraft.Buffers.Extensions.IntExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Buffers_Extensions_IntExtensions_AsVarInt_System_Int32_"></a> AsVarInt\(int\)

```csharp
public static byte[] AsVarInt(this int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

### <a id="Void_Minecraft_Buffers_Extensions_IntExtensions_AsVarInt_System_Int32_System_Span_System_Byte__"></a> AsVarInt\(int, Span<byte\>\)

```csharp
public static int AsVarInt(this int value, Span<byte> buffer)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`buffer` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Buffers_Extensions_IntExtensions_EnumerateVarInt_System_Int32_"></a> EnumerateVarInt\(int\)

```csharp
[Obsolete("Use AsVarInt instead.")]
public static IEnumerable<byte> EnumerateVarInt(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

### <a id="Void_Minecraft_Buffers_Extensions_IntExtensions_VarIntSize_System_Int32_"></a> VarIntSize\(int\)

```csharp
public static int VarIntSize(this int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

