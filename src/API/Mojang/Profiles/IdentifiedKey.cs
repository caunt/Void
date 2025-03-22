using System.Security.Cryptography;
using System.Text;
using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;

namespace Void.Proxy.API.Mojang.Profiles;

public record IdentifiedKey(IdentifiedKeyRevision Revision, long ExpiresAt, byte[] PublicKey, byte[] Signature)
{
    public static readonly byte[] YggdrasilSessionPublicKey = Convert.FromBase64String("MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAylB4B6m5lz7jwrcFz6Fd/fnfUhcvlxsTSn5kIK/2aGG1C3kMy4VjhwlxF6BFUSnfxhNswPjh3ZitkBxEAFY25uzkJFRwHwVA9mdwjashXILtR6OqdLXXFVyUPIURLOSWqGNBtb08EN5fMnG8iFLgEJIBMxs9BvF3s3/FhuHyPKiVTZmXY0WY4ZyYqvoKR+XjaTRPPvBsDa4WI2u1zxXMeHlodT3lnCzVvyOYBLXL6CJgByuOxccJ8hnXfF9yY4F0aeL080Jz/3+EBNG8RO4ByhtBf4Ny8NQ6stWsjfeUIvH7bU/4zCYcYOq4WrInXHqS8qruDmIl7P5XXGcabuzQstPf/h2CRAUpP/PlHXcMlvewjmGU6MfDK+lifScNYwjPxRo4nKTGFZf/0aqHCh/EAsQyLKrOIYRE0lDG3bzBh8ogIMLAugsAfBb6M3mqCqKaTMAf/VAjh5FFJnjS+7bE+bZEV0qwax1CEoPPJL1fIQjOS8zj086gjpGRCtSy9+bTPTfTR/SJ+VUB5G2IeCItkNHpJX2ygojFZ9n5Fnj7R9ZnOM+L8nyIjPu3aePvtcrXlyLhH/hvOfIOjPxOlqW+O5QwSFP4OEcyLAUgDdUgyW36Z5mB285uKW/ighzZsOTevVUG2QwDItObIV6i8RCxFbN2oDHyPaO5j1tTaBNyVt8CAwEAAQ==");

    private bool? _isSignatureValid;

    public Uuid? ProfileUuid { get; set; }

    public bool? IsSignatureValid
    {
        get => _isSignatureValid ??= ValidateData(ProfileUuid ?? default);
        set => _isSignatureValid = value;
    }

    public bool VerifyDataSignature(ReadOnlySpan<byte> signature, params ReadOnlySpan<byte> data)
    {
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(PublicKey, out _);

            return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }

    public bool AddUuid(Uuid uuid)
    {
        var guid = uuid.AsGuid;

        if (guid == default)
            return false;

        var profileGuid = ProfileUuid?.AsGuid;

        if (profileGuid != null)
            return IsSignatureValid.HasValue && IsSignatureValid.Value && profileGuid.Equals(guid);

        if (!ValidateData(uuid))
            return false;

        IsSignatureValid = true;
        ProfileUuid = uuid;

        return true;
    }

    private bool ValidateData(Uuid uuid)
    {
        var guid = uuid.AsGuid;

        if (Revision == IdentifiedKeyRevision.GenericV1Revision)
        {
            var publicKeyText = $"-----BEGIN RSA PUBLIC KEY-----\n{Convert.ToBase64String(PublicKey, Base64FormattingOptions.InsertLineBreaks)}\n-----END RSA PUBLIC KEY-----\n";
            var verify = Encoding.ASCII.GetBytes(ExpiresAt + publicKeyText.Replace("\r", string.Empty));

            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(YggdrasilSessionPublicKey, out _);

            return rsa.VerifyData(verify, Signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }
        else
        {
            if (guid == default)
                return false;

            var verify = new byte[PublicKey.Length + 24].AsSpan();
            var buffer = new MinecraftBuffer(verify);
            buffer.WriteUuid(uuid);
            buffer.WriteLong(ExpiresAt);
            buffer.Write(PublicKey);

            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(YggdrasilSessionPublicKey, out _);

            return rsa.VerifyData(verify[..buffer.Position], Signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }
    }
}

public class IdentifiedKeyRevision(IEnumerable<IdentifiedKeyRevision> backwardsCompatibleTo, List<ProtocolVersion> applicableTo)
{
    public static readonly IdentifiedKeyRevision GenericV1Revision = new([], [ProtocolVersion.MINECRAFT_1_19]);

    public static readonly IdentifiedKeyRevision LinkedV2Revision = new([], [ProtocolVersion.MINECRAFT_1_19_1]);

    public IEnumerable<IdentifiedKeyRevision> BackwardsCompatibleTo { get; } = backwardsCompatibleTo;
    public List<ProtocolVersion> ApplicableTo { get; } = applicableTo;
}