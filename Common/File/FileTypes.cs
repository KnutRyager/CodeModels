using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Files;

public static class FileTypes
{
    public const string Text = "txt";
    public const string Csv = "csv";
    public const string Excel = "xlsx";
    public const string Cpp = "cpp";
    public const string CSharp = "cs";

    public static class HttpContentTypes
    {
        public const string Excel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string Csv = "text/csv";
    }

    public static readonly List<string> TableTypes = new() { Csv, Excel };
    public static readonly IDictionary<FileType, List<string>> AllFileTypes = new Dictionary<FileType, List<string>>()
        {
            {FileType.Table,TableTypes }
        };
    //public static readonly List<List<string>> AllFileTypes = new List<List<string>>() { TableTypes };

    public static FileType ClassifyFileExtension(string extension) => AllFileTypes.Where(x => x.Value.Contains(extension)).Select(x => x.Key).Aggregate(FileType.None, (acc, x) => x);
    public static FileType ClassifyFileName(string fileName) => ClassifyFileExtension(FileUtil.GetFileExtension(fileName));


    public static string Extension(this TableType tableType) => tableType switch
    {
        TableType.Csv => Csv,
        TableType.Excel => Excel,
        _ => throw new ArgumentException($"Unhandled TableType: '{tableType}'")
    };

    public static string HttpContentType(string extension) => extension switch
    {
        Csv => HttpContentTypes.Csv,
        Excel => HttpContentTypes.Excel,
        _ => throw new ArgumentException($"HttpContentType: Unhandled extension: '{extension}'")
    };

    public static TableType GetTableType(string extension) => extension switch
    {
        Csv => TableType.Csv,
        Excel => TableType.Excel,
        _ => TableType.None
    };
}

public enum FileType
{
    None,
    Text,
    Table
}

public enum TableType
{
    None,
    Csv,
    Excel
}
