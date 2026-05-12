# <a id="Void_Minecraft_Nbt_TagType"></a> Enum TagType

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Strongly-typed numerical constants that are prefixed to tags to denote their type.

```csharp
[Serializable]
public enum TagType : byte
```

#### Extension Methods

[StructExtensions.IsDefault<TagType\>\(TagType\)](Void.Proxy.Api.Extensions.StructExtensions.md\#Void\_Proxy\_Api\_Extensions\_StructExtensions\_IsDefault\_\_1\_\_\_0\_)

## Fields

`Byte = 1` 

A single signed byte,



`ByteArray = 7` 

A length-prefixed array of bytes.



`Compound = 10` 

A set of named tags.



`Double = 6` 

A single IEEE-754 double-precision floating point number.



`End = 0` 

Signifies the end of a <xref href="Void.Minecraft.Nbt.CompoundTag" data-throw-if-not-resolved="false"></xref>.

Some implementations may also use this as the child type for an empty list.

`Float = 5` 

A single IEEE-754 single-precision floating point number.



`Int = 3` 

A single signed 32-bit integer.



`IntArray = 11` 

A length-prefixed array of signed 32-bit integers.



`List = 9` 

A list of nameless tags, all of the same type.



`Long = 4` 

A single signed 64-bit integer.



`LongArray = 12` 

A length-prefixed array of signed 64-bit integers.



`Short = 2` 

A single signed 16-bit integer.



`String = 8` 

A length-prefixed UTF-8 string.



