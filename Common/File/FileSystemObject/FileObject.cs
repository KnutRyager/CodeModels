using System.IO;

namespace Common.Files;

public record FileObject(string Name, string Content, string Extension = "")
    : FileSystemObject(Name)
{
    public override void Save(string path)
    {
        File.WriteAllText(MakePath(path), Content);
    }

    public override FileSystemObject Read(string path) => this with
    {
        Name = Name,
        Content = File.ReadAllText(MakePath(path)),
        Extension = Extension
    };

    private string MakePath(string path) => $"{path}/{Name}{(string.IsNullOrEmpty(Extension) ? "" : ".")}{Extension}";
}
