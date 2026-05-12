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

Encodes <code class="paramref">value</code> into a new byte array using the Minecraft VarInt format.

```csharp
public static byte[] AsVarInt(this int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The 32-bit signed value to encode.

#### Returns

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

A new array containing only the encoded VarInt bytes (length from <code>1</code> to <code>5</code>).

#### Examples

<pre><code class="lang-csharp">byte[] payload = 300.AsVarInt();
// payload can be used as a compact packet field representation.</code></pre>

#### Remarks

<p>
This method allocates a new array for every call and copies the encoded payload from an internal
stack buffer.
</p>
<p>
Negative values are encoded from their two's-complement bit pattern and therefore produce <code>5</code> bytes.
</p>

#### See Also

[IntExtensions](Void.Minecraft.Buffers.Extensions.IntExtensions.md).[VarIntSize](Void.Minecraft.Buffers.Extensions.IntExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_IntExtensions\_VarIntSize\_System\_Int32\_)\([int](https://learn.microsoft.com/dotnet/api/system.int32)\)

### <a id="Void_Minecraft_Buffers_Extensions_IntExtensions_AsVarInt_System_Int32_System_Span_System_Byte__"></a> AsVarInt\(int, Span<byte\>\)

Encodes <code class="paramref">value</code> as a Minecraft VarInt into <code class="paramref">buffer</code>.

```csharp
public static int AsVarInt(this int value, Span<byte> buffer)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The 32-bit signed value to encode.

`buffer` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The destination span that receives encoded bytes starting at index <code>0</code>.
The span must be large enough for the encoded value.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

The number of bytes written to <code class="paramref">buffer</code> (from <code>1</code> to <code>5</code>).

#### Examples

<pre><code class="lang-csharp">Span&lt;byte&gt; bytes = stackalloc byte[5];
var length = 300.AsVarInt(bytes);
// bytes[..length] now contains the VarInt payload.</code></pre>

#### Remarks

<p>
Encoding uses the standard 7-bit continuation format used by Minecraft packets.
Negative values are encoded from their two's-complement bit pattern and therefore use <code>5</code> bytes.
</p>
<p>
The method writes only the returned byte count and does not clear any remaining bytes in
<code class="paramref">buffer</code>.
</p>

#### Exceptions

 [IndexOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.indexoutofrangeexception)

Thrown when <code class="paramref">buffer</code> is too small for the encoded value.

#### See Also

[IntExtensions](Void.Minecraft.Buffers.Extensions.IntExtensions.md).[VarIntSize](Void.Minecraft.Buffers.Extensions.IntExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_IntExtensions\_VarIntSize\_System\_Int32\_)\([int](https://learn.microsoft.com/dotnet/api/system.int32)\)

### <a id="Void_Minecraft_Buffers_Extensions_IntExtensions_EnumerateVarInt_System_Int32_"></a> EnumerateVarInt\(int\)

Lazily enumerates bytes of <code class="paramref">value</code> encoded as a Minecraft VarInt.

```csharp
[Obsolete("Use AsVarInt instead.")]
public static IEnumerable<byte> EnumerateVarInt(int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The 32-bit signed integer to encode.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

An <xref href="System.Collections.Generic.IEnumerable%601" data-throw-if-not-resolved="false"></xref> that yields encoded bytes in wire order, from first to last byte.

#### Examples

<pre><code class="lang-csharp">foreach (var b in IntExtensions.EnumerateVarInt(300))
{
    // consume each encoded VarInt byte
}</code></pre>

#### Remarks

<p>
This API is obsolete; prefer <xref href="Void.Minecraft.Buffers.Extensions.IntExtensions.AsVarInt(System.Int32)" data-throw-if-not-resolved="false"></xref> or <xref href="Void.Minecraft.Buffers.Extensions.IntExtensions.AsVarInt(System.Int32%2cSystem.Span%7bSystem.Byte%7d)" data-throw-if-not-resolved="false"></xref> for
clearer ownership and lower overhead.
</p>
<p>
Enumeration executes the encoding loop on demand each time the sequence is iterated. The sequence length is
from <code>1</code> to <code>5</code> bytes, and negative values produce <code>5</code> bytes.
</p>

#### See Also

[IntExtensions](Void.Minecraft.Buffers.Extensions.IntExtensions.md).[VarIntSize](Void.Minecraft.Buffers.Extensions.IntExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_IntExtensions\_VarIntSize\_System\_Int32\_)\([int](https://learn.microsoft.com/dotnet/api/system.int32)\), 
[IntExtensions](Void.Minecraft.Buffers.Extensions.IntExtensions.md).[AsVarInt](Void.Minecraft.Buffers.Extensions.IntExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_IntExtensions\_AsVarInt\_System\_Int32\_)\([int](https://learn.microsoft.com/dotnet/api/system.int32)\)

### <a id="Void_Minecraft_Buffers_Extensions_IntExtensions_VarIntSize_System_Int32_"></a> VarIntSize\(int\)

Computes the number of bytes required to encode <code class="paramref">value</code> as a Minecraft VarInt.

```csharp
public static int VarIntSize(this int value)
```

#### Parameters

`value` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The 32-bit signed integer to evaluate.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

The encoded VarInt length in bytes, always between <code>1</code> and <code>5</code>.

#### Examples

<pre><code class="lang-csharp">var idLength = packetId.VarIntSize();
binaryMessage.Stream.Position = idLength;</code></pre>

#### Remarks

<p>
This method performs a branch-free bit operation and does not allocate.
</p>
<p>
The result matches the byte count written by <xref href="Void.Minecraft.Buffers.Extensions.IntExtensions.AsVarInt(System.Int32%2cSystem.Span%7bSystem.Byte%7d)" data-throw-if-not-resolved="false"></xref> for the same value,
which allows callers to precompute packet offsets and payload lengths.
</p>
<p>
Because Minecraft VarInt uses the two's-complement bit pattern of signed integers, negative values always
require <code>5</code> bytes.
</p>

#### See Also

[IntExtensions](Void.Minecraft.Buffers.Extensions.IntExtensions.md).[AsVarInt](Void.Minecraft.Buffers.Extensions.IntExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_IntExtensions\_AsVarInt\_System\_Int32\_)\([int](https://learn.microsoft.com/dotnet/api/system.int32)\), 
[IntExtensions](Void.Minecraft.Buffers.Extensions.IntExtensions.md).[AsVarInt](Void.Minecraft.Buffers.Extensions.IntExtensions.md\#Void\_Minecraft\_Buffers\_Extensions\_IntExtensions\_AsVarInt\_System\_Int32\_System\_Span\_System\_Byte\_\_)\([int](https://learn.microsoft.com/dotnet/api/system.int32), [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>\)

