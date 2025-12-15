namespace Void.Proxy.Api.Network;

[Flags]
public enum Operation
{
    Read = 1,
    Write = 2,
    Any = Read | Write
}
