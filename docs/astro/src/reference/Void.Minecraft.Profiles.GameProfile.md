# <a id="Void_Minecraft_Profiles_GameProfile"></a> Class GameProfile

Namespace: [Void.Minecraft.Profiles](Void.Minecraft.Profiles.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record GameProfile : IEquatable<GameProfile>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[GameProfile](Void.Minecraft.Profiles.GameProfile.md)

#### Implements

[IEquatable<GameProfile\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Profiles_GameProfile__ctor_System_String_Void_Minecraft_Profiles_Uuid_Void_Minecraft_Profiles_Property___"></a> GameProfile\(string, Uuid, Property\[\]?\)

```csharp
public GameProfile(string Username, Uuid Id = default, Property[]? Properties = null)
```

#### Parameters

`Username` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Id` [Uuid](Void.Minecraft.Profiles.Uuid.md)

`Properties` [Property](Void.Minecraft.Profiles.Property.md)\[\]?

## Properties

### <a id="Void_Minecraft_Profiles_GameProfile_Id"></a> Id

```csharp
[JsonConverter(typeof(UuidJsonConverter))]
public Uuid Id { get; init; }
```

#### Property Value

 [Uuid](Void.Minecraft.Profiles.Uuid.md)

### <a id="Void_Minecraft_Profiles_GameProfile_Properties"></a> Properties

```csharp
public Property[]? Properties { get; init; }
```

#### Property Value

 [Property](Void.Minecraft.Profiles.Property.md)\[\]?

### <a id="Void_Minecraft_Profiles_GameProfile_Username"></a> Username

```csharp
[JsonPropertyName("name")]
public string Username { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

