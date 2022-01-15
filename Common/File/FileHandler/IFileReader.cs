using System.Collections.Generic;
using System.IO;
using Common.DataStructures;
using Common.Util;

namespace Common.Files;

public interface IFileReader : IPathHandler
{
    Stream ReadFile(string path);
    string ReadFileToText(string path);
    List<string> ReadFileToLines(string path);
    List<List<string>> ReadFileToTable(string path, string separator = Constants.DEFAULT_SEPARATOR);
    List<T> ReadFileToType<T>(string path, string separator = Constants.DEFAULT_SEPARATOR, CanonicalNameConverter? canonicalNames = null) where T : new();
    /// <summary>
    /// Get a list of files fine in the directory/directories, possibly filtered by a filetype.
    /// </summary>
    List<string> GetFiles(string path, string fileType = "*", bool recursively = true, List<string>? results = null);
    FileSearchResult Search(FileSearchFilter request);
}
