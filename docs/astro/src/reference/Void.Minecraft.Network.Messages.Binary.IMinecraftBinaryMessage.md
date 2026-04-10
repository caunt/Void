# <a id="Void_Minecraft_Network_Messages_Binary_IMinecraftBinaryMessage"></a> Interface IMinecraftBinaryMessage

Namespace: [Void.Minecraft.Network.Messages.Binary](Void.Minecraft.Network.Messages.Binary.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftBinaryMessage : IMinecraftMessage, INetworkMessage, IDisposable
```

#### Implements

[IMinecraftMessage](Void.Minecraft.Network.Messages.IMinecraftMessage.md), 
[INetworkMessage](Void.Proxy.Api.Network.Messages.INetworkMessage.md), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

## Properties

### <a id="Void_Minecraft_Network_Messages_Binary_IMinecraftBinaryMessage_Id"></a> Id

```csharp
int Id { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Network_Messages_Binary_IMinecraftBinaryMessage_Stream"></a> Stream

```csharp
MemoryStream Stream { get; }
```

#### Property Value

 [MemoryStream](https://learn.microsoft.com/dotnet/api/system.io.memorystream)

