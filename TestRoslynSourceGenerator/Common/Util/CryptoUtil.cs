using System.Security.Cryptography;

namespace Common.Util;

public static class CryptoUtil
{
    private static SHA256 _sha256 = SHA256.Create();

    public static byte[] Hash(byte[] data)
    {
        try
        {
            return _sha256.ComputeHash(data);
        }
        catch (Exception)
        {
            _sha256 = SHA256.Create();
            return _sha256.ComputeHash(data);
        }
    }

    public static byte[] Hash(string? data) => Hash(Str2Hash(data ?? ""));
    public static byte[] Hash(object? data) => Hash(data?.ToString() ?? "");
    public static byte[] Hash(params object?[] data)
    {
        var list = new List<byte>();
        list.AddRange(data.SelectMany(Hash));
        return Hash(list.ToArray());
    }
    public static string HashStr(byte[] data) => Hash2Str(Hash(data));
    public static string HashStr(string? data) => Hash2Str(Hash(data));
    public static string HashStr(object data) => Hash2Str(Hash(data));
    public static string HashStr(params object?[] data) => Hash2Str(Hash(data));

    private static byte[] Str2Hash(string data) => System.Text.Encoding.UTF8.GetBytes(data);
    private static string Hash2Str(byte[] data) => BitConverter.ToString(data).Replace("-", string.Empty);
}
