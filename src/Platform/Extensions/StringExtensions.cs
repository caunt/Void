namespace Void.Proxy.Extensions;

public static class StringExtensions
{
    private static readonly char[] DefaultDelimiters = [',', ';', '\n'];

    extension(string source)
    {
        public IEnumerable<string> SplitInput(char[]? delimiters = null, char? escapeCharacter = null, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        {
            if (string.IsNullOrEmpty(source))
                yield break;

            delimiters ??= DefaultDelimiters;

            if (escapeCharacter is null)
            {
                foreach (var token in source.Split(delimiters, options))
                    yield return token;

                yield break;
            }

            var tokenStartIndex = 0;

            for (var currentIndex = 0; currentIndex < source.Length; currentIndex++)
            {
                if (source[currentIndex] == escapeCharacter)
                {
                    currentIndex++;
                    continue;
                }

                if (delimiters.IndexOf(source[currentIndex]) >= 0)
                {
                    var result = ProcessToken(source, tokenStartIndex, currentIndex - tokenStartIndex, escapeCharacter.Value, options);

                    if (result is not null)
                        yield return result;

                    tokenStartIndex = currentIndex + 1;
                }
            }

            var lastToken = ProcessToken(source, tokenStartIndex, source.Length - tokenStartIndex, escapeCharacter.Value, options);

            if (lastToken is not null)
                yield return lastToken;

            yield break;

            static string? ProcessToken(string inputString, int startIndex, int length, char escapeCharacter, StringSplitOptions options)
            {
                var tokenSpan = inputString.AsSpan(startIndex, length);

                if (options.HasFlag(StringSplitOptions.TrimEntries))
                    tokenSpan = tokenSpan.Trim();

                if (tokenSpan.IsEmpty)
                    return options.HasFlag(StringSplitOptions.RemoveEmptyEntries) ? null : string.Empty;

                if (tokenSpan.IndexOf(escapeCharacter) < 0)
                    return tokenSpan.ToString();

                var unescapedCharacters = new char[tokenSpan.Length];
                var writeIndex = 0;

                for (var readIndex = 0; readIndex < tokenSpan.Length; readIndex++)
                {
                    var currentCharacter = tokenSpan[readIndex];

                    if (currentCharacter == escapeCharacter && readIndex + 1 < tokenSpan.Length)
                    {
                        readIndex++;
                        unescapedCharacters[writeIndex++] = tokenSpan[readIndex];
                    }
                    else
                    {
                        unescapedCharacters[writeIndex++] = currentCharacter;
                    }
                }

                return new string(unescapedCharacters, 0, writeIndex);
            }
        }
    }
}
