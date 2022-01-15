using System.Collections.Generic;

namespace Common.Files;

public record FileSearchResult(IList<SingleFileInfo> Files, int ResultCount);
public record FileSearchFilter(string Path, int? Skip, int? Take, List<string>? Extensions);
public record SingleFileInfo(string Path, string FileName);
