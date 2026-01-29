## Proxy Compatible Forge Plugin

Since the [**Velocity**](https://github.com/PaperMC/Velocity/) proxy does not support modded command argument types for chat commands, [**Proxy Compatible Forge**](https://github.com/adde0109/Proxy-Compatible-Forge/) was developed to provide compatibility between Forge mods and Velocity.

*PCF has more features than just chat command compatibility, such as Modern (Velocity) player-data forwarding, so many server admins use it in their setups.*

The way PCF modifies the chat command packet means it is no longer compatible with the client if you pass it through the proxy as-is.

**So we'll take care of repairing that packet for the client here.**
