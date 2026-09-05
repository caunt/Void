namespace Void.Client;

/// <summary>Defines the complete external HTTP contract for the reusable game container.</summary>
internal static class ClientApiEndpoints
{
    private const string StatusPath = "/api/game/status";

    public static IEndpointRouteBuilder MapClientApi(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api");

        api.MapGet("/health", (GameCoordinator coordinator) => coordinator.IsHealthy
                ? Results.Text("ok")
                : Results.Problem("Game coordinator is not running", statusCode: StatusCodes.Status503ServiceUnavailable))
            .WithName("ClientHealth")
            .WithSummary("Reports whether the client API coordinator is ready.");

        api.MapGet("/game/status", (GameCoordinator coordinator) => Results.Ok(coordinator.Status))
            .WithName("GetGameStatus")
            .WithSummary("Returns the current game lifecycle and latest operation status.");

        api.MapGet("/game/diagnostics", async (SessionDiagnostics diagnostics, CancellationToken cancellationToken) => Results.Ok(await diagnostics.ListAsync(cancellationToken)))
            .WithName("ListGameDiagnostics")
            .WithSummary("Lists retained Minecraft sessions and diagnostic download URLs.");

        api.MapGet("/game/diagnostics/{sessionId:guid}", async Task<IResult> (Guid sessionId, SessionDiagnostics diagnostics, CancellationToken cancellationToken) =>
        {
            var archive = await diagnostics.DownloadAsync(sessionId, cancellationToken);
            return archive is null ? Results.NotFound() : Results.File(archive, "application/zip", $"client-diagnostics-{sessionId}.zip");
        })
            .WithName("DownloadGameDiagnostics")
            .WithSummary("Downloads retained evidence for a running or stopped Minecraft session.");

        api.MapGet("/game/players", async Task<IResult> (GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync("players", async () => Results.Ok(await coordinator.GetPlayersAsync(cancellationToken)), loggerFactory, cancellationToken))
            .WithName("GetGamePlayers")
            .WithSummary("Returns the live local player and all other players tracked in the current client world.");

        api.MapPut("/game/options", async Task<IResult> (HttpRequest request, GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync("options", async () =>
            {
                using var reader = new StreamReader(request.Body);
                var options = await reader.ReadToEndAsync(cancellationToken);
                await coordinator.WriteOptionsAsync(options, cancellationToken);
                return Results.NoContent();
            }, loggerFactory, cancellationToken))
            .Accepts<string>("text/plain")
            .WithName("SetGameOptions")
            .WithSummary("Atomically stores Minecraft options for current and future launches.");

        api.MapPost("/game/start/vanilla", async Task<IResult> (StartGameRequest request, GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync("start-vanilla", async () => Results.Accepted(StatusPath, await coordinator.StartVanillaAsync(request, cancellationToken)), loggerFactory, cancellationToken))
            .WithName("StartVanillaGame")
            .WithSummary("Starts a Mojang vanilla Minecraft version.");

        api.MapPost("/game/start/neoforge", async Task<IResult> (StartNeoForgeGameRequest request, GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync("start-neoforge", async () => Results.Accepted(StatusPath, await coordinator.StartNeoForgeAsync(request, cancellationToken)), loggerFactory, cancellationToken))
            .WithName("StartNeoForgeGame")
            .WithSummary("Starts a NeoForge Minecraft version, or the latest stable release when no version is given.");

        api.MapPost("/game/start/curseforge", async Task<IResult> (StartCurseForgeGameRequest request, GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync("start-curseforge", async () => Results.Accepted(StatusPath, await coordinator.StartCurseForgeAsync(request, cancellationToken)), loggerFactory, cancellationToken))
            .WithName("StartCurseForgeGame")
            .WithSummary("Starts a CurseForge modpack file.");

        api.MapPost("/game/stop", async Task<IResult> (GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync("stop", async () => Results.Ok(await coordinator.StopGameAsync(cancellationToken)), loggerFactory, cancellationToken))
            .WithName("StopGame")
            .WithSummary("Stops Minecraft and confirms that its process tree exited.");

        api.MapPost("/game/connect", async Task<IResult> (ConnectGameRequest request, GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync("connect", async () => Results.Ok(await coordinator.ConnectAsync(request, cancellationToken)), loggerFactory, cancellationToken))
            .WithName("ConnectGame")
            .WithSummary("Connects to a server and visually confirms an interactive game screen.");

        api.MapPost("/game/send-chat", async Task<IResult> (SendChatRequest request, GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync("send-chat", async () =>
            {
                await coordinator.SendChatAsync(request, cancellationToken);
                return Results.NoContent();
            }, loggerFactory, cancellationToken))
            .WithName("SendGameChat")
            .WithSummary("Sends and confirms chat input in the connected game.");

        api.MapGet("/game/screenshot", async Task<IResult> (GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync("screenshot", async () => Results.File(await coordinator.CaptureScreenshotAsync(cancellationToken), "image/png"), loggerFactory, cancellationToken))
            .WithName("CaptureGameScreenshot")
            .WithSummary("Captures the current Minecraft window as PNG.");

        return endpoints;
    }

    private static async Task<IResult> ExecuteAsync(string operation, Func<Task<IResult>> action, ILoggerFactory loggerFactory, CancellationToken cancellationToken)
    {
        try
        {
            return await action();
        }
        catch (GameCommandException exception)
        {
            return Results.Problem(exception.Message, statusCode: exception.StatusCode);
        }
        catch (GamePlayersException exception)
        {
            return Problem(operation, exception, exception.StatusCode, exception.Failure, loggerFactory);
        }
        catch (GameClientException exception)
        {
            return Problem(operation, exception, StatusCodes.Status500InternalServerError, exception.Failure, loggerFactory);
        }
        catch (TimeoutException exception)
        {
            return Problem(operation, exception, StatusCodes.Status504GatewayTimeout, ClientFailure.FromException("client.operation.timeout", operation, "timeout", exception), loggerFactory);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Results.Problem("The request was canceled", statusCode: StatusCodes.Status408RequestTimeout);
        }
        catch (Exception exception)
        {
            return Problem(operation, exception, StatusCodes.Status500InternalServerError, ClientFailure.FromException("client.operation.failed", operation, "api", exception), loggerFactory);
        }
    }

    private static IResult Problem(string operation, Exception exception, int statusCode, ClientFailure? failure, ILoggerFactory loggerFactory)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
            loggerFactory.CreateLogger("ClientApi").LogError(exception, "Client API {Operation} failed", operation);

        var extensions = failure is null ? null : new Dictionary<string, object?> { ["failure"] = failure };
        return Results.Problem(detail: exception.Message, statusCode: statusCode, title: $"Client {operation} failed", extensions: extensions);
    }
}
