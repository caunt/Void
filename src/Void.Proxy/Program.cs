using Void.Proxy;

Proxy.Logger.Information("Hello, World!");

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, eventArgs) =>
{
    cts.Cancel();
    eventArgs.Cancel = true;
};

await Proxy.ExecuteAsync(cts.Token);