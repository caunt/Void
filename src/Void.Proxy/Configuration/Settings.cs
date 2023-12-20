using IniParser;
using IniParser.Model;
using Serilog.Events;
using System.Net;
using System.Text.RegularExpressions;
using Void.Proxy.Models.General;
using Void.Proxy.Network.Protocol.Forwarding;
using Void.Proxy.Properties;
using Void.Proxy.Utils;

namespace Void.Proxy.Configuration;

public partial class Settings(IniData data)
{
    private static readonly FileIniDataParser _parser = new();
    private static readonly SettingsDataFormatter _formatter = new();

    public int ConfigVersion { get; set; } = 1;
    public IPAddress Address { get; set; } = IPAddress.Any;
    public int Port { get; set; } = 25565;
    public int CompressionThreshold { get; set; } = 256;
    public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;
    public ForwardingMode ForwardingMode { get; set; } = ForwardingMode.Auto;
    public NoneForwarding DefaultNoneForwarding { get; set; } = new();
    public AutoForwarding DefaultAutoForwarding { get; set; } = new();
    public LegacyForwarding DefaultLegacyForwarding { get; set; } = new();
    public ModernForwarding DefaultModernForwarding { get; set; } = new(string.Empty);
    public List<ServerInfo> Servers { get; set; } = [];

    static Settings()
    {
        _parser.Parser.Configuration.CommentString = "# ";
        _parser.Parser.Configuration.CaseInsensitive = true;
    }

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

        // update config version if possible
        await settings.SaveAsync(fileName, cancellationToken);

