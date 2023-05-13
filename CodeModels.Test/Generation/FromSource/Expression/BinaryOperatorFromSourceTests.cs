using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Expression;

public class BinaryOperatorFromSourceTests
{
    [Fact] public void Addition() => "1 + 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void Subtraction() => "1 - 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void Multiplication() => "1 * 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void Division() => "1 / 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void Modulo() => "1 % 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void Equal() => "1 == 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void NotEqual() => "1 != 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void GreaterThan() => "1 > 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void GreaterThanOrEqual() => "1 >= 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void LessThan() => "1 < 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void LessThanOrEqual() => "1 <= 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void LogicalAnd() => "1 && 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void LogicalOr() => "1 || 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void BitwiseAnd() => "1 & 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void BitwiseOr() => "1 | 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void ExclusiveOr() => "1 ^ 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void LeftShift() => "1 << 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void RightShift() => "1 >> 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void Dot() => "int.Parse;".AssertParsedAndGeneratedEqual();
    [Fact] public void Assignment() => "int a = 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void As() => "1 as int;".AssertParsedAndGeneratedEqual();
    [Fact] public void Coalesce() => "1 ?? 2;".AssertParsedAndGeneratedEqual();
}