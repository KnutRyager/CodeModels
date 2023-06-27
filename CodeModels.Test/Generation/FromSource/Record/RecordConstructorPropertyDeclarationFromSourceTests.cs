using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Record;

public class RecordConstructorPropertyDeclarationFromSourceTests
{
    [Fact] public void PropertySingle() => @"
record A(int B);".AssertParsedAndGeneratedEqual();

    [Fact] public void PropertyMultiple() => @"
record A(int B, string C);".AssertParsedAndGeneratedEqual();
}