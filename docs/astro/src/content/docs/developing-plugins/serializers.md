---
title: Serializers
description: Learn about serializes in Void.
---

Serializers are used to convert structured data between different formats. 

## Text Components
Prefer using [**`Serialize` methods**](/developing-plugins/text-formatting/#converting-components) on `Component` for serialization.
- `ComponentJsonSerializer` 
  - Converts **[Text Component](/developing-plugins/text-formatting) to Json** or **Json to [Text Component](/developing-plugins/text-formatting)**.
- `ComponentLegacySerializer` 
  - Converts **[Text Component](/developing-plugins/text-formatting) to [Legacy string](/developing-plugins/text-formatting#formatting-codes)** or **[Legacy string](/developing-plugins/text-formatting#formatting-codes) to [Text Component](/developing-plugins/text-formatting)**.
- `ComponentNbtSerializer` 
  - Converts **[Text Component](/developing-plugins/text-formatting) to [Nbt](/developing-plugins/nbt)** or **[Nbt](/developing-plugins/nbt) to [Text Component](/developing-plugins/text-formatting)**.

## NBT
Helpful to convert Json or Snbt to Nbt and vice versa.  
- `NbtJsonSerializer` 
  - Converts **[Nbt](/developing-plugins/nbt) to Json** or **Json to [Nbt](/developing-plugins/nbt)**.
- `NbtStringSerializer` - 
  - Converts **[Nbt](/developing-plugins/nbt) to [Snbt](/developing-plugins/nbt/#snbt)** or **[Snbt](/developing-plugins/nbt/#snbt) to [Nbt](/developing-plugins/nbt)**.

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
	value: 1b
}
```
:::