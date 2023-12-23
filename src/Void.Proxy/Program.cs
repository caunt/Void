using Void.Proxy;

Proxy.Logger.Information("Hello, World!");

var cts = new CancellationTokenSource();
var force = false;

Console.CancelKeyPress += (sender, eventArgs) =>
{
    if (force)
        Environment.Exit(0);

    if (cts.IsCancellationRequested)
    {
        Proxy.Logger.Warning("Proxy is already stopping. Press again to confirm force stop.");
        force = true;
    }

    cts.Cancel();
    eventArgs.Cancel = true;
};

await Proxy.ExecuteAsync(cts.Token);