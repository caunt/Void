﻿using Void.Proxy.API;

namespace Void.Proxy;

public class Server(Link link) : IServer
{
    public ILink Link { get; } = link;
    public string? Brand { get; protected set; }

    public void SetBrand(string brand)
    {
        Brand = brand;
    }
}