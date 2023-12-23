namespace Void.Proxy.Models.General;

public class Server(Link link)
{
    public Link Link { get; } = link;
    public string? Brand { get; protected set; }

    public void SetBrand(string brand)
    {
        Brand = brand;
    }
}