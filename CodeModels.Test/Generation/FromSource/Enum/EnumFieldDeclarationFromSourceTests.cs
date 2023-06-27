using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Class;

public class EnumFieldDeclarationFromSourceTests
{
    [Fact] public void FieldSingle() => @"
enum A
{
    A
}".AssertParsedAndGeneratedEqual();

    [Fact] public void FieldMultipleImplicitValue() => @"
enum A
{
    A,
    B,
    C
}".AssertParsedAndGeneratedEqual();
    [Fact] public void FieldExplicitValue() => @"
enum A
{
    A = 0
}".AssertParsedAndGeneratedEqual();
    [Fact] public void FieldMultipleExplicitValue() => @"
enum A
{
    A = 0,
    B = 1,
    C = 2
}".AssertParsedAndGeneratedEqual();
    [Fact] public void FieldMultipleMixedImplicitValue() => @"
enum A
{
    A = 0,
    B,
    C = 2
}".AssertParsedAndGeneratedEqual();
}