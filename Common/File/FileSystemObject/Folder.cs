using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.Files;

public record Folder(string Name, List<FileSystemObject> Contents)
    : FileSystemObject(Name)
{
    public static Folder Create(string name, params FileSystemObject[] contents)
        => new(name, contents.ToList());

    public override void Save(string path)
    {
        Directory.CreateDirectory(MakePath(path));
        foreach (var content in Contents)
        {
            content.Save(MakePath(path));
        }
    }

    public override FileSystemObject Read(string path) => this with
    {
        Name = Name,
        Contents = Contents.Select(x => x.Read(MakePath(path))).ToList()
    };

    private string MakePath(string path) => string.IsNullOrEmpty(Name) ? path : $"{path}/{Name}";

}
