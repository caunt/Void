---
title: NBT
description: Learn how to work with NBT (Named Binary Tags).
---

[**NBT (Named Binary Tags)**](https://minecraft.wiki/w/NBT_format) is a binary format used by Minecraft to store data. It is commonly used for transferring chunk data, item data, and other game-related information over the network. In this section, we will explore how to work with NBT in your plugins.

## Example Compound Tag
```csharp
var tag = new NbtCompound
{
    ["hello"] = new NbtString("world"),
    ["number"] = new NbtInt(69),
    ["list"] = new NbtList([new NbtInt(420)], NbtTagType.Int),
    ["child"] = new NbtCompound
    {
        ["key"] = new NbtString("value"),
    }
};
```

## Tag Types
- [**NbtByte**](/reference/Void.Minecraft.Nbt.Tags.NbtByte): Represents a byte value.
- [**NbtByteArray**](/reference/Void.Minecraft.Nbt.Tags.NbtByteArray): Represents an array of byte values.
- [**NbtCompound**](/reference/Void.Minecraft.Nbt.Tags.NbtCompound): Represents a compound tag, which can contain other tags.
- [**NbtDouble**](/reference/Void.Minecraft.Nbt.Tags.NbtDouble): Represents a double value.
- [**NbtEnd**](/reference/Void.Minecraft.Nbt.Tags.NbtEnd): Represents the end of a compound tag.
- [**NbtFloat**](/reference/Void.Minecraft.Nbt.Tags.NbtFloat): Represents a float value.
- [**NbtIntArray**](/reference/Void.Minecraft.Nbt.Tags.NbtIntArray): Represents an array of integer values.
- [**NbtInt**](/reference/Void.Minecraft.Nbt.Tags.NbtInt): Represents an integer value.
- [**NbtList**](/reference/Void.Minecraft.Nbt.Tags.NbtList): Represents a list of tags.
- [**NbtLongArray**](/reference/Void.Minecraft.Nbt.Tags.NbtLongArray): Represents an array of long values.
- [**NbtLong**](/reference/Void.Minecraft.Nbt.Tags.NbtLong): Represents a long value.
- [**NbtShort**](/reference/Void.Minecraft.Nbt.Tags.NbtShort): Represents a short value.
- [**NbtString**](/reference/Void.Minecraft.Nbt.Tags.NbtString): Represents a string value.

## Reading NBT Tag
To read NBT data, you can use the [**NbtTag**](/reference/Void.Minecraft.Nbt.NbtTag) class.
```csharp
var bytes = new byte[] { ... }; // Your NBT data as byte array

// length is the number of bytes read
var length = NbtTag.Parse(bytes, out var tag);

logger.LogInformation("Tag type: {Type}", tag.Type);

if (tag is NbtCompound compound)
{
    if (compound["key"] is NbtString key)
    {
        logger.LogInformation("{Value}", key.Value);
    }
}
```

## Writing NBT Tag
Use `AsStream()` method on your tag instance to access NBT as a byte stream.
```csharp
var tag = new NbtCompound
{
    ["key"] = new NbtString("value"),
};

var stream = tag.AsStream();
var bytes = stream.ToArray();
```

## SNBT
SNBT is a human-readable format for NBT data.
[**Read more about SNBT**](https://minecraft.wiki/w/NBT_format#SNBT_format).

### Converting NBT to SNBT
```csharp
var tag = new NbtCompound { ["key"] = new NbtString("value") };
Console.WriteLine(tag.ToString()); // Output: {key:"value"}
```

### Converting SNBT to NBT
```csharp
var snbt = "{key:\"value\"}";
var tag = NbtTag.Parse(snbt);
```
