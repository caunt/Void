# <a id="Void_Minecraft_Profiles_Uuid"></a> Struct Uuid

Namespace: [Void.Minecraft.Profiles](Void.Minecraft.Profiles.md)  
Assembly: Void.Minecraft.dll  

A Minecraft-compatible UUID backed by a .NET <xref href="System.Guid" data-throw-if-not-resolved="false"></xref>, with factory methods for the
wire-format encodings used by the Java Edition protocol.

```csharp
[JsonConverter(typeof(UuidJsonConverter))]
public struct Uuid : IComparable<Uuid>, IEquatable<Uuid>
```

#### Implements

[IComparable<Uuid\>](https://learn.microsoft.com/dotnet/api/system.icomparable\-1), 
[IEquatable<Uuid\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

#### Extension Methods

[StructExtensions.IsDefault<Uuid\>\(Uuid\)](Void.Proxy.Api.Extensions.StructExtensions.md\#Void\_Proxy\_Api\_Extensions\_StructExtensions\_IsDefault\_\_1\_\_\_0\_)

## Constructors

### <a id="Void_Minecraft_Profiles_Uuid__ctor_System_Guid_"></a> Uuid\(Guid\)

A Minecraft-compatible UUID backed by a .NET <xref href="System.Guid" data-throw-if-not-resolved="false"></xref>, with factory methods for the
wire-format encodings used by the Java Edition protocol.

```csharp
public Uuid(Guid guid)
```

#### Parameters

`guid` [Guid](https://learn.microsoft.com/dotnet/api/system.guid)

## Properties

### <a id="Void_Minecraft_Profiles_Uuid_AsGuid"></a> AsGuid

Gets the underlying .NET <xref href="System.Guid" data-throw-if-not-resolved="false"></xref> value.

```csharp
public readonly Guid AsGuid { get; }
```

#### Property Value

 [Guid](https://learn.microsoft.com/dotnet/api/system.guid)

### <a id="Void_Minecraft_Profiles_Uuid_Empty"></a> Empty

Gets the zero UUID (<code>00000000-0000-0000-0000-000000000000</code>), wrapping <xref href="System.Guid.Empty" data-throw-if-not-resolved="false"></xref>.

```csharp
public static Uuid Empty { get; }
```

#### Property Value

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

## Methods

### <a id="Void_Minecraft_Profiles_Uuid_CompareTo_Void_Minecraft_Profiles_Uuid_"></a> CompareTo\(Uuid\)

Compares this UUID to <code class="paramref">other</code> using the underlying <xref href="System.Guid" data-throw-if-not-resolved="false"></xref> comparison.

```csharp
public readonly int CompareTo(Uuid other)
```

#### Parameters

`other` [Uuid](Void.Minecraft.Profiles.Uuid.md)

The UUID to compare against.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A negative integer, zero, or a positive integer if this instance is less than, equal to,
    or greater than <code class="paramref">other</code>, respectively.

### <a id="Void_Minecraft_Profiles_Uuid_Equals_Void_Minecraft_Profiles_Uuid_"></a> Equals\(Uuid\)

Returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if this UUID equals <code class="paramref">other</code> by comparing their
underlying <xref href="System.Guid" data-throw-if-not-resolved="false"></xref> values.

```csharp
public readonly bool Equals(Uuid other)
```

#### Parameters

`other` [Uuid](Void.Minecraft.Profiles.Uuid.md)

The UUID to compare against.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the two UUIDs are equal; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Profiles_Uuid_Equals_System_Object_"></a> Equals\(object?\)

Returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">obj</code> is a <xref href="Void.Minecraft.Profiles.Uuid" data-throw-if-not-resolved="false"></xref> equal to this instance.

```csharp
public override readonly bool Equals(object? obj)
```

#### Parameters

`obj` [object](https://learn.microsoft.com/dotnet/api/system.object)?

The object to compare against.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">obj</code> is a <xref href="Void.Minecraft.Profiles.Uuid" data-throw-if-not-resolved="false"></xref> with the same value;
    otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Profiles_Uuid_FromLongs_System_Int64_System_Int64_"></a> FromLongs\(long, long\)

Reconstructs a UUID from the Java <code>UUID.getMostSignificantBits()</code> and
<code>UUID.getLeastSignificantBits()</code> long values.

```csharp
public static Uuid FromLongs(long mostSig, long leastSig)
```

#### Parameters

`mostSig` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The most significant 64 bits of the UUID.

`leastSig` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The least significant 64 bits of the UUID.

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

The <xref href="Void.Minecraft.Profiles.Uuid" data-throw-if-not-resolved="false"></xref> equivalent to the Java UUID with the given bit halves.

#### Remarks

The byte reordering translates from Java's big-endian UUID representation to the mixed-endian
layout used by .NET's <xref href="System.Guid" data-throw-if-not-resolved="false"></xref>.

### <a id="Void_Minecraft_Profiles_Uuid_FromStringHash_System_String_"></a> FromStringHash\(string\)

Derives a deterministic UUID from a UTF-8 string by computing its MD5 hash and stamping
the result with UUID Version 3 bits and the RFC 4122 variant bits.

```csharp
public static Uuid FromStringHash(string text)
```

#### Parameters

`text` [string](https://learn.microsoft.com/dotnet/api/system.string)

The input string to hash. Cannot be <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

A UUID whose 128-bit value is the MD5 hash of <code class="paramref">text</code>, with the version and variant fields set.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

<code class="paramref">text</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

### <a id="Void_Minecraft_Profiles_Uuid_GetHashCode"></a> GetHashCode\(\)

Returns the hash code of the underlying <xref href="System.Guid" data-throw-if-not-resolved="false"></xref>.

```csharp
public override readonly int GetHashCode()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

The hash code for this UUID.

### <a id="Void_Minecraft_Profiles_Uuid_GetVariant"></a> GetVariant\(\)

Returns the UUID variant as an integer decoded from the variant byte.

```csharp
public int GetVariant()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

<code>0</code> for NCS backward compatibility, <code>1</code> for RFC 4122, <code>2</code> for Microsoft,
<code>3</code> for future reserved, or <code>-1</code> if the variant byte is not in an expected range.

### <a id="Void_Minecraft_Profiles_Uuid_GetVersion"></a> GetVersion\(\)

Returns the UUID version number extracted from the version nibble of the underlying 128-bit value.

```csharp
public int GetVersion()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

An integer between 1 and 5 representing the UUID version field.

### <a id="Void_Minecraft_Profiles_Uuid_NewUuid"></a> NewUuid\(\)

Creates a new random UUID (Version 4).

```csharp
public static Uuid NewUuid()
```

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

A new <xref href="Void.Minecraft.Profiles.Uuid" data-throw-if-not-resolved="false"></xref> backed by a freshly generated <xref href="System.Guid" data-throw-if-not-resolved="false"></xref>.

### <a id="Void_Minecraft_Profiles_Uuid_Offline_System_String_"></a> Offline\(string\)

```csharp
public static Uuid Offline(string name)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

### <a id="Void_Minecraft_Profiles_Uuid_Parse_System_String_"></a> Parse\(string\)

Parses a UUID from its standard string representation.

```csharp
public static Uuid Parse(string text)
```

#### Parameters

`text` [string](https://learn.microsoft.com/dotnet/api/system.string)

The UUID string to parse.

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

The parsed <xref href="Void.Minecraft.Profiles.Uuid" data-throw-if-not-resolved="false"></xref>.

#### Exceptions

 [FormatException](https://learn.microsoft.com/dotnet/api/system.formatexception)

<code class="paramref">text</code> is not in a recognized UUID format.

### <a id="Void_Minecraft_Profiles_Uuid_Parse_System_Int32___"></a> Parse\(params int\[\]\)

Constructs a UUID from exactly four integers as encoded in the Minecraft Java Edition protocol.

```csharp
public static Uuid Parse(params int[] parts)
```

#### Parameters

`parts` [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]

An array of exactly four <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/integral-numeric-types">int</a> values representing the UUID.

#### Returns

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

The <xref href="Void.Minecraft.Profiles.Uuid" data-throw-if-not-resolved="false"></xref> reconstructed from the four integer parts.

#### Remarks

In the Java Edition protocol, a UUID is transmitted as two 64-bit halves, each split into two
big-endian <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/integral-numeric-types">int</a> values. This method reorders the bytes to produce the equivalent
.NET <xref href="System.Guid" data-throw-if-not-resolved="false"></xref> representation.

#### Exceptions

 [ArgumentException](https://learn.microsoft.com/dotnet/api/system.argumentexception)

<code class="paramref">parts</code> does not contain exactly four elements.

### <a id="Void_Minecraft_Profiles_Uuid_ToString"></a> ToString\(\)

Returns the standard hyphenated lowercase UUID string representation,
for example <code>"550e8400-e29b-41d4-a716-446655440000"</code>.

```csharp
public override readonly string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Profiles_Uuid_TryParse_System_String_Void_Minecraft_Profiles_Uuid__"></a> TryParse\(string?, out Uuid\)

Attempts to parse a UUID string. Returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> and sets <code class="paramref">uuid</code>
on success; returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a> and sets <code class="paramref">uuid</code> to the default value on failure.

```csharp
public static bool TryParse(string? text, out Uuid uuid)
```

#### Parameters

`text` [string](https://learn.microsoft.com/dotnet/api/system.string)?

The UUID string to parse, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

`uuid` [Uuid](Void.Minecraft.Profiles.Uuid.md)

When this method returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>, contains the parsed <xref href="Void.Minecraft.Profiles.Uuid" data-throw-if-not-resolved="false"></xref>;
otherwise, the default <xref href="Void.Minecraft.Profiles.Uuid" data-throw-if-not-resolved="false"></xref> value.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">text</code> was successfully parsed; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

## Operators

### <a id="Void_Minecraft_Profiles_Uuid_op_Equality_Void_Minecraft_Profiles_Uuid_Void_Minecraft_Profiles_Uuid_"></a> operator ==\(Uuid, Uuid\)

Returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">left</code> and <code class="paramref">right</code> are equal.

```csharp
public static bool operator ==(Uuid left, Uuid right)
```

#### Parameters

`left` [Uuid](Void.Minecraft.Profiles.Uuid.md)

The first UUID to compare.

`right` [Uuid](Void.Minecraft.Profiles.Uuid.md)

The second UUID to compare.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the two UUIDs are equal; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Profiles_Uuid_op_Inequality_Void_Minecraft_Profiles_Uuid_Void_Minecraft_Profiles_Uuid_"></a> operator \!=\(Uuid, Uuid\)

Returns <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">left</code> and <code class="paramref">right</code> are not equal.

```csharp
public static bool operator !=(Uuid left, Uuid right)
```

#### Parameters

`left` [Uuid](Void.Minecraft.Profiles.Uuid.md)

The first UUID to compare.

`right` [Uuid](Void.Minecraft.Profiles.Uuid.md)

The second UUID to compare.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the two UUIDs are not equal; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

