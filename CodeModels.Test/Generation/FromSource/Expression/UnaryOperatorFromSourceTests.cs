using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Expression;

public class UnaryOperatorFromSourceTests
{
    [Fact] public void Addition() => "+1;".AssertParsedAndGeneratedEqual();
    [Fact] public void Subtraction() => "-1;".AssertParsedAndGeneratedEqual();
    [Fact] public void PostAddition() => "1++;".AssertParsedAndGeneratedEqual();
    [Fact] public void PreAddition() => "++1;".AssertParsedAndGeneratedEqual();
    [Fact] public void PostSubtraction() => "1--;".AssertParsedAndGeneratedEqual();
    [Fact] public void PreSubtraction() => "--1;".AssertParsedAndGeneratedEqual();
    [Fact] public void Not() => "!true;".AssertParsedAndGeneratedEqual();
    [Fact] public void Complement() => "~1;".AssertParsedAndGeneratedEqual();
    [Fact] public void SuppressNullableWarning() => "null !;".AssertParsedAndGeneratedEqual();
    [Fact] public void Parenthesis() => "(1);".AssertParsedAndGeneratedEqual();
}