namespace CodeAnalyzation.Utils;

public static class NamespaceUtils
{
    public static bool IsMemberAccess(string s) => s.Contains(".");
    public static string NamePart(string s) => IsMemberAccess(s) ? s[(s.LastIndexOf(".")+1)..] : s;
    public static string PathPart(string s) => IsMemberAccess(s) ? s[..s.LastIndexOf(".")] : "";
    public static string GetKeyAndName(string? key, string? name) => $"{key ?? ""}{(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(name) ? "" : ".")}{name ?? ""}";
}
