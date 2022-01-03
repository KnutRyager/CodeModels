using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;
using static CodeAnalyzation.Test.TestUtil;

namespace CodeAnalyzation.Models.Primitives.Test;

public class TypeTests
{
    [Fact]
    public void ParseInt() => Type("int").TypeSyntax().CodeEqual("int");
    [Fact]
    public void ParseOptionalInt() => Type("int?").TypeSyntax().CodeEqual("int?");
    [Fact]
    public void ParseObject() => Type("object").TypeSyntax().CodeEqual("object");
    [Fact]
    public void FroTypeInt() => Type(typeof(int)).TypeSyntax().CodeEqual("int");
    [Fact]
    public void FromTypeObject() => Type(typeof(object)).TypeSyntax().CodeEqual("object");
}
