# <a id="Void_Minecraft_Profiles_Property"></a> Class Property

Namespace: [Void.Minecraft.Profiles](Void.Minecraft.Profiles.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record Property : IEquatable<Property>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Property](Void.Minecraft.Profiles.Property.md)

#### Implements

[IEquatable<Property\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Profiles_Property__ctor_System_String_System_String_System_Boolean_System_String_"></a> Property\(string, string, bool, string?\)

```csharp
public Property(string name, string value, bool isSigned = false, string? signature = null)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`value` [string](https://learn.microsoft.com/dotnet/api/system.string)

`isSigned` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`signature` [string](https://learn.microsoft.com/dotnet/api/system.string)?

## Properties

### <a id="Void_Minecraft_Profiles_Property_IsSigned"></a> IsSigned

```csharp
public bool IsSigned { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Profiles_Property_Name"></a> Name

```csharp
public string Name { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Profiles_Property_Signature"></a> Signature

```csharp
public string? Signature { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Minecraft_Profiles_Property_Value"></a> Value

```csharp
public string Value { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

