using IniParser;
using IniParser.Model;
using Serilog.Events;
using System.Text.RegularExpressions;
using Void.Proxy.Properties;

namespace Void.Proxy.Configuration;

public partial class Settings(IniData data)
{
    private static readonly FileIniDataParser _parser = new();

    public int Port { get; set; }
    public int CompressionThreshold { get; set; }
    public LogEventLevel LogLevel { get; set; }

    public static async Task<Settings> LoadAsync(string fileName = "settings.ini", bool createDefault = true, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(fileName))
        {
            if (createDefault)
                await GenerateDefaultAsync(fileName, cancellationToken);
            else
                throw new FileNotFoundException($"File {fileName} not found");
        }

        var data = await File.ReadAllTextAsync(fileName, cancellationToken);
        var settings = new Settings(_parser.Parser.Parse(data));

        settings.Load();

        return settings;
    }

    private static async ValueTask GenerateDefaultAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var defaults = RandomStringRegex().Replace(Resources.settings_defaults, match =>
        {
            const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + "0123456789" + "abcdefghijklmnopqrstuvxyz";

            var suffix = match.Groups[1].Value;

            if (!int.TryParse(suffix, out var size))
                throw new Exception($"Invalid random string length specified: {suffix}");

            return new string(Random.Shared.GetItems<char>(allowedChars, size));
        });

        await File.WriteAllTextAsync(fileName, defaults, cancellationToken);
    }

    private void Load()
    {
        var global = data.Sections.ContainsSection("SETTINGS") ? data.Sections.GetSectionData("SETTINGS") : new("SETTINGS") { Keys = data.Global };

        var portKeyData = global.Keys.GetKeyData("port");

        if (!int.TryParse(portKeyData.Value, out var port) || port is < 1 or > 65536)
            throw new Exception("Invalid port specified. Valid range: from 1 to 65536");

        Port = port;

        var compressionThresholdKeyData = global.Keys.GetKeyData("compressionThreshold");

        if (!int.TryParse(compressionThresholdKeyData.Value, out var compressionThreshold) || port is < -1 or > 65536)
            throw new Exception($"Invalid compression threshold specified. Valid range: from -1 to {int.MaxValue}");

        CompressionThreshold = compressionThreshold;

        var logLevelKeyData = global.Keys.GetKeyData("logLevel");

        if (!Enum.TryParse(logLevelKeyData.Value, true, out LogEventLevel logLevel))
            throw new Exception($"Invalid log level specified. Valid values: verbose, debug, information, warning, error, fatal");

        LogLevel = logLevel;

        /* 
        new()
        {
            { "server1", new ServerInfo("127.0.0.1", 25566, new NoneForwarding()) },
            { "server2", new ServerInfo("127.0.0.1", 25567, new NoneForwarding()) },
            { "server3", new ServerInfo("127.0.0.1", 25568, new NoneForwarding()) }
        };
        */
    }

    [GeneratedRegex(@"{RANDOM_STRING_(\d+)}")]
    private static partial Regex RandomStringRegex();
}
