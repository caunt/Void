# <a id="Void_Minecraft_Nbt_NumericTag_1"></a> Class NumericTag<T\>

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Abstract base class for <xref href="Void.Minecraft.Nbt.Tag" data-throw-if-not-resolved="false"></xref> types that contain a single numeric value.

```csharp
public abstract class NumericTag<T> : Tag, IEquatable<Tag>, ICloneable, IEquatable<NumericTag<T>>, IComparable<NumericTag<T>>, IComparable where T : unmanaged, INumber<T>
```

#### Type Parameters

`T` 

A value type that implements <xref href="System.Numerics.INumber%601" data-throw-if-not-resolved="false"></xref>.

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Tag](Void.Minecraft.Nbt.Tag.md) ← 
[NumericTag<T\>](Void.Minecraft.Nbt.NumericTag\-1.md)

#### Implements

[IEquatable<Tag\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[ICloneable](https://learn.microsoft.com/dotnet/api/system.icloneable), 
[IEquatable<NumericTag<T\>\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IComparable<NumericTag<T\>\>](https://learn.microsoft.com/dotnet/api/system.icomparable\-1), 
[IComparable](https://learn.microsoft.com/dotnet/api/system.icomparable)

#### Inherited Members

[Tag.NoName](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_NoName), 
[Tag.Type](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Type), 
[Tag.Parent](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Parent), 
[Tag.Name](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Name), 
[Tag.PrettyPrinted\(StringBuilder, int, string\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_PrettyPrinted\_System\_Text\_StringBuilder\_System\_Int32\_System\_String\_), 
[Tag.PrettyName](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_PrettyName), 
[Tag.WriteJson\(Utf8JsonWriter, bool\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_WriteJson\_System\_Text\_Json\_Utf8JsonWriter\_System\_Boolean\_), 
[Tag.WriteJson\(Stream, JsonWriterOptions?\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_WriteJson\_System\_IO\_Stream\_System\_Nullable\_System\_Text\_Json\_JsonWriterOptions\_\_), 
[Tag.WriteJsonAsync\(Stream, JsonWriterOptions?\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_WriteJsonAsync\_System\_IO\_Stream\_System\_Nullable\_System\_Text\_Json\_JsonWriterOptions\_\_), 
[Tag.ToJson\(JsonWriterOptions?\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_ToJson\_System\_Nullable\_System\_Text\_Json\_JsonWriterOptions\_\_), 
[Tag.ToJsonString\(bool, string\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_ToJsonString\_System\_Boolean\_System\_String\_), 
[Tag.Equals\(Tag?\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Equals\_Void\_Minecraft\_Nbt\_Tag\_), 
[Tag.Equals\(object?\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Equals\_System\_Object\_), 
[Tag.GetHashCode\(\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_GetHashCode), 
[Tag.Clone\(\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Clone), 
[Tag.Stringify\(bool\)](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_Stringify\_System\_Boolean\_), 
[Tag.StringifyName](Void.Minecraft.Nbt.Tag.md\#Void\_Minecraft\_Nbt\_Tag\_StringifyName), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Nbt_NumericTag_1__ctor_Void_Minecraft_Nbt_TagType_System_String__0_"></a> NumericTag\(TagType, string?, T\)

```csharp
protected NumericTag(TagType type, string? name, T value)
```

#### Parameters

`type` [TagType](Void.Minecraft.Nbt.TagType.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

`value` T

## Properties

### <a id="Void_Minecraft_Nbt_NumericTag_1_Value"></a> Value

Gets or sets the value of the tag.

```csharp
public T Value { get; set; }
```

#### Property Value

 T

## Methods

### <a id="Void_Minecraft_Nbt_NumericTag_1_CompareTo_Void_Minecraft_Nbt_NumericTag__0__"></a> CompareTo\(NumericTag<T\>?\)

Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.

```csharp
public int CompareTo(NumericTag<T>? other)
```

#### Parameters

`other` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

An object to compare with this instance.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A value that indicates the relative order of the objects being compared. The return value has these meanings:

 <table><thead><tr><th class="term"> Value</th><th class="description"> Meaning</th></tr></thead><tbody><tr><td class="term"> Less than zero</td><td class="description"> This instance precedes <code class="paramref">other</code> in the sort order.</td></tr><tr><td class="term"> Zero</td><td class="description"> This instance occurs in the same position in the sort order as <code class="paramref">other</code>.</td></tr><tr><td class="term"> Greater than zero</td><td class="description"> This instance follows <code class="paramref">other</code> in the sort order.</td></tr></tbody></table>

### <a id="Void_Minecraft_Nbt_NumericTag_1_CompareTo_System_Object_"></a> CompareTo\(object?\)

Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.

```csharp
public int CompareTo(object? obj)
```

#### Parameters

`obj` [object](https://learn.microsoft.com/dotnet/api/system.object)?

An object to compare with this instance.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A value that indicates the relative order of the objects being compared. The return value has these meanings:

 <table><thead><tr><th class="term"> Value</th><th class="description"> Meaning</th></tr></thead><tbody><tr><td class="term"> Less than zero</td><td class="description"> This instance precedes <code class="paramref">obj</code> in the sort order.</td></tr><tr><td class="term"> Zero</td><td class="description"> This instance occurs in the same position in the sort order as <code class="paramref">obj</code>.</td></tr><tr><td class="term"> Greater than zero</td><td class="description"> This instance follows <code class="paramref">obj</code> in the sort order.</td></tr></tbody></table>

#### Exceptions

 [ArgumentException](https://learn.microsoft.com/dotnet/api/system.argumentexception)

<code class="paramref">obj</code> is not the same type as this instance.

### <a id="Void_Minecraft_Nbt_NumericTag_1_Equals_Void_Minecraft_Nbt_NumericTag__0__"></a> Equals\(NumericTag<T\>?\)

Indicates whether the current object is equal to another object of the same type.

```csharp
public bool Equals(NumericTag<T>? other)
```

#### Parameters

`other` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

An object to compare with this object.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the current object is equal to the <code class="paramref">other</code> parameter; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Nbt_NumericTag_1_Equals_System_Object_"></a> Equals\(object?\)

Determines whether the specified object is equal to the current object.

```csharp
public override bool Equals(object? obj)
```

#### Parameters

`obj` [object](https://learn.microsoft.com/dotnet/api/system.object)?

The object to compare with the current object.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the specified object  is equal to the current object; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Nbt_NumericTag_1_GetHashCode"></a> GetHashCode\(\)

Serves as the default hash function.

```csharp
public override int GetHashCode()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A hash code for the current object.

## Operators

### <a id="Void_Minecraft_Nbt_NumericTag_1_op_Equality_Void_Minecraft_Nbt_NumericTag__0__Void_Minecraft_Nbt_NumericTag__0__"></a> operator ==\(NumericTag<T\>?, NumericTag<T\>?\)

Compares two values to determine equality.

```csharp
public static bool operator ==(NumericTag<T>? left, NumericTag<T>? right)
```

#### Parameters

`left` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">right</code>.

`right` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">left</code>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">left</code> is equal to <code class="paramref">right</code>; otherwise,
<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Nbt_NumericTag_1_op_GreaterThan_Void_Minecraft_Nbt_NumericTag__0__Void_Minecraft_Nbt_NumericTag__0__"></a> operator \>\(NumericTag<T\>?, NumericTag<T\>?\)

Compares two values to determine which is greater.

```csharp
public static bool operator >(NumericTag<T>? left, NumericTag<T>? right)
```

#### Parameters

`left` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">right</code>.

`right` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">left</code>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">left</code> is greater than <code class="paramref">right</code>; otherwise,
<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Nbt_NumericTag_1_op_GreaterThanOrEqual_Void_Minecraft_Nbt_NumericTag__0__Void_Minecraft_Nbt_NumericTag__0__"></a> operator \>=\(NumericTag<T\>?, NumericTag<T\>?\)

Compares two values to determine which is greater or equal.

```csharp
public static bool operator >=(NumericTag<T>? left, NumericTag<T>? right)
```

#### Parameters

`left` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">right</code>.

`right` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">left</code>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">left</code> is greater than or equal to <code class="paramref">right</code>;
otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Nbt_NumericTag_1_op_Implicit_Void_Minecraft_Nbt_NumericTag__0____0"></a> implicit operator T\(NumericTag<T\>\)

Implicit conversion of a <xref href="Void.Minecraft.Nbt.NumericTag%601" data-throw-if-not-resolved="false"></xref> to a T.

```csharp
public static implicit operator T(NumericTag<T> tag)
```

#### Parameters

`tag` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>

The <xref href="Void.Minecraft.Nbt.NumericTag%601" data-throw-if-not-resolved="false"></xref> to be converted.

#### Returns

 T

The value of <code class="paramref">tag</code> as a T.

### <a id="Void_Minecraft_Nbt_NumericTag_1_op_Inequality_Void_Minecraft_Nbt_NumericTag__0__Void_Minecraft_Nbt_NumericTag__0__"></a> operator \!=\(NumericTag<T\>?, NumericTag<T\>?\)

Compares two values to determine inequality.

```csharp
public static bool operator !=(NumericTag<T>? left, NumericTag<T>? right)
```

#### Parameters

`left` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">right</code>.

`right` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">left</code>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">left</code> is not equal to <code class="paramref">right</code>; otherwise,
<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Nbt_NumericTag_1_op_LessThan_Void_Minecraft_Nbt_NumericTag__0__Void_Minecraft_Nbt_NumericTag__0__"></a> operator <\(NumericTag<T\>?, NumericTag<T\>?\)

Compares two values to determine which is less.

```csharp
public static bool operator <(NumericTag<T>? left, NumericTag<T>? right)
```

#### Parameters

`left` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">right</code>.

`right` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">left</code>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">left</code> is less than <code class="paramref">right</code>; otherwise,
<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Void_Minecraft_Nbt_NumericTag_1_op_LessThanOrEqual_Void_Minecraft_Nbt_NumericTag__0__Void_Minecraft_Nbt_NumericTag__0__"></a> operator <=\(NumericTag<T\>?, NumericTag<T\>?\)

Compares two values to determine which is less or equal.

```csharp
public static bool operator <=(NumericTag<T>? left, NumericTag<T>? right)
```

#### Parameters

`left` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">right</code>.

`right` [NumericTag](Void.Minecraft.Nbt.NumericTag\-1.md)<T\>?

The value to compare with <code class="paramref">left</code>.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if <code class="paramref">left</code> is less than or equal to <code class="paramref">right</code>;
otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

