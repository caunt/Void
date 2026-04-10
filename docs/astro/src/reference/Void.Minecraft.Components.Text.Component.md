# <a id="Void_Minecraft_Components_Text_Component"></a> Class Component

Namespace: [Void.Minecraft.Components.Text](Void.Minecraft.Components.Text.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record Component : IEquatable<Component>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Component](Void.Minecraft.Components.Text.Component.md)

#### Implements

[IEquatable<Component\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Components_Text_Component__ctor_Void_Minecraft_Components_Text_Properties_Content_IContent_Void_Minecraft_Components_Text_Properties_Children_Void_Minecraft_Components_Text_Properties_Formatting_Void_Minecraft_Components_Text_Properties_Interactivity_"></a> Component\(IContent, Children, Formatting, Interactivity\)

```csharp
public Component(IContent Content, Children Children, Formatting Formatting, Interactivity Interactivity)
```

#### Parameters

`Content` [IContent](Void.Minecraft.Components.Text.Properties.Content.IContent.md)

`Children` [Children](Void.Minecraft.Components.Text.Properties.Children.md)

`Formatting` [Formatting](Void.Minecraft.Components.Text.Properties.Formatting.md)

`Interactivity` [Interactivity](Void.Minecraft.Components.Text.Properties.Interactivity.md)

## Properties

### <a id="Void_Minecraft_Components_Text_Component_AsText"></a> AsText

Returns a string representation of the component.

```csharp
public string AsText { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Components_Text_Component_Children"></a> Children

```csharp
public Children Children { get; init; }
```

#### Property Value

 [Children](Void.Minecraft.Components.Text.Properties.Children.md)

### <a id="Void_Minecraft_Components_Text_Component_Content"></a> Content

```csharp
public IContent Content { get; init; }
```

#### Property Value

 [IContent](Void.Minecraft.Components.Text.Properties.Content.IContent.md)

### <a id="Void_Minecraft_Components_Text_Component_Default"></a> Default

Represents a default component initialized with empty text content, default children, formatting, and
interactivity.

```csharp
public static Component Default { get; }
```

#### Property Value

 [Component](Void.Minecraft.Components.Text.Component.md)

### <a id="Void_Minecraft_Components_Text_Component_Formatting"></a> Formatting

```csharp
public Formatting Formatting { get; init; }
```

#### Property Value

 [Formatting](Void.Minecraft.Components.Text.Properties.Formatting.md)

### <a id="Void_Minecraft_Components_Text_Component_Interactivity"></a> Interactivity

```csharp
public Interactivity Interactivity { get; init; }
```

#### Property Value

 [Interactivity](Void.Minecraft.Components.Text.Properties.Interactivity.md)

## Methods

### <a id="Void_Minecraft_Components_Text_Component_DeserializeJson_System_Text_Json_Nodes_JsonNode_"></a> DeserializeJson\(JsonNode\)

Deserializes a JSON node into a Component object.

```csharp
public static Component DeserializeJson(JsonNode source)
```

#### Parameters

`source` [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

The JSON node containing the data to be deserialized into a Component.

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

Returns a Component object created from the provided JSON data.

### <a id="Void_Minecraft_Components_Text_Component_DeserializeLegacy_System_String_System_Char_"></a> DeserializeLegacy\(string, char\)

Deserializes a string into a Component object using legacy serialization.

```csharp
public static Component DeserializeLegacy(string source, char prefix = '&')
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

The string representation of the Component to be deserialized.

`prefix` [char](https://learn.microsoft.com/dotnet/api/system.char)

The character used to identify properties in the string representation.

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

Returns a deserialized Component object.

### <a id="Void_Minecraft_Components_Text_Component_DeserializeNbt_Void_Minecraft_Nbt_NbtTag_"></a> DeserializeNbt\(NbtTag\)

Deserializes an NBT tag into a Component object.

```csharp
public static Component DeserializeNbt(NbtTag tag)
```

#### Parameters

`tag` [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

The NBT tag that contains the data to be deserialized into a Component.

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

Returns a Component object created from the provided NBT tag.

### <a id="Void_Minecraft_Components_Text_Component_DeserializeSnbt_System_String_"></a> DeserializeSnbt\(string\)

Deserializes a string representation of SNBT into a Component object.

```csharp
public static Component DeserializeSnbt(string source)
```

#### Parameters

`source` [string](https://learn.microsoft.com/dotnet/api/system.string)

The string representation of SNBT that needs to be converted into a Component.

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

Returns a Component object created from the deserialized SNBT data.

### <a id="Void_Minecraft_Components_Text_Component_ReadFrom__1___0__System_Boolean_"></a> ReadFrom<TBuffer\>\(ref TBuffer, bool\)

Reads an NBT tag from the buffer and deserializes it as a Minecraft text component.

```csharp
public static Component ReadFrom<TBuffer>(ref TBuffer buffer, bool readName = true) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

A reference to the buffer from which the NBT-encoded component is read.

`readName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

When <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>, reads the tag's name prefix from the binary stream before deserializing
the component. When <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>, the name field is skipped, which is appropriate for tags
embedded inside a List. Defaults to <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a>.

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

The <xref href="Void.Minecraft.Components.Text.Component" data-throw-if-not-resolved="false"></xref> deserialized from the NBT data read out of the buffer.

#### Type Parameters

`TBuffer` 

The buffer type. Must be a value type implementing <xref href="Void.Minecraft.Buffers.IMinecraftBuffer%601" data-throw-if-not-resolved="false"></xref>.

### <a id="Void_Minecraft_Components_Text_Component_ReadFrom_Void_Minecraft_Buffers_MinecraftBuffer__System_Boolean_"></a> ReadFrom\(ref MinecraftBuffer, bool\)

Reads data from a buffer and deserializes it into a Component.

```csharp
public static Component ReadFrom(ref MinecraftBuffer buffer, bool readName = true)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

The buffer containing the data to be read and deserialized into a Component.

`readName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

Returns a Component object created from the data in the buffer.

### <a id="Void_Minecraft_Components_Text_Component_SerializeJson"></a> SerializeJson\(\)

Serializes the current component to JSON format.

```csharp
public JsonNode SerializeJson()
```

#### Returns

 [JsonNode](https://learn.microsoft.com/dotnet/api/system.text.json.nodes.jsonnode)

Returns a JsonNode representing the serialized component.

### <a id="Void_Minecraft_Components_Text_Component_SerializeLegacy_System_Char_"></a> SerializeLegacy\(char\)

Serializes the current object to Legacy format.

```csharp
public string SerializeLegacy(char prefix = '&')
```

#### Parameters

`prefix` [char](https://learn.microsoft.com/dotnet/api/system.char)

Specifies the prefix of the formatting codes to be used during serialization.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

Returns a Legacy string representing the serialized object.

### <a id="Void_Minecraft_Components_Text_Component_SerializeNbt"></a> SerializeNbt\(\)

Serializes the current component into NBT format.

```csharp
public NbtTag SerializeNbt()
```

#### Returns

 [NbtTag](Void.Minecraft.Nbt.NbtTag.md)

Returns the serialized NBT tag representation of the component.

### <a id="Void_Minecraft_Components_Text_Component_SerializeSnbt"></a> SerializeSnbt\(\)

Serializes the current component into SNBT format.

```csharp
public string SerializeSnbt()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

Returns the serialized data in SNBT format.

### <a id="Void_Minecraft_Components_Text_Component_ToString"></a> ToString\(\)

Converts the object to its string representation by serializing it to SNBT format.
version.

```csharp
public override string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

Returns the string representation of the serialized NBT data.

### <a id="Void_Minecraft_Components_Text_Component_WriteTo_Void_Minecraft_Buffers_MinecraftBuffer__System_Boolean_"></a> WriteTo\(ref MinecraftBuffer, bool\)

Writes serialized data to a buffer.

```csharp
public void WriteTo(ref MinecraftBuffer buffer, bool writeName = true)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

The buffer is used to store serialized data.

`writeName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

A boolean parameter that indicates whether to include the name when writing the tag to the buffer. Default is true.

### <a id="Void_Minecraft_Components_Text_Component_WriteTo__1___0__System_Boolean_"></a> WriteTo<TBuffer\>\(ref TBuffer, bool\)

Writes serialized data to a buffer.

```csharp
public void WriteTo<TBuffer>(ref TBuffer buffer, bool writeName = true) where TBuffer : struct, IMinecraftBuffer<TBuffer>, allows ref struct
```

#### Parameters

`buffer` TBuffer

This parameter is the destination where the serialized data will be written.

`writeName` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

A boolean parameter that indicates whether to include the name when writing the tag to the buffer. Default is true.

#### Type Parameters

`TBuffer` 

This type parameter represents a structure that implements a specific buffer interface for writing data.

## Operators

### <a id="Void_Minecraft_Components_Text_Component_op_Implicit_System_String__Void_Minecraft_Components_Text_Component"></a> implicit operator Component\(string\)

```csharp
public static implicit operator Component(string text)
```

#### Parameters

`text` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [Component](Void.Minecraft.Components.Text.Component.md)

