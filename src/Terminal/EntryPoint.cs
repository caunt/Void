using Void.Terminal;

var reader = new PromptReader();
Console.SetOut(reader.TextWriter);

reader.ResetStyle();
reader.ShowCursor();
reader.Buffer.Append(new string('L', 150));

var i = 0;
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
        Console.WriteLine(i++ + new string('a', 200));
    }
}
