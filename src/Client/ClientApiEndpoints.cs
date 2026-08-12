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

        api.MapPut("/game/options", async Task<IResult> (HttpRequest request, GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () =>
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
            await ExecuteAsync(async () => Results.Accepted(StatusPath, await coordinator.StartVanillaAsync(request, cancellationToken)), loggerFactory, cancellationToken))
            .WithName("StartVanillaGame")
            .WithSummary("Starts a Mojang vanilla Minecraft version.");

        api.MapPost("/game/start/neoforge", async Task<IResult> (StartNeoForgeGameRequest request, GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () => Results.Accepted(StatusPath, await coordinator.StartNeoForgeAsync(request, cancellationToken)), loggerFactory, cancellationToken))
            .WithName("StartNeoForgeGame")
            .WithSummary("Starts the latest stable NeoForge Minecraft version.");

        api.MapPost("/game/start/curseforge", async Task<IResult> (StartCurseForgeGameRequest request, GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () => Results.Accepted(StatusPath, await coordinator.StartCurseForgeAsync(request, cancellationToken)), loggerFactory, cancellationToken))
            .WithName("StartCurseForgeGame")
            .WithSummary("Starts a CurseForge modpack file.");

        api.MapPost("/game/stop", async Task<IResult> (GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () => Results.Ok(await coordinator.StopGameAsync(cancellationToken)), loggerFactory, cancellationToken))
            .WithName("StopGame")
            .WithSummary("Stops Minecraft and confirms that its process tree exited.");

        api.MapPost("/game/connect", async Task<IResult> (ConnectGameRequest request, GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () => Results.Ok(await coordinator.ConnectAsync(request, cancellationToken)), loggerFactory, cancellationToken))
            .WithName("ConnectGame")
            .WithSummary("Connects to a server and visually confirms an interactive game screen.");

        api.MapPost("/game/send-chat", async Task<IResult> (SendChatRequest request, GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () =>
            {
                await coordinator.SendChatAsync(request, cancellationToken);
                return Results.NoContent();
            }, loggerFactory, cancellationToken))
            .WithName("SendGameChat")
            .WithSummary("Sends and confirms chat input in the connected game.");

        api.MapGet("/game/screenshot", async Task<IResult> (GameCoordinator coordinator, ILoggerFactory loggerFactory, CancellationToken cancellationToken) =>
            await ExecuteAsync(async () => Results.File(await coordinator.CaptureScreenshotAsync(cancellationToken), "image/png"), loggerFactory, cancellationToken))
            .WithName("CaptureGameScreenshot")
            .WithSummary("Captures the current Minecraft window as PNG.");

        return endpoints;
    }

    private static async Task<IResult> ExecuteAsync(Func<Task<IResult>> action, ILoggerFactory loggerFactory, CancellationToken cancellationToken)
    {
        try
        {
            return await action();
        }
        catch (GameCommandException exception)
        {
            return Results.Problem(exception.Message, statusCode: exception.StatusCode);
        }
        catch (TimeoutException exception)
        {
            return Results.Problem(exception.Message, statusCode: StatusCodes.Status504GatewayTimeout);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Results.Problem("The request was canceled", statusCode: StatusCodes.Status408RequestTimeout);
        }
        catch (Exception exception)
        {
            loggerFactory.CreateLogger("ClientApi").LogError(exception, "Client API operation failed");
            return Results.Problem(exception.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
