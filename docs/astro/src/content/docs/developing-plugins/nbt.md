---
title: NBT
description: Learn how to work with NBT (Named Binary Tags).
---

[**NBT (Named Binary Tags)**](https://minecraft.fandom.com/wiki/NBT_format) is a binary format used by Minecraft to store data. It is commonly used for transferring chunks data, items data, and other game-related information over the network. In this section, we will explore how to work with NBT in your plugins.

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
- `NbtByte`: Represents a byte value.
- `NbtByteArray`: Represents an array of byte values.
- `NbtCompound`: Represents a compound tag, which can contain other tags.
- `NbtDouble`: Represents a double value.
- `NbtEnd`: Represents the end of a compound tag.
- `NbtFloat`: Represents a float value.
- `NbtIntArray`: Represents an array of integer values.
- `NbtInt`: Represents an integer value.
- `NbtList`: Represents a list of tags.
- `NbtLongArray`: Represents an array of long values.
- `NbtLong`: Represents a long value.
- `NbtShort`: Represents a short value.
- `NbtString`: Represents a string value.

## Reading NBT Tag
To read NBT data, you can use the `NbtTag` class.
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
