using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Class;

public class ClassFieldDeclarationFromSourceTests
{
    [Fact] public void Field() => @"
class A
{
    int B;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PrivateField() => @"
class A
{
    private int B;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void FieldWithValue() => @"
class A
{
    int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PrivateFieldWithValue() => @"
class A
{
    private int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void StaticField() => @"
class A
{
    static int B;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void StaticFieldWithValue() => @"
class A
{
    static int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PrivateStaticFieldWithValue() => @"
class A
{
    private static int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ConstField() => @"
class A
{
    const int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PrivateConstField() => @"
class A
{
    private const int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PublicConstField() => @"
class A
{
    public const int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact(Skip = "Not implemented")] public void MultiField() => @"
class A
{
    int B,C;
}".AssertParsedAndGeneratedEqual();

    [Fact(Skip = "Not implemented")] public void MultiFieldWithValue() => @"
class A
{
    int B,C = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact(Skip = "Not implemented")] public void StaticMultiField() => @"
class A
{
    static int B,C;
}".AssertParsedAndGeneratedEqual();

    [Fact(Skip = "Not implemented")] public void StaticMultiFieldWithValue() => @"
class A
{
    static int B,C = 1337;
}".AssertParsedAndGeneratedEqual();
}