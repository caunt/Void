using IniParser;
using IniParser.Model;
using Void.Proxy.Properties;

namespace Void.Proxy.Configuration;

public class Settings(IniData data)
{
    private static readonly FileIniDataParser _parser = new();

    public static async Task<Settings> LoadAsync(string fileName = "settings.ini", bool createDefault = true, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(fileName))
        {
            if (createDefault)
                await File.WriteAllTextAsync(fileName, Resources.settings_defaults);
            else
                throw new FileNotFoundException($"File {fileName} not found");
        }

        var data = await File.ReadAllTextAsync(fileName, cancellationToken);
        return new(_parser.Parser.Parse(data));
    }
}
