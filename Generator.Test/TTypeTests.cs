using CodeAnalyzation.Models;
using Xunit;
using static CodeAnalysisTests.TestUtil;

namespace Generator.Test;

public class TTypeTests
{
    [Fact]
    public void ParseInt() => TType.Parse("int").TypeSyntax().CodeEqual("int");
    [Fact]
    public void ParseOptionalInt() => TType.Parse("int?").TypeSyntax().CodeEqual("int?");
    [Fact]
    public void ParseObject() => TType.Parse("object").TypeSyntax().CodeEqual("object");
    [Fact]
    public void FroTypeInt() => new TType(typeof(int)).TypeSyntax().CodeEqual("int");
    [Fact]
    public void FromTypeObject() => new TType(typeof(object)).TypeSyntax().CodeEqual("object");
}
