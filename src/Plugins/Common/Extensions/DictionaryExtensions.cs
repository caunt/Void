using System.Diagnostics.CodeAnalysis;

namespace Void.Proxy.Plugins.Common.Extensions;

public static class DictionaryExtensions
{
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
        public bool TryGetKey(TValue value, [MaybeNullWhen(false)] out TKey key)
        {
            if (value is not null)
            {
                foreach (var entry in dictionary)
                {
                    if (!value.Equals(entry.Value))
                        continue;

                    key = entry.Key;
                    return true;
                }
            }

            key = default;
            return false;
        }
    }
}
