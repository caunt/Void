---
title: Online (Private Key)
description: Learn how to configure the Online (Private Key) forwarding.
sidebar:
  order: 3
---

Online (Private Key) forwarding is a type of forwarding player data developed by [**Void**](https://github.com/caunt/Void).

:::tip
This forwarding is the hardest to set up, but does not break official Mojang authentication and enables Minecraft Encryption with the server.
:::

:::note[Mods]
Online (Private Key) forwarding is not supported by mod loaders, and currently has no community implementations.  
However, there are [**examples**](https://github.com/caunt/Void/blob/main/src/Servers/Bukkit/src/main/java/net/caunt/thevoid/EntryPoint.java) on how to enable it with **any** Minecraft server, including modded.
:::

:::caution[Limitations]
- You have to install a plugin, mod, or server with built-in implementation.
- Does not support redirections to online servers until the release of the [Transfer packet](https://minecraft.wiki/w/Java_Edition_protocol/Packets#Transfer_(configuration)) (1.20.5).
:::

## Configuration
As there is no community implementation yet, you need a plugin on both the server and the proxy.

First, obtain the server's private key that Minecraft uses for authentication and encryption.

```java
// Plugin.java
package net.caunt.thevoid;

import org.bukkit.Bukkit;
import org.bukkit.plugin.java.JavaPlugin;

import java.lang.reflect.InvocationTargetException;
import java.security.KeyPair;

public final class EntryPoint extends JavaPlugin {

    @Override
    public void onEnable() {
        var logger = getLogger();

        try {
            var server = Bukkit.getServer();
            var craftServerClass = server.getClass();
            var getDedicatedServerMethod = craftServerClass.getMethod("getServer");
            var dedicatedServer = getDedicatedServerMethod.invoke(server);
            var dedicatedServerClass = dedicatedServer.getClass();
            var getKeyPairMethod = dedicatedServerClass.getMethod("getKeyPair");

            var keyPair = (KeyPair)getKeyPairMethod.invoke(dedicatedServer);
            var publicKey = keyPair.getPublic();
            var privateKey = keyPair.getPrivate();

            logger.info("Public Key (Hex): " + bytesToHex(publicKey.getEncoded()));
            logger.info("Private Key (Hex): " + bytesToHex(privateKey.getEncoded()));
        } catch (NoSuchMethodException | IllegalAccessException | InvocationTargetException e) {
            throw new RuntimeException(e);
        }
    }

    public static String bytesToHex(byte[] bytes) {
        StringBuilder hexString = new StringBuilder();
        for (byte b : bytes) {
            String hex = Integer.toHexString(0xff & b);
            if (hex.length() == 1) {
                hexString.append('0');
            }
            hexString.append(hex);
        }
        return hexString.toString();
    }
}
```

Next, send the private key to your proxy plugin over a secure channel.

:::caution[Security]
Never expose the key publicly or you risk compromising the server.
:::

Finally, provide the key to [**Void**](https://github.com/caunt/Void) when the player connects so the proxy can decrypt traffic and offer its protocol API.

Void will automatically search for the private key if `AuthenticationSide` is set to `Server`.

```csharp
[Subscribe(PostOrder.Last)]
public void OnAuthenticationStarting(AuthenticationStartingEvent @event)
{
    if (@event.Link.Server.Name is not "lobby")
        return;

    // Ensure Void targets authentication to the server side,
    // allowing the player to authenticate with the server.
    @event.Result = AuthenticationSide.Server;
}

[Subscribe]
public async ValueTask OnSearchServerPrivateKey(SearchServerPrivateKeyEvent @event)
{
    if (@event.Server.Name is not "lobby")
        return;

    // Retrieve the private key from your server plugin or mod.
    byte[] privateKey = await ValueTask.FromResult(...);
    @event.Result = privateKey;
}
```
