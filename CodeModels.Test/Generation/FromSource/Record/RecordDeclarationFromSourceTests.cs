using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Record;

public class RecordDeclarationFromSourceTests
{
    [Fact] public void Record() => @"
record A;".AssertParsedAndGeneratedEqual();

    [Fact] public void PublicRecord() => @"
public record A;".AssertParsedAndGeneratedEqual();

    [Fact] public void StaticRecord() => @"
static record A;".AssertParsedAndGeneratedEqual();

    [Fact] public void PrivateRecord() => @"
private record A;".AssertParsedAndGeneratedEqual();

    [Fact] public void ProtectedRecord() => @"
protected record A;".AssertParsedAndGeneratedEqual();

    [Fact] public void InternalRecord() => @"
internal record A;".AssertParsedAndGeneratedEqual();

    [Fact] public void FileRecord() => @"
file record A;".AssertParsedAndGeneratedEqual();

    [Fact] public void PublicStaticRecord() => @"
public static record A;".AssertParsedAndGeneratedEqual();

    [Fact] public void RecordSimplifiesBody() => @"
record A
{
}".AssertParsedAndGeneratedEqual(@"
record A;");

    [Fact]
    public void RecordSimplifiesArgumentList() => @"
record A();".AssertParsedAndGeneratedEqual(@"
record A;");
}