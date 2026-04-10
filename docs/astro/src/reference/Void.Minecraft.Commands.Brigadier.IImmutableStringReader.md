# <a id="Void_Minecraft_Commands_Brigadier_IImmutableStringReader"></a> Interface IImmutableStringReader

Namespace: [Void.Minecraft.Commands.Brigadier](Void.Minecraft.Commands.Brigadier.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IImmutableStringReader
```

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_IImmutableStringReader_CanRead"></a> CanRead

```csharp
bool CanRead { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_IImmutableStringReader_Cursor"></a> Cursor

```csharp
int Cursor { get; set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_IImmutableStringReader_Peek"></a> Peek

```csharp
char Peek { get; }
```

#### Property Value

 [char](https://learn.microsoft.com/dotnet/api/system.char)

### <a id="Void_Minecraft_Commands_Brigadier_IImmutableStringReader_Read"></a> Read

```csharp
string Read { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_IImmutableStringReader_Remaining"></a> Remaining

```csharp
string Remaining { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_IImmutableStringReader_RemainingLength"></a> RemainingLength

```csharp
int RemainingLength { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_IImmutableStringReader_Source"></a> Source

```csharp
string Source { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_IImmutableStringReader_TotalLength"></a> TotalLength

```csharp
int TotalLength { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_IImmutableStringReader_CanReadLength_System_Int32_"></a> CanReadLength\(int\)

```csharp
bool CanReadLength(int length)
```

#### Parameters

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_IImmutableStringReader_PeekAt_System_Int32_"></a> PeekAt\(int\)

```csharp
char PeekAt(int offset)
```

#### Parameters

`offset` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [char](https://learn.microsoft.com/dotnet/api/system.char)

