# <a id="Void_Minecraft_Nbt_NumberExtensions"></a> Class NumberExtensions

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Contains extension methods dealing with endianness of numeric types.

```csharp
public static class NumberExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NumberExtensions](Void.Minecraft.Nbt.NumberExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Nbt_NumberExtensions_IsValidUnquoted_System_Char_"></a> IsValidUnquoted\(char\)

```csharp
public static bool IsValidUnquoted(this char c)
```

#### Parameters

`c` [char](https://learn.microsoft.com/dotnet/api/system.char)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Nbt_NumberExtensions_SwapEndian_System_Int16_"></a> SwapEndian\(short\)

Swap the endian of the given <code class="paramref">value</code>.

```csharp
public static short SwapEndian(this short value)
```

#### Parameters

`value` [short](https://learn.microsoft.com/dotnet/api/system.int16)

The value to swap endian of.

#### Returns

 [short](https://learn.microsoft.com/dotnet/api/system.int16)

The value with bytes in opposite format.

### <a id="Void_Minecraft_Nbt_NumberExtensions_SwapEndian_System_UInt16_"></a> SwapEndian\(ushort\)

Swap the endian of the given <code class="paramref">value</code>.

```csharp
public static ushort SwapEndian(this ushort value)
```

#### Parameters

`value` [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

The value to swap endian of.

#### Returns

 [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

The value with bytes in opposite format.

### <a id="Void_Minecraft_Nbt_NumberExtensions_SwapEndian_System_Int32_"></a> SwapEndian\(int\)

Swap the endian of the given <code class="paramref">value</code>.

```csharp
public static int SwapEndian(this int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The value to swap endian of.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

The value with bytes in opposite format.

### <a id="Void_Minecraft_Nbt_NumberExtensions_SwapEndian_System_UInt32_"></a> SwapEndian\(uint\)

Swap the endian of the given <code class="paramref">value</code>.

```csharp
public static uint SwapEndian(this uint value)
```

#### Parameters

`value` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The value to swap endian of.

#### Returns

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The value with bytes in opposite format.

### <a id="Void_Minecraft_Nbt_NumberExtensions_SwapEndian_System_UInt64_"></a> SwapEndian\(ulong\)

Swap the endian of the given <code class="paramref">value</code>.

```csharp
public static ulong SwapEndian(this ulong value)
```

#### Parameters

`value` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The value to swap endian of.

#### Returns

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The value with bytes in opposite format.

### <a id="Void_Minecraft_Nbt_NumberExtensions_SwapEndian_System_Int64_"></a> SwapEndian\(long\)

Swap the endian of the given <code class="paramref">value</code>.

```csharp
public static long SwapEndian(this long value)
```

#### Parameters

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The value to swap endian of.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

The value with bytes in opposite format.

### <a id="Void_Minecraft_Nbt_NumberExtensions_SwapEndian_System_Single_"></a> SwapEndian\(float\)

Swap the endian of the given <code class="paramref">value</code>.

```csharp
public static float SwapEndian(this float value)
```

#### Parameters

`value` [float](https://learn.microsoft.com/dotnet/api/system.single)

The value to swap endian of.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

The value with bytes in opposite format.

### <a id="Void_Minecraft_Nbt_NumberExtensions_SwapEndian_System_Double_"></a> SwapEndian\(double\)

Swap the endian of the given <code class="paramref">value</code>.

```csharp
public static double SwapEndian(this double value)
```

#### Parameters

`value` [double](https://learn.microsoft.com/dotnet/api/system.double)

The value to swap endian of.

#### Returns

 [double](https://learn.microsoft.com/dotnet/api/system.double)

The value with bytes in opposite format.