        return settings;
    }

    public async Task SaveAsync(string fileName = "settings.ini", CancellationToken cancellationToken = default)
    {
        await File.WriteAllTextAsync(fileName, data.ToString(_formatter), cancellationToken);
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

        if (global.Keys.ContainsKey("config-version"))
        {
            var configVersionKeyData = global.Keys.GetKeyData("config-version");

            if (!int.TryParse(configVersionKeyData.Value, out var configVersion))
                throw new Exception("Invalid config version specified");

            ConfigVersion = configVersion;
        }

        if (global.Keys.ContainsKey("bind"))
        {
            var bindKeyData = global.Keys.GetKeyData("bind");
            var bind = bindKeyData.Value;

            var colonIdx = bind.IndexOf(':');

            if (colonIdx == 0)
                throw new Exception($"You must specify IP address to bind");

            if (!IPAddress.TryParse(bind[..colonIdx], out var address))
                throw new Exception("You must specify valid IP address to bind");

            if (colonIdx < 0 || !int.TryParse(bind[++colonIdx..], out var port))
                port = 25565;

            if (port is < 1 or > 65536)
                throw new Exception("Invalid bind port specified. Valid range: from 1 to 65536");

            Address = address;
            Port = port;
        }

        if (global.Keys.ContainsKey("compressionThreshold"))
        {
            var compressionThresholdKeyData = global.Keys.GetKeyData("compressionThreshold");

            if (!int.TryParse(compressionThresholdKeyData.Value, out var compressionThreshold) || compressionThreshold is < -1)
                throw new Exception($"Invalid compression threshold specified. Valid range: from -1 to {int.MaxValue}");

            CompressionThreshold = compressionThreshold;
        }

        if (global.Keys.ContainsKey("logLevel"))
        {
            var logLevelKeyData = global.Keys.GetKeyData("logLevel");

            if (!Enum.TryParse(logLevelKeyData.Value, true, out LogEventLevel logLevel))
                throw new Exception($"Invalid log level specified. Valid values: verbose, debug, information, warning, error, fatal");

            LogLevel = logLevel;
        }

        if (global.Keys.ContainsKey("forwarding"))
        {
            var forwardingKeyData = global.Keys.GetKeyData("forwarding");

            if (!Enum.TryParse(forwardingKeyData.Value, true, out ForwardingMode forwardingMode))
                throw new Exception($"Invalid forwarding mode specified. Valid values: none, auto, legacy, modern");

            ForwardingMode = forwardingMode;
        }

        foreach (var forwardingSettingsSection in data.Sections.Where(section => section.SectionName.StartsWith("FORWARDING.", StringComparison.InvariantCultureIgnoreCase)))
        {
            if (forwardingSettingsSection.SectionName.EndsWith("LEGACY", StringComparison.InvariantCultureIgnoreCase))
            {
                if (forwardingSettingsSection.Keys.ContainsKey("includeAddress"))
                {
                    var includeAddressKeyData = forwardingSettingsSection.Keys.GetKeyData("includeAddress");

                    if (!bool.TryParse(includeAddressKeyData.Value, out var includeAddress))
                        throw new Exception($"Invalid include address value specified. Valid values: true, false");

                    DefaultLegacyForwarding.IncludeAddress = includeAddress;
                }

                if (forwardingSettingsSection.Keys.ContainsKey("includeUuid"))
                {
                    var includeUuidKeyData = forwardingSettingsSection.Keys.GetKeyData("includeUuid");

                    if (!bool.TryParse(includeUuidKeyData.Value, out var includeUuid))
                        throw new Exception($"Invalid include uuid value specified. Valid values: true, false");

                    DefaultLegacyForwarding.IncludeUuid = includeUuid;
                }

                if (forwardingSettingsSection.Keys.ContainsKey("includeSkin"))
                {
                    var includeSkinKeyData = forwardingSettingsSection.Keys.GetKeyData("includeSkin");

                    if (!bool.TryParse(includeSkinKeyData.Value, out var includeSkin))
                        throw new Exception($"Invalid include skin value specified. Valid values: true, false");

                    DefaultLegacyForwarding.IncludeSkin = includeSkin;
                }
            }

            if (forwardingSettingsSection.SectionName.EndsWith("MODERN", StringComparison.InvariantCultureIgnoreCase))
            {
                if (forwardingSettingsSection.Keys.ContainsKey("secret"))
                {
                    var secretKeyData = forwardingSettingsSection.Keys.GetKeyData("secret");
                    DefaultModernForwarding.Secret = secretKeyData.Value;
                }
            }
        }

        if (data.Sections.ContainsSection("SERVERS"))
        {
            var servers = data.Sections.GetSectionData("SERVERS");

            foreach (var (serverName, serverAddress) in servers.Keys.Select(keyData => (keyData.KeyName, keyData.Value)))
            {
                var colonIdx = serverAddress.IndexOf(':');

                if (colonIdx == 0)
                    throw new Exception($"You must specify hostname for server {serverName}");

                var serverHostname = colonIdx == -1 ? serverAddress : serverAddress[..colonIdx];

                if (colonIdx < 0 || !int.TryParse(serverAddress[++colonIdx..], out var serverPort))
                    serverPort = 25565;

                Servers.Add(new(serverName, serverHostname, serverPort, ForwardingMode switch
                {
                    ForwardingMode.None => DefaultNoneForwarding,
                    ForwardingMode.Auto => DefaultAutoForwarding,
                    ForwardingMode.Legacy => DefaultLegacyForwarding,
                    ForwardingMode.Modern => DefaultModernForwarding,
                    _ => throw new ArgumentOutOfRangeException($"Invalid forwarding mode specified: {ForwardingMode}")
                }));
            }
        }

        FillIniData();
    }

    private void FillIniData()
    {
        var global = data.Sections.ContainsSection("SETTINGS") ? data.Sections.GetSectionData("SETTINGS") : new("SETTINGS") { Keys = data.Global };

        global.Keys.SetKeyData(new KeyData("config-version")
        {
            Value = "1",
            Comments = ["Config version. Do not change this."]
        });

        global.Keys.SetKeyData(new KeyData("bind")
        {
            Value = $"{Address}:{Port}",
            Comments = ["Address and port to bind proxy. Default is all network interfaces and port 25565."]
        });

        global.Keys.SetKeyData(new KeyData("compressionThreshold")
        {
            Value = CompressionThreshold.ToString(),
            Comments = ["Compressiong threshold. Specifies size of minecraft packets that should be compressed to clients. Accepts -1 to disable compression."]
        });

        global.Keys.SetKeyData(new KeyData("logLevel")
        {
            Value = LogLevel.ToString().ToLower(),
            Comments = ["Logging level to print in console output. Valid levels: verbose, debug, information, warning, error, fatal"]
        });

        global.Keys.SetKeyData(new KeyData("forwarding")
        {
            Value = ForwardingMode.ToString().ToLower(),
            Comments = ["Default forwarding mode to servers listed in this file below. Can be overwritten on per-server basis by plugins. Valid modes: none, auto, legacy, modern"]
        });

        var forwardingModernSection = data.Sections.ContainsSection("FORWARDING.MODERN") ? data.Sections.GetSectionData("FORWARDING.MODERN") : new("FORWARDING.MODERN");

        forwardingModernSection.Keys.SetKeyData(new KeyData("secret")
        {
            Value = DefaultModernForwarding.Secret,
            Comments = ["Secret to enable modern forwarding. This should match with your minecraft servers configs. If you do not have such a config, ignore this and use legacy forwarding or none."]
        });

        data.Sections.SetSectionData(forwardingModernSection.SectionName, forwardingModernSection);

        var forwardingLegacySection = data.Sections.ContainsSection("FORWARDING.LEGACY") ? data.Sections.GetSectionData("FORWARDING.LEGACY") : new("FORWARDING.LEGACY");

        forwardingLegacySection.Keys.SetKeyData(new KeyData("includeAddress")
        {
            Value = DefaultLegacyForwarding.IncludeAddress.ToString().ToLower(),
            Comments = ["Should proxy share user IP with minecraft server? Valid values: true/false"]
        });

        forwardingLegacySection.Keys.SetKeyData(new KeyData("includeUuid")
        {
            Value = DefaultLegacyForwarding.IncludeUuid.ToString().ToLower(),
            Comments = ["Should proxy share user UUID with minecraft server? Valid values: true/false"]
        });

        forwardingLegacySection.Keys.SetKeyData(new KeyData("includeSkin")
        {
            Value = DefaultLegacyForwarding.IncludeSkin.ToString().ToLower(),
            Comments = ["Should proxy share user skin with minecraft server? Valid values: true/false"]
        });

        data.Sections.SetSectionData(forwardingLegacySection.SectionName, forwardingLegacySection);

        var serversSection = data.Sections.ContainsSection("SERVERS") ? data.Sections.GetSectionData("SERVERS") : new("SERVERS");

        foreach (var serverInfo in Servers)
        {
            serversSection.Keys.SetKeyData(new KeyData(serverInfo.Name)
            {
                Value = $"{serverInfo.Host}:{serverInfo.Port}"
            });
        }

        data.Sections.SetSectionData(serversSection.SectionName, serversSection);
    }

    [GeneratedRegex(@"{RANDOM_STRING_(\d+)}")]
    private static partial Regex RandomStringRegex();
}
