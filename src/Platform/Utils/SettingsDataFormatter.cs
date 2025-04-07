using IniParser.Model;
using IniParser.Model.Configuration;
using IniParser.Model.Formatting;
using System.Text;

namespace Void.Proxy.Utils;

internal class SettingsDataFormatter : IIniDataFormatter
{
    public IniParserConfiguration Configuration
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public string IniDataToString(IniData iniData)
    {
        var source = new StringBuilder();

        WriteProperties(iniData.Global, source);

        foreach (var section in iniData.Sections)
            WriteSection(section, source);

        var newLineLength = Environment.NewLine.Length;
        source.Remove(source.Length - newLineLength, newLineLength);

        return source.ToString();
    }

    protected virtual void WriteSection(SectionData section, StringBuilder source)
    {
        if (source.Length > 0)
            source.AppendLine();

        WriteComments(section.Comments, source);
        source.AppendLine($"[{section.SectionName}]");
        WriteProperties(section.Keys, source);
    }

    protected virtual void WriteProperties(KeyDataCollection properties, StringBuilder source)
    {
        foreach (var property in properties)
        {
            WriteComments(property.Comments, source);
            source.AppendLine($"{property.KeyName} = {property.Value}");
        }
    }

    protected virtual void WriteComments(List<string> comments, StringBuilder source)
    {
        foreach (var comment in comments)
            source.AppendLine($"# {comment}");
    }
}
