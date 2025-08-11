---
title: Serializers
description: Learn about serializers in Void.
---

Serializers are used to convert structured data between different formats. 

## Text Components
Prefer using [**`Serialize` methods**](/docs/developing-plugins/text-formatting/#converting-components) on `Component` for serialization.
- `ComponentJsonSerializer` 
  - Converts **[Text Component](/docs/developing-plugins/text-formatting) to Json** or **Json to [Text Component](/docs/developing-plugins/text-formatting)**.
- `ComponentLegacySerializer` 
  - Converts **[Text Component](/docs/developing-plugins/text-formatting) to [Legacy string](/docs/developing-plugins/text-formatting#formatting-codes)** or **[Legacy string](/docs/developing-plugins/text-formatting#formatting-codes) to [Text Component](/docs/developing-plugins/text-formatting)**.
- `ComponentNbtSerializer` 
  - Converts **[Text Component](/docs/developing-plugins/text-formatting) to [Nbt](/docs/developing-plugins/nbt)** or **[Nbt](/docs/developing-plugins/nbt) to [Text Component](/docs/developing-plugins/text-formatting)**.

## NBT
Helpful to convert Json or Snbt to Nbt and vice versa.  
- `NbtJsonSerializer` 
  - Converts **[Nbt](/docs/developing-plugins/nbt) to Json** or **Json to [Nbt](/docs/developing-plugins/nbt)**.
- `NbtStringSerializer` - 
  - Converts **[Nbt](/docs/developing-plugins/nbt) to [Snbt](/docs/developing-plugins/nbt/#snbt)** or **[Snbt](/docs/developing-plugins/nbt/#snbt) to [Nbt](/docs/developing-plugins/nbt)**.

:::caution[Prefer deserializing from Snbt]
Json is not recommended because its properties' type conversion is guessed by bounds of the value.  

```json
{
	"value": 1
}
```
This 'value' number does not specify if it is a byte, short, int or long. So in deserialization time, parser checks if it fits in byte, short, int or long, and uses the first Nbt Tag Type that fits.

On the other hand, Snbt specifies concrete type of the value. 
```json
{
	value: 1b
}
```
:::