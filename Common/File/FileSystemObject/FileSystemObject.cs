namespace Common.Files;

public abstract record FileSystemObject(string Name)
{
    public abstract void Save(string path);
    public abstract FileSystemObject Read(string path);
}
