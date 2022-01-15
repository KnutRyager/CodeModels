using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Common.Extensions;

namespace Common.Util;

public static class FormatUtil
{
    private const string NewLine = "\r\n";

    public static string Lines(IEnumerable<string> strings, bool filterEmpty = false, int tabCount = 0)
        => string.Join(NewLine, filterEmpty ? strings.Where(x => !string.IsNullOrEmpty(x)) : tabCount == 0 ? strings : strings.Select(x => $"{Tabs(tabCount)}{x}"));
    public static string Lines(params string[] strings) => Lines(strings, true);
    public static string Property(string name, string value, int tabCount = 0) => $"${Tabs(tabCount)}{String(name)}: {value}";
    public static string String(string content, string spacing = "", bool onlyIfContent = false) => MakeScope(content, "\"", "\"", spacing, onlyIfContent);
    public static string Parentheses(string content, string spacing = "", bool onlyIfContent = false) => MakeScope(content, "(", ")", spacing, onlyIfContent);
    public static string Curly(string content, string spacing = NewLine, bool onlyIfContent = false, int tabCount = 0) => MakeScope(content, tabCount: tabCount, spacing: spacing, onlyIfContent: onlyIfContent);
    public static string Tabs(int tabCount, int tabWidth = 4) => new(' ', tabWidth * tabCount);
    public static string MakeScope(string content, string scopeStart = "{", string scopeEnd = "}", string spacing = " ", bool onlyIfContent = false, int tabCount = 0)
        => !(onlyIfContent && string.IsNullOrEmpty(content)) ? $"{Tabs(tabCount)}{scopeStart}{spacing}{content}{spacing}{scopeEnd}" : "";
    public static string FormatTabs(string content, string scopeStart = "{", string scopeEnd = "}")
    {
        var sb = new StringBuilder();
        var tabCount = 0;
        var splitted = content.Split(NewLine);
        foreach (var s in splitted)
        {
            tabCount -= Regex.Matches(s, scopeEnd).Count;
            sb.Append($"{Tabs(tabCount)}{s}{NewLine}");
            tabCount += Regex.Matches(s, scopeStart).Count;
        }
        return sb.ToString();
    }
    public static string Format(Type type) => $"{(type.IsGenericType ? type.Name[..^2] : type.Name)}{(type.IsGenericType ? $"<{string.Join(", ", type.GetGenericArguments().Select(Format))}>" : "")}";
}
