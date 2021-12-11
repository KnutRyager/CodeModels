using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common.Util;

namespace Common.Files
{
    public interface IFileWriter : IPathHandler
    {
        Task Write(string path, Stream content);
        Task Write<T>(string path, T content);
        Task Write(string path, string text);
        Task Write(string path, IEnumerable<string> content);
        Task Write(string path, IEnumerable<IEnumerable<object>> content, string separator = Constants.DEFAULT_SEPARATOR);
        Task Write<T>(string path, IEnumerable<T> content);
        Task Write(string path, byte[] bytes);
        Task Copy(string srcPath, string destinationPath);
        Task Copy(Stream src, string destinationPath);
        Task Copy(IFileReader src, string srcPath, string destinationPath);
    }
}
