---
title: Online (Private Key)
description: Learn how to configure the Online (Private Key) forwarding.
sidebar:
  order: 3
---

Online (Private Key) forwarding is a type of forwarding player data developed by [**Void**](https://github.com/caunt/Void).

:::tip
This forwarding is the hardest to setup, but does not break official Mojang authentication and enables Minecraft Encryption with the server.
:::

:::note[Mods]
Online (Private Key) forwarding is not supported by mod loaders, and currently have no community implementations.  
However, there is [**examples**](https://github.com/caunt/Void/blob/main/src/Servers/Bukkit/src/main/java/net/caunt/thevoid/EntryPoint.java) on how to enable it with **any** minecraft server, including modded.
:::

:::caution[Limitations]
- You have to install a plugin, mod, or server with built-in implementation.
:::

## Configuration
Since currently it has no community implementation, you have to set up it manually by creating a Void plugin and Minecraft plugin, mod or Java agent. 

First of all, you have to retrieve server's private key used to authenticate and encrypt network data.  

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

Second step is to **securely** send the private key to your Void proxy plugin.

:::caution[Security]
Ensure that you have a very secure and encrypted way to pass that key, or at least configured firewall rules to prevent unauthorized access to server key.  
This is **very important** step, as it can lead to **server compromise** if not done correctly.

Prefer using community or official implementations of Online forwarding, when they become available.
:::

Last step is to pass that key to [**Void**](https://github.com/caunt/Void) when player is connecting to that server.  
This key will be used to decrypt network traffic and provide protocol API to plugins.

Void will automatically search for the private key if `AuthenticationSide` is set to `Server`.

```csharp
[Subscribe(PostOrder.Last)]
public void OnAuthenticationStarting(AuthenticationStartingEvent @event)
{
    if (@event.Link.Server.Name is not "lobby")
        return;

    // Ensure Void targets authentication to server side,
    // So it allows player to authenticate with server.
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