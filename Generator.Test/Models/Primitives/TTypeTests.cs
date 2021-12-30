using CodeAnalyzation.Models;
using Xunit;
using static Generator.Test.TestUtil;

namespace Generator.Test.Models.Primitives;

public class TTypeTests
{
    [Fact]
    public void ParseInt() => AbstractType.Parse("int").TypeSyntax().CodeEqual("int");
    [Fact]
    public void ParseOptionalInt() => AbstractType.Parse("int?").TypeSyntax().CodeEqual("int?");
    [Fact]
    public void ParseObject() => AbstractType.Parse("object").TypeSyntax().CodeEqual("object");
    [Fact]
    public void FroTypeInt() => new TypeFromReflection(typeof(int)).TypeSyntax().CodeEqual("int");
    [Fact]
    public void FromTypeObject() => new TypeFromReflection(typeof(object)).TypeSyntax().CodeEqual("object");
}
