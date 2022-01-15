namespace Common.Files;

public interface IPathHandler
{
    string RootPath { get; set; }
    string RootInPath { get; set; }
    string RootOutPath { get; set; }
    string Pathify(string path);
    string PathifyIn(string path);
    string PathifyOut(string path);
}
