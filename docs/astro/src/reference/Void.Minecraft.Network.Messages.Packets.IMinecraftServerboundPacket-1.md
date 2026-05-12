# <a id="Void_Minecraft_Network_Messages_Packets_IMinecraftServerboundPacket_1"></a> Interface IMinecraftServerboundPacket<T\>

Namespace: [Void.Minecraft.Network.Messages.Packets](Void.Minecraft.Network.Messages.Packets.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftServerboundPacket<out T> : IMinecraftServerboundPacket, IMinecraftPacket<T>, IMinecraftPacket, IMinecraftMessage, INetworkMessage, IDisposable where T : class, IMinecraftPacket
```

#### Type Parameters

`T` 

#### Implements

[IMinecraftServerboundPacket](Void.Minecraft.Network.Messages.Packets.IMinecraftServerboundPacket.md), 
[IMinecraftPacket<T\>](Void.Minecraft.Network.Messages.Packets.IMinecraftPacket\-1.md), 
[IMinecraftPacket](Void.Minecraft.Network.Messages.Packets.IMinecraftPacket.md), 
[IMinecraftMessage](Void.Minecraft.Network.Messages.IMinecraftMessage.md), 
[INetworkMessage](Void.Proxy.Api.Network.Messages.INetworkMessage.md), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

