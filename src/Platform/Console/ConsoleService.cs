using Serilog;
using Serilog.Events;
using System.Text;
using Void.Proxy.Api.Console;
namespace Void.Proxy.Console;

public class ConsoleService : IConsoleService
{
    private readonly StringBuilder _buffer = new();
    private int _bufferLines = 0;
    private int _width = 0;
    private string _emptyLine = string.Empty;
    private bool _visible = true;

    public void Setup()
    {
        // AnsiConsole.Cursor.Show(false);

        var configuration = new LoggerConfiguration();
        configuration.Enrich.FromLogContext();
        configuration.MinimumLevel.ControlledBy(Platform.LoggingLevelSwitch);
        configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
        configuration.WriteTo.Sink(new ConsoleSink(this, "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {NewLine}{Exception}"));

        Log.Logger = configuration.CreateLogger();

        // _ = Task.Factory.StartNew(async () =>
        // {
        //     while (true)
        //     {
        //         await Task.Delay(200);
        //         Log.Logger.Information(new string([.. Enumerable.Repeat('a', 50)])); // Random.Shared.Next(100)
        //     }
        // });
    }

    public void PrepareRender()
    {

    }

    public void Render(bool newLine = false)
    {
        Log.Logger.Information(ReadLine.ReadLine.Read(">"));
        Thread.Sleep(20);
    }

    // public void PrepareRender()
    // {
    //     lock (this)
    //         ClearBufferLine();
    // }
    // 
    // public void Render(bool newLine = false)
    // {
    //     lock (this)
    //     {
    //         if (HandleInput()) //  || RequiresRenderBuffer()
    //             ClearBufferLine();
    // 
    //         if (!_visible)
    //             PrintBuffer();
    //     }
    // 
    //     Thread.Sleep(20);
    // }
    // 
    // private bool HandleInput()
    // {
    //     while (SystemConsole.KeyAvailable)
    //     {
    //         var keyInfo = SystemConsole.ReadKey(true);
    // 
    //         if (keyInfo.Key == ConsoleKey.Enter)
    //         {
    //             var command = _buffer.ToString();
    //             _buffer.Clear();
    // 
    //             // ILogger cannot be injected here because Setup() called after ConsoleService constructor
    //             Log.Logger.ForContext<ConsoleService>().Information("Console issued command {Command}", command);
    //         }
    //         else if (keyInfo.Key == ConsoleKey.Backspace)
    //         {
    //             if (_buffer.Length is 0)
    //                 continue;
    // 
    //             _buffer.Length--;
    //             return true;
    //         }
    //         else
    //         {
    //             _buffer.Append(keyInfo.KeyChar);
    //             return true;
    //         }
    //     }
    // 
    //     return false;
    // }
    // 
    // private bool EnsureWidth()
    // {
    //     var width = AnsiConsole.Profile.Width;
    // 
    //     if (width != _width)
    //         _emptyLine = new string(' ', _buffer.Length);
    // 
    //     var less = width < _width;
    //     _width = width;
    // 
    //     return less;
    // }
    // 
    // private bool RequiresRenderBuffer()
    // {
    //     var less = EnsureWidth();
    //     var bufferNotFit = _width < _buffer.Length;
    // 
    //     return less && bufferNotFit;
    // }
    // 
    // private void PrintBuffer()
    // {
    //     AnsiConsole.Write(_buffer.ToString());
    //     AnsiConsole.Cursor.MoveLeft(int.MaxValue);
    //     _visible = true;
    //     _bufferLines = GetBufferLines();
    // }
    // 
    // private void ClearBufferLine()
    // {
    //     var less = EnsureWidth();
    //     var lines = _bufferLines;
    // 
    //     if (less && _width < _buffer.Length - 1)
    //         lines += 1;
    // 
    //     for (var i = 0; i < lines; i++)
    //     {
    //         AnsiConsole.Cursor.MoveLeft(int.MaxValue);
    //         AnsiConsole.Write(_emptyLine);
    //         AnsiConsole.Cursor.MoveLeft(int.MaxValue);
    // 
    //         if (i < lines - 1)
    //             AnsiConsole.Cursor.MoveUp(1);
    //     }
    // 
    //     _visible = false;
    // }
    // 
    // private int GetBufferLines()
    // {
    //     return Math.Max(1, (_buffer.Length + _width - 1) / _width);
    // }
    // 
    // private int GetCursorTop()
    // {
    //     return GetCursorPosition().Top;
    // }
    // 
    // private int GetCursorLeft()
    // {
    //     return GetCursorPosition().Left;
    // }
    // 
    // private (int Left, int Top) GetCursorPosition()
    // {
    //     return SystemConsole.GetCursorPosition();
    // }
}
