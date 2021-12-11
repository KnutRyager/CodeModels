using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Util;

namespace Common.Files
{
    public static class FileUtil
    {
        public const int DEFAULT_READ_RANGE = 100000;

        public static string DefaultAzureBlobAccount = "";
        public static string ProjectResourcePath = Constants.DEFAULT_RESOURCE_LOCATION;
        public static string RootInFolder = ProjectResourcePath;
        public static string RootOutFolder = $@"{Constants.DEFAULT_RESOURCE_LOCATION}Out\";

        // Read
        private static string Pathify(string? path) => Pathify(RootInFolder, path);
        public static string Pathify(string? rootPath, string? path)
        {
            var root = rootPath ?? "";
            var subPath = path ?? "";
            if (subPath.Contains(':')) root = "";
            var middle = !string.IsNullOrWhiteSpace(root) && !string.IsNullOrWhiteSpace(subPath) && !root.EndsWith(@"\") ? @"\" : "";
            return $"{root}{middle}{subPath}";
        }

        public static string Extensify(string path, string? extension) => $"{path}{(path.Contains('.') || string.IsNullOrEmpty(extension) ? "" : $".{extension}")}";
        public static string FileName(string filePath) => filePath.Substring(filePath.LastIndexOf("\\") + 1);
        public static string FilePath(string filePath) => filePath.Substring(0, Math.Max(0, filePath.LastIndexOf("\\")));
        public static string GetFileExtension(string path) => path.Contains('.') ? path.Substring(path.LastIndexOf(".") + 1) : "";
        public static string GetSubPath(string path, string root) => path.Substring(root.Length);


        public static string ReadFileToText(string path) => File.ReadAllText(Pathify(path));
        public static List<string> ReadFileToLines(string path) => DataConvert.Text2Lines(ReadFileToText(path));
        public static List<List<string>> ReadFileToTable(string path, string separator = Constants.DEFAULT_SEPARATOR) => DataConvert.Text2Table(ReadFileToText(path), separator);
        public static List<T> ReadFileToType<T>(string path, string separator = Constants.DEFAULT_SEPARATOR) where T : new() => DataConvert.Text2Type<T>(ReadFileToText(path), separator);
        public static List<(T1, T2)> ReadFileToTuple<T1, T2>(string path, string separator = Constants.DEFAULT_SEPARATOR) => DataConvert.Text2Tuple<T1, T2>(ReadFileToText(path), separator);
        public static List<(T1, T2, T3)> ReadFileToTuple<T1, T2, T3>(string path, string separator = Constants.DEFAULT_SEPARATOR) => DataConvert.Text2Tuple<T1, T2, T3>(ReadFileToText(path), separator);
        public static List<(T1, T2, T3, T4)> ReadFileToTuple<T1, T2, T3, T4>(string path, string separator = Constants.DEFAULT_SEPARATOR) => DataConvert.Text2Tuple<T1, T2, T3, T4>(ReadFileToText(path), separator);
        public static List<(T1, T2, T3, T4, T5)> ReadFileToTuple<T1, T2, T3, T4, T5>(string path, string separator = Constants.DEFAULT_SEPARATOR) => DataConvert.Text2Tuple<T1, T2, T3, T4, T5>(ReadFileToText(path), separator);
        public static List<(string path, string content)> ReadFilesToTextWithPath(IEnumerable<string> paths) => paths.Select(x => (path: x, content: ReadFileToText(x))).ToList();
        public static List<string> ReadFilesToText(IEnumerable<string> paths) => paths.Select(ReadFileToText).ToList();
        public static FileStream ReadFileToStream(string path, string? extension = null) => new(Extensify(Pathify(path), extension), FileMode.Open, FileAccess.Read);
        public static FileStream ReadExcel(string path) => ReadFileToStream(path, FileTypes.Excel);

        // Write
        public static void WriteTextToFile(string path, string content, bool createFolderIfNotExist = true)
        {
            path = $"{(path.Contains(':') ? "" : RootOutFolder)}{path}";
            path = path.Replace("/", "\\");
            //Console.WriteLine("Create: " + (createFolderIfNotExist && path.LastIndexOf("\\") >= 0) + ": " + path + ":: " + (path.Substring(0, path.LastIndexOf("\\"))));
            if (createFolderIfNotExist && path.LastIndexOf("\\") >= 0) Directory.CreateDirectory(path.Substring(0, path.LastIndexOf("\\")));
            File.WriteAllText(path, content);
        }
        public static void WriteTextToFile(string path, IEnumerable<object> content, bool createFolderIfNotExist = true)
            => WriteTextToFile(path, string.Join("\r\n", content), createFolderIfNotExist);
        public static void WriteTextToFile(string path, IEnumerable<IEnumerable<object>> content, bool createFolderIfNotExist = true, string separator = Constants.DEFAULT_SEPARATOR)
            => WriteTextToFile(path, DataConvert.Table2Text(content, separator), createFolderIfNotExist);
        public static void WriteTableToFile<T>(string path, IEnumerable<T> content, bool createFolderIfNotExist = true, string separator = Constants.DEFAULT_SEPARATOR)
            => WriteTextToFile(path, DataConvert.Type2Text(content, separator), createFolderIfNotExist);

        public static List<string> GetFiles(string sDir, string fileType = "*", bool recursively = true, List<string>? results = null)
        {
            if (results == null) results = new List<string>();
            try
            {
                foreach (var f in Directory.GetFiles(sDir))
                {
                    results.Add(f);
                }
                foreach (var d in Directory.GetDirectories(sDir))
                {
                    if (recursively) GetFiles(d, fileType, recursively, results);
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return results.Where(x => fileType.Equals("*") || GetFileExtension(x).Equals(fileType)).ToList();
        }

        public static List<string> GetSubfolders(string path, bool recursively = true, List<string>? results = null)
        {
            if (results == null) results = new List<string>();
            try
            {
                foreach (var d in Directory.GetDirectories(path))
                {
                    results.Add(d);
                    if (recursively) GetSubfolders(d, recursively, results);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return results;
        }
    }
}