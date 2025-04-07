using System.Globalization;

namespace Void.Proxy.Utils;

[AttributeUsage(AttributeTargets.Assembly)]
public class BuildDateAttribute(string value) : Attribute
{
    public DateTime DateTime { get; } = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None);
}
