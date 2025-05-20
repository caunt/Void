---
title: Serializers
description: Learn about serializes in Void.
---

Serializers are used to convert structured data between different formats. 

## Text Components
Prefer using [**`Serialize` methods**](../text-formatting/#converting-components) on `Component` for serialization.
- `ComponentJsonSerializer` 
  - Converts **[Text Component](../text-formatting) to Json** or **Json to [Text Component](../text-formatting)**.
- `ComponentLegacySerializer` 
  - Converts **[Text Component](../text-formatting) to [Legacy string](../text-formatting#formatting-codes)** or **[Legacy string](../text-formatting#formatting-codes) to [Text Component](../text-formatting)**.
- `ComponentNbtSerializer` 
  - Converts **[Text Component](../text-formatting) to [Nbt](../nbt)** or **[Nbt](../nbt) to [Text Component](../text-formatting)**.

## NBT
Helpful to convert Json or Snbt to Nbt and vice versa.  
- `NbtJsonSerializer` 
  - Converts **[Nbt](../nbt) to Json** or **Json to [Nbt](../nbt)**.
- `NbtStringSerializer` - 
  - Converts **[Nbt](../nbt) to [Snbt](https://minecraft.fandom.com/wiki/NBT_format#SNBT_format)** or **[Snbt](https://minecraft.fandom.com/wiki/NBT_format#SNBT_format) to [Nbt](../nbt)**.

:::caution[Prefer deserializing from Snbt]
Json is not recommended because it's properties types conversion is guessed by bounds of the value.  

```json
{
	"value": 1
}
```
This 'value' number does not specify if it is a byte, short, int or long. So in deserialization time, parser checks if it fits in byte, short, int or long, and uses the first Nbt Tag Type that fits.

On other hand, Snbt specifies concrete type of the value. 
```json
{
	"value": 1b
}
```
:::