# <a id="Void_Minecraft_Nbt_TagIO"></a> Class TagIO

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Abstract base class for the <xref href="Void.Minecraft.Nbt.TagReader" data-throw-if-not-resolved="false"></xref> and <xref href="Void.Minecraft.Nbt.TagWriter" data-throw-if-not-resolved="false"></xref> classes, providing shared functionality.

```csharp
public abstract class TagIO : IDisposable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TagIO](Void.Minecraft.Nbt.TagIO.md)

#### Derived

[TagReader](Void.Minecraft.Nbt.TagReader.md), 
[TagWriter](Void.Minecraft.Nbt.TagWriter.md)

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Nbt_TagIO__ctor_System_IO_Stream_Void_Minecraft_Nbt_FormatOptions_"></a> TagIO\(Stream, FormatOptions\)

Initializes a new instance of the <xref href="Void.Minecraft.Nbt.TagIO" data-throw-if-not-resolved="false"></xref> class.

```csharp
protected TagIO(Stream stream, FormatOptions options)
```

#### Parameters

`stream` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

A <xref href="System.IO.Stream" data-throw-if-not-resolved="false"></xref> instance that the writer will be writing to.

`options` [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

Bitwise flags to configure how data should be handled for compatibility between different specifications.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

Thrown when <code class="paramref">stream</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>

## Properties

### <a id="Void_Minecraft_Nbt_TagIO_BaseStream"></a> BaseStream

Gets the underlying stream this instance is operating on.

```csharp
protected Stream BaseStream { get; }
```

#### Property Value

 [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

### <a id="Void_Minecraft_Nbt_TagIO_FormatOptions"></a> FormatOptions

Gets the format to be followed for compatibility.

```csharp
public FormatOptions FormatOptions { get; }
```

#### Property Value

 [FormatOptions](Void.Minecraft.Nbt.FormatOptions.md)

### <a id="Void_Minecraft_Nbt_TagIO_SwapEndian"></a> SwapEndian

Gets a flag indicating if byte swapping is required for numeric values, accounting for both the endianness of the host machine and the
specified <xref href="Void.Minecraft.Nbt.TagIO.FormatOptions" data-throw-if-not-resolved="false"></xref>.

```csharp
protected bool SwapEndian { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Nbt_TagIO_UseVarInt"></a> UseVarInt

Gets a flag indicating if variable-length integers should be used in applicable places.

```csharp
protected bool UseVarInt { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Nbt_TagIO_ZigZagEncoding"></a> ZigZagEncoding

Gets a flag indicating if variable-length integers will be "ZigZag encoded".

```csharp
public bool ZigZagEncoding { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Methods

### <a id="Void_Minecraft_Nbt_TagIO_Dispose"></a> Dispose\(\)

Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

```csharp
public abstract void Dispose()
```

### <a id="Void_Minecraft_Nbt_TagIO_DisposeAsync"></a> DisposeAsync\(\)

Asynchronously releases the unmanaged resources used by the <xref href="Void.Minecraft.Nbt.TagIO" data-throw-if-not-resolved="false"></xref> instance.

```csharp
public abstract ValueTask DisposeAsync()
```

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

