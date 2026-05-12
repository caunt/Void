# <a id="Void_Minecraft_Nbt_CompressionType"></a> Enum CompressionType

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Describes compression formats supported by the NBT specification.

```csharp
public enum CompressionType : byte
```

#### Extension Methods

[StructExtensions.IsDefault<CompressionType\>\(CompressionType\)](Void.Proxy.Api.Extensions.StructExtensions.md\#Void\_Proxy\_Api\_Extensions\_StructExtensions\_IsDefault\_\_1\_\_\_0\_)

## Fields

`AutoDetect = 255` 

Automatically detect compression using magic numbers.

This is not a valid value when specifying a compression type for <b>writing</b>.

`GZip = 1` 

GZip compression



`None = 0` 

No compression.



`ZLib = 2` 

ZLib compression



