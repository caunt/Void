using System;
using System.Net;

namespace Void.IntegrationTests.Infrastructure.Extensions;

public static class EndPointExtensions
{
    private const string DockerHost = "host.docker.internal";

    extension(EndPoint endPoint)
    {
        public (string Host, int Port) AsDockerHostPort => endPoint switch
        {
            DnsEndPoint dnsEndPoint when !OperatingSystem.IsLinux() && string.Equals(dnsEndPoint.Host, "localhost", StringComparison.OrdinalIgnoreCase) => (DockerHost, dnsEndPoint.Port),
            DnsEndPoint dnsEndPoint => (dnsEndPoint.Host, dnsEndPoint.Port),
            IPEndPoint ipEndPoint when !OperatingSystem.IsLinux() && IPAddress.IsLoopback(ipEndPoint.Address) => (DockerHost, ipEndPoint.Port),
            IPEndPoint ipEndPoint => (ipEndPoint.Address.ToString(), ipEndPoint.Port),
            _ => throw new NotSupportedException($"Unsupported endpoint type: {endPoint.GetType()}")
        };
    }
}
