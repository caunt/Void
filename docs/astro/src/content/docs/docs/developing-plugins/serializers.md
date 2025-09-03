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
- `NbtStringSerializer`
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