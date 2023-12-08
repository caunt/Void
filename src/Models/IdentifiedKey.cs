using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol;
using System.Security.Cryptography;
using System.Text;

namespace MinecraftProxy.Models;

public class IdentifiedKey(IdentifiedKeyRevision revision, long expiresAt, byte[] publicKey, byte[] signature)
{
    public static readonly byte[] YggdrasilSessionPublicKey = Convert.FromBase64String("MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAylB4B6m5lz7jwrcFz6Fd/fnfUhcvlxsTSn5kIK/2aGG1C3kMy4VjhwlxF6BFUSnfxhNswPjh3ZitkBxEAFY25uzkJFRwHwVA9mdwjashXILtR6OqdLXXFVyUPIURLOSWqGNBtb08EN5fMnG8iFLgEJIBMxs9BvF3s3/FhuHyPKiVTZmXY0WY4ZyYqvoKR+XjaTRPPvBsDa4WI2u1zxXMeHlodT3lnCzVvyOYBLXL6CJgByuOxccJ8hnXfF9yY4F0aeL080Jz/3+EBNG8RO4ByhtBf4Ny8NQ6stWsjfeUIvH7bU/4zCYcYOq4WrInXHqS8qruDmIl7P5XXGcabuzQstPf/h2CRAUpP/PlHXcMlvewjmGU6MfDK+lifScNYwjPxRo4nKTGFZf/0aqHCh/EAsQyLKrOIYRE0lDG3bzBh8ogIMLAugsAfBb6M3mqCqKaTMAf/VAjh5FFJnjS+7bE+bZEV0qwax1CEoPPJL1fIQjOS8zj086gjpGRCtSy9+bTPTfTR/SJ+VUB5G2IeCItkNHpJX2ygojFZ9n5Fnj7R9ZnOM+L8nyIjPu3aePvtcrXlyLhH/hvOfIOjPxOlqW+O5QwSFP4OEcyLAUgDdUgyW36Z5mB285uKW/ighzZsOTevVUG2QwDItObIV6i8RCxFbN2oDHyPaO5j1tTaBNyVt8CAwEAAQ==");
    
    public IdentifiedKeyRevision Revision { get; set; } = revision;
    public long ExpiresAt { get; set; } = expiresAt;
    public byte[] PublicKey { get; set; } = publicKey;
    public byte[] Signature { get; set; } = signature;

    public Guid? Guid { get; set; }
    public bool? IsSignatureValid { get => isSignatureValid.HasValue ? isSignatureValid : isSignatureValid = ValidateData(Guid ?? default); set => isSignatureValid = value; }
    private bool? isSignatureValid;

    public bool VerifyDataSignature(byte[] signature, byte[] data)
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

    public bool AddGuid(Guid guid)
    {
        if (guid == default)
            return false;

        if (Guid == default)
        {
            if (!ValidateData(guid))
                return false;

            IsSignatureValid = true;
            Guid = guid;

            return true;
        }

        return IsSignatureValid.HasValue && IsSignatureValid.Value && Guid.Equals(guid);
    }

    private bool ValidateData(Guid guid)
    {
        if (revision == IdentifiedKeyRevision.GENERIC_V1)
        {
            var publicKeyText = $"-----BEGIN RSA PUBLIC KEY-----\n{Convert.ToBase64String(PublicKey, Base64FormattingOptions.InsertLineBreaks)}\n-----END RSA PUBLIC KEY-----\n";
            var verify = Encoding.ASCII.GetBytes(ExpiresAt + publicKeyText.Replace("\r", string.Empty));
            
            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(YggdrasilSessionPublicKey, out _);

            return rsa.VerifyData(verify, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }
        else
        {
            if (guid == default)
                return false;

            var verify = new byte[PublicKey.Length + 24];
            var buffer = new MinecraftBuffer(verify);
            buffer.WriteGuid(guid);
            buffer.WriteLong(ExpiresAt);
            buffer.Write(PublicKey);

            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(YggdrasilSessionPublicKey, out _);

            return rsa.VerifyData(buffer.Span, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }
    }
}

public class IdentifiedKeyRevision(IEnumerable<IdentifiedKeyRevision> backwardsCompatibleTo, List<ProtocolVersion> applicableTo)
{
    public static readonly IdentifiedKeyRevision GENERIC_V1 = new(Enumerable.Empty<IdentifiedKeyRevision>(), [ProtocolVersion.MINECRAFT_1_19]);
    public static readonly IdentifiedKeyRevision LINKED_V2 = new(Enumerable.Empty<IdentifiedKeyRevision>(), [ProtocolVersion.MINECRAFT_1_19_1]);

    public IEnumerable<IdentifiedKeyRevision> BackwardsCompatibleTo { get; } = backwardsCompatibleTo;
    public List<ProtocolVersion> ApplicableTo { get; } = applicableTo;
}
