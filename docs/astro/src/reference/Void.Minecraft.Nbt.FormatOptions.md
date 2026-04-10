# <a id="Void_Minecraft_Nbt_FormatOptions"></a> Enum FormatOptions

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Describes the specification to use for reading/writing.

```csharp
[Flags]
public enum FormatOptions
```

#### Extension Methods

[StructExtensions.IsDefault<FormatOptions\>\(FormatOptions\)](Void.Proxy.Api.Extensions.StructExtensions.md\#Void\_Proxy\_Api\_Extensions\_StructExtensions\_IsDefault\_\_1\_\_\_0\_)

## Fields

`BedrockFile = 2` 

Flags for using a format compatible with Bedrock editions of Minecraft when writing to a file.



`BedrockNetwork = 14` 

Flags for using a format compatible with Bedrock editions of Minecraft when transporting across a network.



`BigEndian = 1` 

Numeric values will be read/written in big-endian format.

This is the default for the Java edition of Minecraft.

`Java = 1` 

Flags for using a format compatible with Java editions of Minecraft.



`LittleEndian = 2` 

Numeric values will be read/written in little-endian format.

This is the default for Bedrock editions of Minecraft.

`None = 0` 

None/invalid option flags.



`VarIntegers = 4` 

Integer types will be read/written as variable-length integers.



`ZigZagEncoding = 8` 

Variable-length integers will be written using ZigZag encoding.



## Remarks

There are some major changes between the original Java version, and the Bedrock editions of Minecraft that make them incompatible with one another.
Furthermore, the Bedrock editions use a different specification depending on whether it is writing to disk or sending over a network.

## See Also

[https://wiki.vg/NBT\#Bedrock\_edition](https://wiki.vg/NBT\#Bedrock\_edition)

