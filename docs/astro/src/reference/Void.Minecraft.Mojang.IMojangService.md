# <a id="Void_Minecraft_Mojang_IMojangService"></a> Interface IMojangService

Namespace: [Void.Minecraft.Mojang](Void.Minecraft.Mojang.md)  
Assembly: Void.Minecraft.dll  

Provides Mojang session authentication for online-mode players joining the proxy.

```csharp
public interface IMojangService
```

## Methods

### <a id="Void_Minecraft_Mojang_IMojangService_VerifyAsync_Void_Proxy_Api_Players_IPlayer_System_Threading_CancellationToken_"></a> VerifyAsync\(IPlayer, CancellationToken\)

Verifies that the player has authenticated with the Minecraft session server by computing a
Java-style SHA-1 server ID from the encryption shared secret and the proxy's RSA public key,
then querying <code>sessionserver.mojang.com/session/minecraft/hasJoined</code> with the player's
username and the computed server ID.
When offline mode is enabled, the verification step is skipped entirely and a
<xref href="Void.Minecraft.Profiles.GameProfile" data-throw-if-not-resolved="false"></xref> with an offline-mode UUID derived from the player's username is
returned immediately.

```csharp
ValueTask<GameProfile?> VerifyAsync(IPlayer player, CancellationToken cancellationToken = default)
```

#### Parameters

`player` [IPlayer](Void.Proxy.Api.Players.IPlayer.md)

The player to authenticate. The player's profile must already contain at least a username
before this method is called.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

A token to cancel the HTTP request to the session server.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[GameProfile](Void.Minecraft.Profiles.GameProfile.md)?\>

The <xref href="Void.Minecraft.Profiles.GameProfile" data-throw-if-not-resolved="false"></xref> returned by Mojang's session server if verification succeeds,
or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a> if the session server responds with HTTP 204 No Content, which
indicates that the player has not joined the session and authentication should be refused.

