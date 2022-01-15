using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.DataStructures;
using Common.Util;

namespace Common.Files;

public abstract class AbstractFileHandler : IFileHandler
{
    public string RootPath { get; set; }
    private string? _rootInPath;
    public string RootInPath { get => FileUtil.Pathify(RootPath, _rootInPath); set => _rootInPath = value; }
    private string? _rootOutPath;
    public string RootOutPath { get => FileUtil.Pathify(RootPath, _rootOutPath); set => _rootOutPath = value; }

    public AbstractFileHandler(string rootPath, string? rootInPath = null, string? rootOutPath = null)
    {
        RootPath = rootPath;
        _rootInPath = rootInPath;
        _rootOutPath = rootOutPath;
    }

    public abstract Task Copy(Stream src, string destinationPath);
    public abstract List<string> GetFiles(string path, string fileType = "*", bool recursively = true, List<string>? results = null);
    public abstract Stream ReadFile(string path);
    public abstract string ReadFileToText(string path);
    public abstract Task Write(string path, Stream content);
    protected string Pathify(string path) => FileUtil.Pathify(RootPath, path);
    protected string PathifyIn(string path) => FileUtil.Pathify(RootInPath, path);
    protected string PathifyOut(string path) => FileUtil.Pathify(RootOutPath, path);

    public FileSearchResult Search(FileSearchFilter request)
    {
        var allFiles = GetFiles(request.Path)
            .Where(x => request.Extensions == null || request.Extensions.Contains(FileUtil.GetFileExtension(x)));
        var files = allFiles
        .Select(x => new SingleFileInfo(FileName: FileUtil.FileName(x), Path: request.Path))
        .OrderByDescending(x => DateUtil.FindDate(x.FileName))
        .Skip(request.Skip ?? 0).Take(request.Take ?? 1000).ToList();
        return new FileSearchResult(ResultCount: allFiles.Count(), Files: files);
    }

    public List<string> ReadFileToLines(string path)
    {
        throw new System.NotImplementedException();
    }

    public List<List<string>> ReadFileToTable(string path, string separator = ";")
    {
        throw new System.NotImplementedException();
    }

    public List<T> ReadFileToType<T>(string path, string separator = ";", CanonicalNameConverter? canonicalNames = null) where T : new()
    {
        throw new System.NotImplementedException();
    }

    public Task Write<T>(string path, T content)
    {
        throw new System.NotImplementedException();
    }

    public Task Write(string path, string text)
    {
        throw new System.NotImplementedException();
    }

    public Task Write(string path, IEnumerable<string> content)
    {
        throw new System.NotImplementedException();
    }

    public Task Write(string path, IEnumerable<IEnumerable<object>> content, string separator = ";")
    {
        throw new System.NotImplementedException();
    }

    public Task Write<T>(string path, IEnumerable<T> content)
    {
        throw new System.NotImplementedException();
    }

    public Task Write(string path, byte[] bytes)
    {
        throw new System.NotImplementedException();
    }

    public Task Copy(string srcPath, string destinationPath)
    {
        throw new System.NotImplementedException();
    }

    public Task Copy(IFileReader src, string srcPath, string destinationPath)
    {
        throw new System.NotImplementedException();
    }

    string IPathHandler.Pathify(string path)
    {
        throw new System.NotImplementedException();
    }

    string IPathHandler.PathifyIn(string path)
    {
        throw new System.NotImplementedException();
    }

    string IPathHandler.PathifyOut(string path)
    {
        throw new System.NotImplementedException();
    }
}
