using Void.Terminal;

var reader = new PromptReader();
Console.SetOut(reader.TextWriter);

reader.ResetStyle();
reader.HideCursor();

await Task.WhenAny(AskAsync(), FloodAsync()).Unwrap();

async Task AskAsync()
{
    await Task.Yield();

    while (true)
    {
        var line = await reader.ReadLineAsync();
        Console.WriteLine($"You entered: {line}");
    }
}

async Task FloodAsync()
{
    while (true)
    {
        await Task.Delay(2000);
        Console.WriteLine(new string('a', 200));
    }
}
