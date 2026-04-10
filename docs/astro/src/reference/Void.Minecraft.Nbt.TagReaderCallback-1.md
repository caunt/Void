# <a id="Void_Minecraft_Nbt_TagReaderCallback_1"></a> Delegate TagReaderCallback<T\>

Namespace: [Void.Minecraft.Nbt](Void.Minecraft.Nbt.md)  
Assembly: Void.Minecraft.dll  

Handler for events used with the <xref href="Void.Minecraft.Nbt.TagReader" data-throw-if-not-resolved="false"></xref> class.

```csharp
public delegate void TagReaderCallback<in T>(TagReader reader, T args) where T : EventArgs
```

#### Parameters

`reader` [TagReader](Void.Minecraft.Nbt.TagReader.md)

The <xref href="Void.Minecraft.Nbt.TagReader" data-throw-if-not-resolved="false"></xref> instance invoking the event.

`args` T

Any relevant args to be supplied with the callback,

#### Type Parameters

`T` 

A type derived from <xref href="System.EventArgs" data-throw-if-not-resolved="false"></xref>.

