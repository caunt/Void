# <a id="Void_Minecraft_Components_Text_Serializers_ComponentLegacySerializer"></a> Class ComponentLegacySerializer

Namespace: [Void.Minecraft.Components.Text.Serializers](Void.Minecraft.Components.Text.Serializers.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class ComponentLegacySerializer
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ComponentLegacySerializer](Void.Minecraft.Components.Text.Serializers.ComponentLegacySerializer.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Fields

### <a id="Void_Minecraft_Components_Text_Serializers_ComponentLegacySerializer_ExampleComplexLegacyString"></a> ExampleComplexLegacyString

```csharp
public const string ExampleComplexLegacyString = "&0Aa1!b@2$&k3#Cd$Ef%&1Gh7%h^j&lKl8&Lm*&2No9(Q)r&mSt0_Op+&3Uv-1=Wx&nYz2!Za@&4b#3$Cd%&oEf4&Gh*&5Ij5)Kl(&rMn6)Op?&6Qr7_Rs-&7Tu8*Vw&&8Xy9@Za!&9Bc0#De$&aFg1%Hi^&bJk2&Lm*&cNo3(Pq)&dRs4_St+&eUv5=Wx-&fYz6!Ab@"
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Serializers_ComponentLegacySerializer_ExampleLegacyString"></a> ExampleLegacyString

```csharp
public const string ExampleLegacyString = "&1Hello, &2this is a &x&F&F&A&A&0&1hex colored text, &lwith bold, &oitalic, &nunderline, &mstrikethrough, &kobfuscated, &rand reset."
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Void_Minecraft_Components_Text_Serializers_ComponentLegacySerializer_Deserialize_System_String_System_Char_"></a> Deserialize\(string, char\)

```csharp
public static Component Deserialize(string source, char prefix = '&')
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

`prefix` [char](https://learn.microsoft.com/dotnet/api/system.char)

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

### <a id="Void_Minecraft_Components_Text_Serializers_ComponentLegacySerializer_Serialize_Void_Minecraft_Components_Text_Component_System_Char_"></a> Serialize\(Component, char\)

```csharp
public static string Serialize(Component component, char prefix = '&')
```

#### Parameters

`component` [Component](Void.Minecraft.Components.Text.Component.md)

`prefix` [char](https://learn.microsoft.com/dotnet/api/system.char)

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

