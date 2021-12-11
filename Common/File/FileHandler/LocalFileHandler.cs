using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common.Util;

namespace Common.Files
{
    public class LocalFileHandler : AbstractFileHandler, IPathHandler
    {

        public LocalFileHandler(string rootPath, string? rootInPath = null, string? rootOutPath = null)
            : base(rootPath, rootInPath, rootOutPath)
        { }

        public override Task Write(string path, Stream content)
        {
            path = PathifyIn(path);
            using var fileStream = File.Create(path);
            content.Seek(0, SeekOrigin.Begin);
            content.CopyTo(fileStream);
            return Task.FromResult(0);
        }

        public override Task Copy(Stream src, string destinationPath)
        {
            using var file = File.Create(destinationPath);
            return src.CopyToAsync(file);
        }

        public Task Copy(string srcPath, string destinationPath)
        {
            File.Copy(Pathify(srcPath), Pathify(destinationPath));
            return Task.FromResult(0);
        }

        public Task WriteAll(string path, FileStream content)
        {
            using var file = File.Create(PathifyIn(path));
            return content.CopyToAsync(file);
        }

        public override Stream ReadFile(string path) => DataConvert.Path2FileStream(PathifyOut(path));
        public override string ReadFileToText(string path) => FileUtil.ReadFileToText(PathifyOut(path));
        public override List<string> GetFiles(string path, string fileType = "*", bool recursively = true, List<string>? results = null) => FileUtil.GetFiles(PathifyOut(path), fileType, recursively, results);
    }
}
