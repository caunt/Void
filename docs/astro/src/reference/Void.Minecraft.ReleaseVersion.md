# <a id="Void_Minecraft_ReleaseVersion"></a> Class ReleaseVersion

Namespace: [Void.Minecraft](Void.Minecraft.md)  
Assembly: Void.Minecraft.dll  

Represents a Minecraft release version number in <code>Major.Minor[.Patch]</code> form.

```csharp
public record ReleaseVersion : IComparable<ReleaseVersion>, IEquatable<ReleaseVersion>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ReleaseVersion](Void.Minecraft.ReleaseVersion.md)

#### Implements

[IComparable<ReleaseVersion\>](https://learn.microsoft.com/dotnet/api/system.icomparable\-1), 
[IEquatable<ReleaseVersion\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Remarks

<p>
Patch is optional: when it is <code>0</code>, <xref href="Void.Minecraft.ReleaseVersion.ToString" data-throw-if-not-resolved="false"></xref> omits it and returns the two-component form
(for example, <code>"1.21"</code>). A non-zero patch produces the three-component form (for example,
<code>"1.21.4"</code>).
</p>
<p>
Ordering follows the natural numeric ordering of the components: Major first, then Minor, then Patch.
</p>
<p>
Implicit conversions are provided in both directions so that a
<xref href="Void.Minecraft.ReleaseVersion" data-throw-if-not-resolved="false"></xref> can be assigned from or cast to a <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/reference-types">string</a> without an explicit
call to <xref href="Void.Minecraft.ReleaseVersion.Parse(System.String)" data-throw-if-not-resolved="false"></xref>.
</p>

## Constructors

### <a id="Void_Minecraft_ReleaseVersion__ctor_System_UInt16_System_UInt16_System_UInt16_"></a> ReleaseVersion\(ushort, ushort, ushort\)

Represents a Minecraft release version number in <code>Major.Minor[.Patch]</code> form.

```csharp
public ReleaseVersion(ushort Major, ushort Minor, ushort Patch)
```

#### Parameters

`Major` [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

The major component of the version number (for example, <code>1</code> in <code>1.21.4</code>).

`Minor` [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

The minor component of the version number (for example, <code>21</code> in <code>1.21.4</code>).

`Patch` [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

The patch component of the version number (for example, <code>4</code> in <code>1.21.4</code>). Pass <code>0</code> for releases that have no patch segment.

#### Remarks

<p>
Patch is optional: when it is <code>0</code>, <xref href="Void.Minecraft.ReleaseVersion.ToString" data-throw-if-not-resolved="false"></xref> omits it and returns the two-component form
(for example, <code>"1.21"</code>). A non-zero patch produces the three-component form (for example,
<code>"1.21.4"</code>).
</p>
<p>
Ordering follows the natural numeric ordering of the components: Major first, then Minor, then Patch.
</p>
<p>
Implicit conversions are provided in both directions so that a
<xref href="Void.Minecraft.ReleaseVersion" data-throw-if-not-resolved="false"></xref> can be assigned from or cast to a <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/reference-types">string</a> without an explicit
call to <xref href="Void.Minecraft.ReleaseVersion.Parse(System.String)" data-throw-if-not-resolved="false"></xref>.
</p>

## Properties

### <a id="Void_Minecraft_ReleaseVersion_Major"></a> Major

The major component of the version number (for example, <code>1</code> in <code>1.21.4</code>).

```csharp
public ushort Major { get; init; }
```

#### Property Value

 [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

### <a id="Void_Minecraft_ReleaseVersion_Minor"></a> Minor

The minor component of the version number (for example, <code>21</code> in <code>1.21.4</code>).

```csharp
public ushort Minor { get; init; }
```

#### Property Value

 [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

### <a id="Void_Minecraft_ReleaseVersion_Patch"></a> Patch

The patch component of the version number (for example, <code>4</code> in <code>1.21.4</code>). Pass <code>0</code> for releases that have no patch segment.

```csharp
public ushort Patch { get; init; }
```

#### Property Value

 [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

## Methods

### <a id="Void_Minecraft_ReleaseVersion_CompareTo_Void_Minecraft_ReleaseVersion_"></a> CompareTo\(ReleaseVersion?\)

Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.

```csharp
public int CompareTo(ReleaseVersion? other)
```

#### Parameters

`other` [ReleaseVersion](Void.Minecraft.ReleaseVersion.md)?

An object to compare with this instance.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A value that indicates the relative order of the objects being compared. The return value has these meanings:

 <table><thead><tr><th class="term"> Value</th><th class="description"> Meaning</th></tr></thead><tbody><tr><td class="term"> Less than zero</td><td class="description"> This instance precedes <code class="paramref">other</code> in the sort order.</td></tr><tr><td class="term"> Zero</td><td class="description"> This instance occurs in the same position in the sort order as <code class="paramref">other</code>.</td></tr><tr><td class="term"> Greater than zero</td><td class="description"> This instance follows <code class="paramref">other</code> in the sort order.</td></tr></tbody></table>

### <a id="Void_Minecraft_ReleaseVersion_Parse_System_String_"></a> Parse\(string\)

```csharp
public static ReleaseVersion Parse(string versionString)
```

#### Parameters

`versionString` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [ReleaseVersion](Void.Minecraft.ReleaseVersion.md)

### <a id="Void_Minecraft_ReleaseVersion_ToString"></a> ToString\(\)

Returns a string that represents the current object.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

A string that represents the current object.

### <a id="Void_Minecraft_ReleaseVersion_TryParse_System_String_Void_Minecraft_ReleaseVersion__"></a> TryParse\(string, out ReleaseVersion\)

```csharp
public static bool TryParse(string versionString, out ReleaseVersion version)
```

#### Parameters

`versionString` [string](https://learn.microsoft.com/dotnet/api/system.string)

`version` [ReleaseVersion](Void.Minecraft.ReleaseVersion.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Operators

### <a id="Void_Minecraft_ReleaseVersion_op_Implicit_System_String__Void_Minecraft_ReleaseVersion"></a> implicit operator ReleaseVersion\(string\)

```csharp
public static implicit operator ReleaseVersion(string versionString)
```

#### Parameters

`versionString` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [ReleaseVersion](Void.Minecraft.ReleaseVersion.md)

### <a id="Void_Minecraft_ReleaseVersion_op_Implicit_Void_Minecraft_ReleaseVersion__System_String"></a> implicit operator string\(ReleaseVersion\)

```csharp
public static implicit operator string(ReleaseVersion version)
```

#### Parameters

`version` [ReleaseVersion](Void.Minecraft.ReleaseVersion.md)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

