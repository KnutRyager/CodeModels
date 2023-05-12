using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Expression;

public class BinaryOperatorFromSourceTests
{
    [Fact] public void Addition() => "1 + 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void Subtraction() => "1 - 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void Multiplication() => "1 * 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void Division() => "1 / 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void Modulo() => "1 % 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void Equal() => "1 == 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void NotEqual() => "1 != 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void GreaterThan() => "1 > 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void GreaterThanOrEqual() => "1 >= 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void LessThan() => "1 < 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void LessThanOrEqual() => "1 <= 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void LogicalAnd() => "1 && 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void LogicalOr() => "1 || 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void BitwiseAnd() => "1 & 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void BitwiseOr() => "1 | 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void ExclusiveOr() => "1 ^ 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void LeftShift() => "1 << 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void RightShift() => "1 >> 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void Dot() => "int.Parse;".AssertParsedAndGeneratedEqual();
    [Fact] public void Assignment() => "int a = 4;".AssertParsedAndGeneratedEqual();
    [Fact(Skip = "Not implemented")] public void Is() => "1 is 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void As() => "1 as int;".AssertParsedAndGeneratedEqual();
    [Fact] public void Coalesce() => "1 ?? 1;".AssertParsedAndGeneratedEqual();
}