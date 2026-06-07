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
            DnsEndPoint dnsEndPoint when IsLocalHost(dnsEndPoint.Host) => (DockerHost, dnsEndPoint.Port),
            DnsEndPoint dnsEndPoint => (dnsEndPoint.Host, dnsEndPoint.Port),
            IPEndPoint ipEndPoint when IPAddress.IsLoopback(ipEndPoint.Address) => (DockerHost, ipEndPoint.Port),
            IPEndPoint ipEndPoint => (ipEndPoint.Address.ToString(), ipEndPoint.Port),
            _ => throw new NotSupportedException($"Unsupported endpoint type: {endPoint.GetType()}")
        };

        private static bool IsLocalHost(string host)
        {
            return string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase);
        }
    }
}
