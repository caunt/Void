namespace Void.Proxy.API;

public interface IServer
{
    public ILink Link { get; }
    public string? Brand { get; }

    public void SetBrand(string brand);
}