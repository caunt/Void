---
title: Serializers
description: Learn about serializers in Void.
---

Serializers are used to convert structured data between different formats. 

## Text Components
Prefer using the [**Serialize methods**](/docs/developing-plugins/text-formatting/#converting-components) on [**Component**](/reference/Void.Minecraft.Components.Text.Component) for serialization.
- [**ComponentJsonSerializer**](/reference/Void.Minecraft.Components.Text.Serializers.ComponentJsonSerializer)
  - Converts **[Text Component](/docs/developing-plugins/text-formatting) to Json** or **Json to [Text Component](/docs/developing-plugins/text-formatting)**.
- [**ComponentLegacySerializer**](/reference/Void.Minecraft.Components.Text.Serializers.ComponentLegacySerializer)
  - Converts **[Text Component](/docs/developing-plugins/text-formatting) to [Legacy string](/docs/developing-plugins/text-formatting/#formatting-codes)** or **[Legacy string](/docs/developing-plugins/text-formatting/#formatting-codes) to [Text Component](/docs/developing-plugins/text-formatting)**.
- [**ComponentNbtSerializer**](/reference/Void.Minecraft.Components.Text.Serializers.ComponentNbtSerializer)
  - Converts **[Text Component](/docs/developing-plugins/text-formatting) to [Nbt](/docs/developing-plugins/nbt)** or **[Nbt](/docs/developing-plugins/nbt) to [Text Component](/docs/developing-plugins/text-formatting)**.

## NBT
Helpful to convert Json or Snbt to Nbt and vice versa.  
- [**NbtJsonSerializer**](/reference/Void.Minecraft.Nbt.Serializers.Json.NbtJsonSerializer)
  - Converts **[Nbt](/docs/developing-plugins/nbt) to Json** or **Json to [Nbt](/docs/developing-plugins/nbt)**.
- [**NbtStringSerializer**](/reference/Void.Minecraft.Nbt.Serializers.String.NbtStringSerializer)
  - Converts **[Nbt](/docs/developing-plugins/nbt) to [Snbt](/docs/developing-plugins/nbt/#snbt)** or **[Snbt](/docs/developing-plugins/nbt/#snbt) to [Nbt](/docs/developing-plugins/nbt)**.

:::caution[Prefer deserializing from Snbt]
JSON can't guarantee numeric types. In JSON, `{"value": 1}` leaves the number's type unspecified.

```json
{
    "value": 1
}
```

In SNBT, `value: 1b` explicitly sets it as a byte.

```json
{
    value: 1b
}
```
:::