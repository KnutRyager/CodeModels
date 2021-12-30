using CodeAnalyzation.Models;
using Xunit;
using static Generator.Test.TestUtil;

namespace Generator.Test.Models.Primitives;

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
