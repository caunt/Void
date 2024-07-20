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
