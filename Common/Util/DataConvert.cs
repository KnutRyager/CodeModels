using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.DataStructures;
using Common.Extensions;

namespace Common.Util;

public static class DataConvert
{
    //Single steps
    public static byte[] Text2Bytes(string str) => Encoding.UTF8.GetBytes(str);
    public static byte[] Stream2Bytes(Stream stream) => Text2Bytes(Stream2Text(stream));
    public static string Bytes2Text(byte[] bytes) => Stream2Text(Bytes2Stream(bytes));
    public static string Stream2Text(MemoryStream stream) => Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
    public static string[] Text2Array(string str) => str.Split(Constants.FILE_NEW_LINE);
    public static Stream Bytes2Stream(byte[] bytes, Stream? stream = null)
    {
        if (stream == null) stream = new MemoryStream();
        stream.Write(bytes, 0, bytes.Length);
        stream.Seek(0, SeekOrigin.Begin);
        stream.Flush();
        return stream;
    }
    public static FileStream Path2FileStream(string path, FileMode fileMode = FileMode.Open) => new(path, fileMode);
    public static Stream Text2Stream(string str, Stream? stream = null) => Bytes2Stream(Text2Bytes(str), stream);
    public static StreamWriter Stream2StreamWriter(Stream stream) => new(stream);
    public static StreamWriter StreamWriter2Text(string str) => Stream2StreamWriter(Text2Stream(str));
    public static StreamReader Text2StreamReader(string str) => new(new MemoryStream(Text2Bytes(str)));
    public static string Stream2Text(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var text = reader.ReadToEnd();
        return text;
    }

    public static List<string> Text2Lines(string text) => text.Split(Constants.FILE_NEW_LINE).ToList();
    public static List<List<string>> Lines2Table(List<string> lines, string separator = Constants.DEFAULT_SEPARATOR) => lines.Select(x => x.Split(separator).ToList()).ToList();
    public static List<T> Table2Type<T>(List<List<string>> table, CanonicalNameConverter? canonicalNames = null) where T : new() => DataParsing.Parse<T>(canonicalNames?.ConvertTable(table) ?? table);
    public static List<(T1, T2)> Table2Tuple<T1, T2>(List<List<string>> table) => DataParsing.ParseTuple<T1, T2>(table);
    public static List<(T1, T2, T3)> Table2Tuple<T1, T2, T3>(List<List<string>> table) => DataParsing.ParseTuple<T1, T2, T3>(table);
    public static List<(T1, T2, T3, T4)> Table2Tuple<T1, T2, T3, T4>(List<List<string>> table) => DataParsing.ParseTuple<T1, T2, T3, T4>(table);
    public static List<(T1, T2, T3, T4, T5)> Table2Tuple<T1, T2, T3, T4, T5>(List<List<string>> table) => DataParsing.ParseTuple<T1, T2, T3, T4, T5>(table);


    public static IEnumerable<IEnumerable<object>> Type2Table<T>(IEnumerable<T>? content) => DataParsing.DeParse(content, true);
    public static List<string> Table2Lines(IEnumerable<IEnumerable<object?>> table, string separator = Constants.DEFAULT_SEPARATOR) => table.Select(x => string.Join(separator, x)).ToList();
    public static string Lines2Text(IEnumerable<string> lines) => string.Join(Constants.FILE_NEW_LINE, lines);


    // Multi steps
    public static List<List<string>> Stream2Table(Stream stream, string separator = Constants.DEFAULT_SEPARATOR) => Text2Table(Stream2Text(stream), separator);
    public static List<T> Stream2Type<T>(Stream stream, string separator = Constants.DEFAULT_SEPARATOR) where T : new() => Text2Type<T>(Stream2Text(stream), separator);
    public static List<List<string>> Text2Table(string text, string separator = Constants.DEFAULT_SEPARATOR) => Lines2Table(Text2Lines(text), separator);
    public static List<T> Text2Type<T>(string text, string separator = Constants.DEFAULT_SEPARATOR, CanonicalNameConverter? canonicalNames = null) where T : new() => Table2Type<T>(Text2Table(text, separator), canonicalNames);
    public static List<(T1, T2)> Text2Tuple<T1, T2>(string text, string separator = Constants.DEFAULT_SEPARATOR) => Table2Tuple<T1, T2>(Text2Table(text, separator));
    public static List<(T1, T2, T3)> Text2Tuple<T1, T2, T3>(string text, string separator = Constants.DEFAULT_SEPARATOR) => Table2Tuple<T1, T2, T3>(Text2Table(text, separator));
    public static List<(T1, T2, T3, T4)> Text2Tuple<T1, T2, T3, T4>(string text, string separator = Constants.DEFAULT_SEPARATOR) => Table2Tuple<T1, T2, T3, T4>(Text2Table(text, separator));
    public static List<(T1, T2, T3, T4, T5)> Text2Tuple<T1, T2, T3, T4, T5>(string text, string separator = Constants.DEFAULT_SEPARATOR) => Table2Tuple<T1, T2, T3, T4, T5>(Text2Table(text, separator));

    public static List<string> Type2Lines<T>(IEnumerable<T> content, string separator = Constants.DEFAULT_SEPARATOR) => Table2Lines(Type2Table(content), separator);
    public static string Type2Text<T>(IEnumerable<T> content, string separator = Constants.DEFAULT_SEPARATOR) => Lines2Text(Type2Lines(content, separator));
    public static Stream Stream2Stream(Stream input, Stream? output = null)
    {
        if (output == null) output = new MemoryStream();
        input.CopyTo(output);
        output.Seek(0, SeekOrigin.Begin);
        return output;
    }
    public static Stream Type2Stream<T>(IEnumerable<T> content, Stream stream, string separator = Constants.DEFAULT_SEPARATOR) => Text2Stream(Type2Text(content, separator), stream);
    public static Stream Table2Stream(IEnumerable<IEnumerable<object?>> content, string separator = Constants.DEFAULT_SEPARATOR) => Text2Stream(Table2Text(content, separator));
    public static Stream Table2Stream(IEnumerable<IEnumerable<object?>> content, Stream? stream = null, string separator = Constants.DEFAULT_SEPARATOR) => Text2Stream(Table2Text(content, separator), stream);
    public static string Table2Text(IEnumerable<IEnumerable<object?>> content, string separator = Constants.DEFAULT_SEPARATOR) => Lines2Text(Table2Lines(content, separator));

}
