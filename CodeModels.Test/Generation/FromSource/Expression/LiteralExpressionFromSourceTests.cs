using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Expression;

public class LiteralExpressionFromSourceTests
{
    [Fact] public void Int() => "1;".AssertParsedAndGeneratedEqual();
    [Fact] public void Float() => "1F;".AssertParsedAndGeneratedEqual();
    [Fact] public void Double() => "0D;".AssertParsedAndGeneratedEqual();
    [Fact] public void Bool() => "true;".AssertParsedAndGeneratedEqual();
    [Fact] public void String() => @"""test"";".AssertParsedAndGeneratedEqual();
    [Fact] public void Null() => @"null;".AssertParsedAndGeneratedEqual();
}