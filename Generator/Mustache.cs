using System;
using System.Text;
using Common.Extensions;

namespace CodeAnalyzation
{
    public static class Mustache
    {

        public static string RemoveMustache(this string str)
        {
            if (!str.Contains("{{")) return str;
            var results = new StringBuilder();
            var parts = str.Split("{{");
            results.Append(parts[0]);
            for (var i = 1; i < parts.Length; i++)
            {
                var current = parts[i];
                if (current[0] == '{') current = current[1..];
                var splitted = current.Split("}}");
                if (splitted.Length != 2)
                {
                    throw new ArgumentException($"Invalid Mustache, closing split count was not 2 but: {splitted.Length}. {str}");
                }
                var content = splitted[0];
                var mustacheTokenType = content[0];
                switch (mustacheTokenType)
                {
                    case '!': break;
                    case '#': break;
                    case '^': break;
                    case '>': break;
                    case '/': break;
                    case '=': throw new ArgumentException("Mustache delimiter change not supported.");
                    default:
                        {
                            results.Append($"MUSTACHE_{content}");
                            break;
                        }
                }
                var textAfter = splitted[1];
                if (textAfter.Length > 0 && textAfter[0] == '}') textAfter = textAfter.Substring(1);
                results.Append(textAfter);
            }

            return results.ToString();
        }
    }
}