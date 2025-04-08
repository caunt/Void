using Docker.DotNet;
using Docker.DotNet.Models;
using MCStatus;
using Nito.AsyncEx;
using System.Net.Sockets;
using System.Reflection;
using Void.Minecraft.Network;

var version = ProtocolVersion.Latest;
var count = 1;

if (args.Length is 1 && int.TryParse(args[0], out var value))
    count = value;

Console.WriteLine(@$"Starting {count} paper containers");
await StartDockerPaperEnvironmentAsync(version, count);

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

async ValueTask StartDockerPaperEnvironmentAsync(ProtocolVersion version, int count = 3, CancellationToken cancellationToken = default)
{
    var imageName = "itzg/minecraft-server";
    var imageTag = version switch
    {
        _ when version >= ProtocolVersion.MINECRAFT_1_20_5 => "java21-jdk",
        _ when version >= ProtocolVersion.MINECRAFT_1_17 => "java17-jdk",
        _ => "java8-jdk"
    };
    var timeout = TimeSpan.FromSeconds(900);
    var variables = new List<string>
    {
        "EULA=TRUE",
        "TYPE=PAPER",
        "MODE=CREATIVE",
        "ONLINE_MODE=FALSE",
        "OPS=caunt,Shonz1",
        "PATCH_DEFINITIONS=/tmp/patch.json",
        "VERSION=" + VersionStringName(version)
    };

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
            Image = imageName + ":" + imageTag,
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
        await RunEntryPointAsync();
    }
    finally
    {
        // await papers.Select(async id => await docker.Containers.StopContainerAsync(id, new ContainerStopParameters())).WhenAll();
    }

    async ValueTask<ContainerListResponse?> GetContainerAsync(string name, CancellationToken cancellationToken)
    {
        var containers = await docker.Containers.ListContainersAsync(new ContainersListParameters { All = true }, cancellationToken);
        return containers.FirstOrDefault(container => container.Names.Any(containerName => containerName.EndsWith("/" + name)));
    }

    string VersionStringName(ProtocolVersion version)
    {
        return version switch
        {
            var value when value == ProtocolVersion.MINECRAFT_1_21_2 => value.Names[1], // paper skipped 1.21.2
            var value when value == ProtocolVersion.MINECRAFT_1_20_5 => value.Names[1], // paper skipped 1.20.5
            var value when value == ProtocolVersion.MINECRAFT_1_18 => value.Names[1], // paper skipped 1.18
            var value when value == ProtocolVersion.MINECRAFT_1_8 => value.Names[8], // paper first release is 1.8.8
            var value => value.GetVersionIntroducedIn()
        };
    }
}
