namespace Void.Proxy.Plugins.Common.Network.Protocol.Encryption;

public record EncryptionRequest(byte[] PublicKey, byte[] VerifyToken, string ServerId = "", bool ShouldAuthenticate = true);
