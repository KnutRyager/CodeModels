using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Record;

public class RecordFieldDeclarationFromSourceTests
{
    [Fact] public void Field() => @"
record A
{
    int B;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PrivateField() => @"
record A
{
    private int B;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void FieldWithValue() => @"
record A
{
    int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PrivateFieldWithValue() => @"
record A
{
    private int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void StaticField() => @"
record A
{
    static int B;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void StaticFieldWithValue() => @"
record A
{
    static int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PrivateStaticFieldWithValue() => @"
record A
{
    private static int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ConstField() => @"
record A
{
    const int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PrivateConstField() => @"
record A
{
    private const int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PublicConstField() => @"
record A
{
    public const int B = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact(Skip = "Not implemented")] public void MultiField() => @"
record A
{
    int B,C;
}".AssertParsedAndGeneratedEqual();

    [Fact(Skip = "Not implemented")] public void MultiFieldWithValue() => @"
record A
{
    int B,C = 1337;
}".AssertParsedAndGeneratedEqual();

    [Fact(Skip = "Not implemented")] public void StaticMultiField() => @"
record A
{
    static int B,C;
}".AssertParsedAndGeneratedEqual();

    [Fact(Skip = "Not implemented")] public void StaticMultiFieldWithValue() => @"
record A
{
    static int B,C = 1337;
}".AssertParsedAndGeneratedEqual();
}