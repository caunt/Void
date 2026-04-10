# <a id="Void_Minecraft_Commands_Brigadier_Context_StringRange"></a> Class StringRange

Namespace: [Void.Minecraft.Commands.Brigadier.Context](Void.Minecraft.Commands.Brigadier.Context.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record StringRange : IEquatable<StringRange>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

#### Implements

[IEquatable<StringRange\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Context_StringRange__ctor_System_Int32_System_Int32_"></a> StringRange\(int, int\)

```csharp
public StringRange(int Start, int End)
```

#### Parameters

`Start` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`End` [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Context_StringRange_End"></a> End

```csharp
public int End { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_Context_StringRange_IsEmpty"></a> IsEmpty

```csharp
public bool IsEmpty { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_Context_StringRange_Length"></a> Length

```csharp
public int Length { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_Context_StringRange_Start"></a> Start

```csharp
public int Start { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Context_StringRange_At_System_Int32_"></a> At\(int\)

```csharp
public static StringRange At(int pos)
```

#### Parameters

`pos` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_StringRange_Between_System_Int32_System_Int32_"></a> Between\(int, int\)

```csharp
public static StringRange Between(int start, int end)
```

#### Parameters

`start` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`end` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_StringRange_Encompassing_Void_Minecraft_Commands_Brigadier_Context_StringRange_Void_Minecraft_Commands_Brigadier_Context_StringRange_"></a> Encompassing\(StringRange, StringRange\)

```csharp
public static StringRange Encompassing(StringRange a, StringRange b)
```

#### Parameters

`a` [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

`b` [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

#### Returns

 [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_StringRange_Get_Void_Minecraft_Commands_Brigadier_IImmutableStringReader_"></a> Get\(IImmutableStringReader\)

```csharp
public string Get(IImmutableStringReader reader)
```

#### Parameters

`reader` [IImmutableStringReader](Void.Minecraft.Commands.Brigadier.IImmutableStringReader.md)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Context_StringRange_Get_System_String_"></a> Get\(string\)

```csharp
public string Get(string value)
```

#### Parameters

`value` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

