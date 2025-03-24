using Docker.DotNet;
using Docker.DotNet.Models;
using MCStatus;
using Nito.AsyncEx;
using System.Net.Sockets;
using System.Reflection;
using Void.Proxy.Api.Mojang.Minecraft.Network;

var version = ProtocolVersion.Latest;
var count = 1;
await StartDockerEnvironmentAsync(count: args.Length is 1 && int.TryParse(args[0], out count) ? count : count);

return;

async ValueTask RunEntryPointAsync()
{
    var assembly = typeof(Void.Proxy.Platform).Assembly;
    var programType = assembly.GetType(nameof(Program));
    var mainMethod = programType?.GetMethod("<Main>", BindingFlags.Static | BindingFlags.NonPublic);
    var invocationResult = mainMethod?.Invoke(null, [Array.Empty<string>()]);

    if (invocationResult is not Task task)
        return;

    await task;
}

async ValueTask StartDockerEnvironmentAsync(int count = 3, CancellationToken cancellationToken = default)
{
    var imageName = "itzg/minecraft-server";
    var imageTag = version > ProtocolVersion.MINECRAFT_1_13 ? "java21-jdk" : "java8-jdk";
    const string patches =
        """
        {
           "patches":[
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
           ]
        }
        """;

    using var docker = new DockerClientConfiguration().CreateClient();

    Console.WriteLine(@$"Starting {count} paper containers");
    var papers = await Enumerable.Range(1, count).Select(async index =>
    {
        var images = await docker.Images.ListImagesAsync(new ImagesListParameters { All = true }, cancellationToken);

        if (images.All(image => !image.RepoTags.Any(imageName.Equals)))
        {
            await docker.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = imageName,
                Tag = imageTag
            }, null, new Progress<JSONMessage>(), cancellationToken);
        }

        var name = $"paper-{index}";
        var port = (ushort)(25565 + index);

        while (true)
        {
            var found = await GetContainerAsync(name, cancellationToken);

            if (found is null)
                break;

            switch (found.State)
            {
                case "running":
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
            Image = imageName + ":" + imageTag,
            Name = name,
            Env =
            [
                "EULA=TRUE",
                "TYPE=PAPER",
                "MODE=CREATIVE",
                "ONLINE_MODE=FALSE",
                "OPS=caunt",
                "PATCH_DEFINITIONS=/tmp/patch.json",
                "VERSION=" + version switch
                {
                    var value when value == ProtocolVersion.MINECRAFT_1_21_2 => value.Names[1], // paper skipped 1.21.2
                    var value when value == ProtocolVersion.MINECRAFT_1_8 => value.Names[8], // paper first release is 1.8.8
                    var value => value.Names[0]
                }
            ],
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
            Entrypoint = ["/bin/bash", "-c", $"echo '{patches}' > /tmp/patch.json && timeout 300 /start"]
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

        Console.WriteLine(status);
        return created.ID;
    }).WhenAll();

    try
    {
        await RunEntryPointAsync();
    }
    finally
    {
        await papers.Select(async id => await docker.Containers.StopContainerAsync(id, new ContainerStopParameters())).WhenAll();
    }

    async ValueTask<ContainerListResponse?> GetContainerAsync(string name, CancellationToken cancellationToken)
    {
        var containers = await docker.Containers.ListContainersAsync(new ContainersListParameters { All = true }, cancellationToken);
        return containers.FirstOrDefault(container => container.Names.Any(containerName => containerName.EndsWith("/" + name)));
    }
}