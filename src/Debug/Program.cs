using System.Net.Sockets;
using Docker.DotNet;
using Docker.DotNet.Models;
using MCStatus;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Void.Minecraft.Network;
using Void.Proxy;

if (OperatingSystem.IsWindows())
    Console.Clear();

var patchModernForwarding = false;
var docker = true;
var version = ProtocolVersion.Latest;
var timeout = TimeSpan.FromSeconds(900);

IDockerMinecraftServer[] servers =
[
    new PaperServer(version, 25566),
    new PaperServer(version, 25567),
    new PaperServer(version, 25568)
];

string[] arguments = [
    "--read-only",
    "--logging", nameof(LogLevel.Debug),
    "--forwarding-modern-key", "aaa",
    "--ignore-file-servers",
    "--port", "25565",
    "--server", "127.0.0.1:25566",
    "--server", "127.0.0.1:25567",
    "--server", "127.0.0.1:25568"
];

Console.WriteLine(@$"Starting {servers.Length} minecraft container(s)");

if (docker)
    await StartDockerEnvironmentAsync(servers, arguments: [.. arguments]);
else
    await EntryPoint.RunAsync(new EntryPoint.RunOptions { Arguments = [.. arguments] });

return;

async ValueTask StartDockerEnvironmentAsync(IEnumerable<IDockerMinecraftServer> servers, string[]? arguments = null, CancellationToken cancellationToken = default)
{

    const string imageName = "itzg/minecraft-server";
    var patches =
        $$"""
        {
           "patches": [
            {{(
            patchModernForwarding ?
            """
            {
                 "file":"/data/config/paper-global.yml",
                 "ops":[
                    {
                       "$set":{
                          "path":"$.proxies.velocity.enabled",
                          "value":true
                       }
                    },
                    {
                       "$set":{
                          "path":"$.proxies.velocity.secret",
                          "value":"aaa"
                       }
                    }
                 ]
              },
            """
            : string.Empty)}}

            {{(
            patchModernForwarding ?
            """
              {
                 "file":"/data/config/paper.yml",
                 "ops":[
                    {
                       "$set":{
                          "path":"$.settings.velocity-support.enabled",
                          "value":true
                       }
                    },
                    {
                       "$set":{
                          "path":"$.proxies.velocity.online-mode",
                          "value":true
                       }
                    },
                    {
                       "$set":{
                          "path":"$.settings.velocity-support.secret",
                          "value":"aaa"
                       }
                    }
                 ]
              },
            """
            : string.Empty)}}

            {{"""
              {
                 "file":"/data/config/spigot.yml",
                 "ops":[
                    {
                       "$set":{
                          "path":"$.settings.debug",
                          "value":true
                       }
                    }
                 ]
               }
            """}}
           ]
        }
        """;

    using var docker = new DockerClientConfiguration().CreateClient();

    var containers = await servers.Select(async server =>
    {
        var imageTag = server.ProtocolVersion switch
        {
            // _ when server.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_20_5 => "java21-jdk",
            // _ when server.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_18 => "java17-jdk", // doesn't support patches
            _ when server.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_18 => "java21-jdk",
            _ when server.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_17 => "java16",
            _ => "java8-jdk"
        };

        var variables = new List<string>
        {
            "EULA=TRUE",
            "TYPE=" + server.ItzgType,
            "MODE=CREATIVE",
            "ONLINE_MODE=FALSE",
            "OPS=caunt,Shonz1",
            "VERSION=" + VersionStringName(server.ProtocolVersion)
        };

        if (server is PaperServer)
        {
            variables.Add("PAPER_CONFIG_REPO=https://raw.githubusercontent.com/Shonz1/minecraft-default-configs/main");
            variables.Add("PATCH_DEFINITIONS=/tmp/patch.json");

            if (server.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_21_5)
                variables.Add("PAPER_CHANNEL=experimental");

            if (server.ProtocolVersion == ProtocolVersion.MINECRAFT_1_17)
                variables.Add("PAPER_DOWNLOAD_URL=https://api.papermc.io/v2/projects/paper/versions/1.17/builds/79/downloads/paper-1.17-79.jar");
            else if (server.ProtocolVersion == ProtocolVersion.MINECRAFT_1_17_1)
                variables.Add("PAPER_DOWNLOAD_URL=https://api.papermc.io/v2/projects/paper/versions/1.17.1/builds/411/downloads/paper-1.17.1-411.jar");
        }
        else if (server is ForgeServer forge && forge.Mods.Count > 0)
        {
            variables.Add("MODS=" + string.Join(',', forge.Mods));
        }

        var images = await docker.Images.ListImagesAsync(new ImagesListParameters { All = true }, cancellationToken);

        if (images.All(image => !image.RepoTags.Any(imageName.Equals)))
        {
            await docker.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = imageName,
                Tag = imageTag
            }, null, new Progress<JSONMessage>(), cancellationToken);
        }

        var name = $"void-{server.Port}";
        var port = (ushort)server.Port;

        while (true)
        {
            var found = await GetContainerAsync(name, cancellationToken);

            if (found is null)
                break;

            switch (found.State)
            {
                case "running":
                    var inspect = await docker.Containers.InspectContainerAsync(found.ID, cancellationToken);

                    if (variables.All(inspect.Config.Env.Contains))
                    {
                        var exec = await docker.Exec.ExecCreateContainerAsync(found.ID, new ContainerExecCreateParameters
                        {
                            AttachStdin = true,
                            AttachStdout = true,
                            AttachStderr = true,
                            Tty = true,
                            Cmd = ["/bin/bash", "-c", $"echo {(int)timeout.TotalSeconds} > /tmp/timeout"]
                        }, cancellationToken);

                        await docker.Exec.StartContainerExecAsync(exec.ID, cancellationToken);
                        return found.ID;
                    }

                    await docker.Containers.StopContainerAsync(found.ID, new ContainerStopParameters(), cancellationToken);
                    break;
                case "stopped":
                    await docker.Containers.RemoveContainerAsync(found.ID, new ContainerRemoveParameters(), cancellationToken);
                    break;
            }

            await Task.Delay(500, cancellationToken);
        }

        var createContainerParameters = new CreateContainerParameters
        {
            Image = $"{imageName}:{imageTag}",
            Name = name,
            Env = variables,
            OpenStdin = true,
            AttachStdin = true,
            Tty = true,
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                { "25565/tcp", default }
            },
            HostConfig = new HostConfig
            {
                AutoRemove = true,
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "25565/tcp", [new PortBinding { HostPort = port.ToString() }] }
                }
            },
            Entrypoint = ["/bin/bash", "-c", $"echo '{patches}' > /tmp/patch.json && echo {(int)timeout.TotalSeconds} > /tmp/timeout && /start & proc=$!; while kill -0 $proc 2>/dev/null; do sleep 5; timeleft=$(cat /tmp/timeout 2>/dev/null || echo 0); newtime=$((timeleft-5)); echo $newtime > /tmp/timeout; if [ $newtime -le 0 ]; then kill $proc; break; fi; done; wait $proc"]
        };

        var created = await docker.Containers.CreateContainerAsync(createContainerParameters, cancellationToken);

        if (!await docker.Containers.StartContainerAsync(created.ID, new ContainerStartParameters(), cancellationToken))
            throw new Exception($"Could not start #{created.ID} container");

        using var logStream = await docker.Containers.GetContainerLogsAsync(created.ID, createContainerParameters.Tty, new ContainerLogsParameters { ShowStdout = true, ShowStderr = true }, cancellationToken);

        StatusResponse status = default;
        while (status == default)
        {
            try
            {
                if (await GetContainerAsync(name, cancellationToken) is null)
                {
                    var (stdout, stderr) = await logStream.ReadOutputToEndAsync(cancellationToken);
                    throw new Exception($"Cannot start {name} container:\n\nstdout:\n{stdout}\n\nstderr:\n{stderr}");
                }

                status = await ServerListClient.GetStatusAsync("localhost", port);
                break;
            }
            catch (Exception exception) when (exception is SocketException or EndOfStreamException or IOException)
            {
                await Task.Delay(500, cancellationToken);
            }
            catch (Exception exception) when (exception.Message.Contains("Unexpected packet id"))
            {
                // TODO temporary ignore, library seems to not support 1.13- versions
                break;
            }
        }

        return created.ID;
    }).WhenAll();

    try
    {
        await EntryPoint.RunAsync(new EntryPoint.RunOptions { Arguments = arguments ?? [] }, cancellationToken);
    }
    finally
    {
        // await containers.Select(async id => await docker.Containers.StopContainerAsync(id, new ContainerStopParameters())).WhenAll();
    }

    async ValueTask<ContainerListResponse?> GetContainerAsync(string name, CancellationToken cancellationToken)
    {
        var containers = await docker.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true,
            Filters = new Dictionary<string, IDictionary<string, bool>>
            {
                {
                    "name", new Dictionary<string, bool> { ["/" + name] = true }
                }
            }
        }, cancellationToken);

        return containers.FirstOrDefault(container => container.Names.Any(n => n.EndsWith(name)));
    }

    string VersionStringName(ProtocolVersion version)
    {
        return version switch
        {
            var value when value == ProtocolVersion.MINECRAFT_1_21_2 => value.Names[1], // paper skipped 1.21.2
            var value when value == ProtocolVersion.MINECRAFT_1_20_5 => value.Names[1], // paper skipped 1.20.5
            var value when value == ProtocolVersion.MINECRAFT_1_20_3 => value.Names[1], // paper skipped 1.20.3
            var value when value == ProtocolVersion.MINECRAFT_1_18 => value.Names[1], // paper skipped 1.18
            var value when value == ProtocolVersion.MINECRAFT_1_8 => value.Names[8], // paper first release is 1.8.8
            var value => value.VersionIntroducedIn,
        };
    }
}

record PaperServer(ProtocolVersion ProtocolVersion, int Port) : IDockerMinecraftServer
{
    public string ItzgType => "PAPER";
}

record ForgeServer(ProtocolVersion ProtocolVersion, int Port, params IReadOnlyList<string> Mods) : IDockerMinecraftServer
{
    public string ItzgType => "FORGE";
}

interface IDockerMinecraftServer
{
    public ProtocolVersion ProtocolVersion { get; }
    public int Port { get; }
    public string ItzgType { get; }
}
