---
title: Serializers
description: Learn about serializes in Void.
---

Serializers are used to convert structured data between different formats. 

## Text Components
Prefer using [**`Serialize` methods**](../text-components/#converting-components) on `Component` for serialization.
- `ComponentJsonSerializer` 
  - Converts **[Text Component](https://minecraft.wiki/w/Text_component_format) to Json** or **Json to [Text Component](https://minecraft.wiki/w/Text_component_format)**.
- `ComponentLegacySerializer` 
  - Converts **[Text Component](https://minecraft.wiki/w/Text_component_format) to [Legacy string](https://minecraft.fandom.com/wiki/Formatting_codes)** or **[Legacy string](https://minecraft.fandom.com/wiki/Formatting_codes) to [Text Component](https://minecraft.wiki/w/Text_component_format)**.
- `ComponentNbtSerializer` 
  - Converts **[Text Component](https://minecraft.wiki/w/Text_component_format) to [Nbt](https://minecraft.wiki/w/NBT_format)** or **[Nbt](https://minecraft.wiki/w/NBT_format) to [Text Component](https://minecraft.wiki/w/Text_component_format)**.

## NBT
Helpful to convert Json or Snbt to Nbt and vice versa.  
- `NbtJsonSerializer` 
  - Converts **[Nbt](https://minecraft.wiki/w/NBT_format) to Json** or **Json to [Nbt](https://minecraft.wiki/w/NBT_format)**.
- `NbtStringSerializer` - 
  - Converts **[Nbt](https://minecraft.wiki/w/NBT_format) to [Snbt](https://minecraft.fandom.com/wiki/NBT_format#SNBT_format)** or **[Snbt](https://minecraft.fandom.com/wiki/NBT_format#SNBT_format) to [Nbt](https://minecraft.wiki/w/NBT_format)**.

:::caution[Prefer deserializing from Snbt]
Snbt specifies concrete types.  
Json is not recommended because it's properties types conversion is guessed by bounds of the value.  

```json
{
	"value": 1
}
```
This 'value' number does not specify is it a byte, short, int or long. So in deserialization time, parser checks if it firs in byte, short, int or long, and uses the first Nbt Tag Type that fits.

On other hand, Snbt specifies the type of the value. 
```json
{
	"value": 1b
}
```
:::