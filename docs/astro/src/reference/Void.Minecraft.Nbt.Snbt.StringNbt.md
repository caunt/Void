# <a id="Void_Minecraft_Nbt_Snbt_StringNbt"></a> Class StringNbt

Namespace: [Void.Minecraft.Nbt.Snbt](Void.Minecraft.Nbt.Snbt.md)  
Assembly: Void.Minecraft.dll  

Provides static methods for parsing string-NBT (SNBT) source text into a complete <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public static class StringNbt
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[StringNbt](Void.Minecraft.Nbt.Snbt.StringNbt.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Nbt_Snbt_StringNbt_Parse_System_String_"></a> Parse\(string\)

Parse the given <code class="paramref">source</code> text into a <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public static CompoundTag Parse(string source)
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

A string containing the SNBT code to parse.

#### Returns

 [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

The <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance described in the source text.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

When <code class="paramref">source</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

 [SyntaxErrorException](https://learn.microsoft.com/dotnet/api/system.data.syntaxerrorexception)

When <code class="paramref">source</code> is invalid SNBT code.

### <a id="Void_Minecraft_Nbt_Snbt_StringNbt_Parse_System_ReadOnlySpan_System_Byte__System_Text_Encoding_"></a> Parse\(ReadOnlySpan<byte\>, Encoding?\)

Parse the given <code class="paramref">source</code> text into a <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public static CompoundTag Parse(ReadOnlySpan<byte> source, Encoding? encoding = null)
```

#### Parameters

`source` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A string containing the SNBT code to parse.

`encoding` [Encoding](https://learn.microsoft.com/dotnet/api/system.text.encoding)?

Encoding of the <code class="paramref">source</code>.

#### Returns

 [CompoundTag](Void.Minecraft.Nbt.CompoundTag.md)

The <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance described in the source text.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

When <code class="paramref">source</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

 [SyntaxErrorException](https://learn.microsoft.com/dotnet/api/system.data.syntaxerrorexception)

When <code class="paramref">source</code> is invalid SNBT code.

### <a id="Void_Minecraft_Nbt_Snbt_StringNbt_ParseList_System_String_"></a> ParseList\(string\)

Parse the given <code class="paramref">source</code> text into a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public static ListTag ParseList(string source)
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

A string containing the SNBT code to parse.

#### Returns

 [ListTag](Void.Minecraft.Nbt.ListTag.md)

The <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance described in the source text.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

When <code class="paramref">source</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

 [SyntaxErrorException](https://learn.microsoft.com/dotnet/api/system.data.syntaxerrorexception)

When <code class="paramref">source</code> is invalid SNBT code.

### <a id="Void_Minecraft_Nbt_Snbt_StringNbt_ParseList_System_ReadOnlySpan_System_Byte__System_Text_Encoding_"></a> ParseList\(ReadOnlySpan<byte\>, Encoding?\)

Parse the given <code class="paramref">source</code> text into a <xref href="Void.Minecraft.Nbt.ListTag" data-throw-if-not-resolved="false"></xref>.

```csharp
public static ListTag ParseList(ReadOnlySpan<byte> source, Encoding? encoding = null)
```

#### Parameters

`source` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

A string containing the SNBT code to parse.

`encoding` [Encoding](https://learn.microsoft.com/dotnet/api/system.text.encoding)?

Encoding of the <code class="paramref">source</code>.

#### Returns

 [ListTag](Void.Minecraft.Nbt.ListTag.md)

The <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref> instance described in the source text.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

When <code class="paramref">source</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

 [SyntaxErrorException](https://learn.microsoft.com/dotnet/api/system.data.syntaxerrorexception)

When <code class="paramref">source</code> is invalid SNBT code.

