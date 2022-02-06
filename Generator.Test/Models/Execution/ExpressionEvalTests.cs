using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.Test;

public class ExpressionEvalTests
{
    // Unary
    [Fact] public void NotTrue() => "!false".Eval().Should().Be(true);
    [Fact] public void NotFalse() => "!true".Eval().Should().Be(false);
    [Fact] public void Complement() => "~0x00FF00FF00FF00FFUL".Eval().Should().Be(0xFF00FF00FF00FF00UL);
    [Fact] public void UnaryAdd() => "+5".Eval().Should().Be(5);
    [Fact] public void UnaryAddBefore() => "++5".Eval().Should().Be(6);
    [Fact] public void UnaryAddAfter() => "5++".Eval().Should().Be(5);
    [Fact] public void UnarySubtract() => "-5".Eval().Should().Be(-5);
    [Fact] public void UnarySubtractBefore() => "--5".Eval().Should().Be(4);
    [Fact] public void UnarySubtractAfter() => "5--".Eval().Should().Be(5);

    // Binary
    [Fact] public void Plus() => "2 + 5".Eval().Should().Be(7);
    [Fact] public void Subtract() => "8 - 2".Eval().Should().Be(6);
    [Fact] public void Multiply() => "8 * 2".Eval().Should().Be(16);
    [Fact] public void Division() => "8 / 2".Eval().Should().Be(4);
    [Fact] public void Modulo() => "23 % 5".Eval().Should().Be(3);
    [Fact] public void EqualsTrue() => "4 == 4".Eval().Should().Be(true);
    [Fact] public void EqualsFalse() => "4 == 3".Eval().Should().Be(false);
    [Fact] public void NotEqualsTrue() => "4 != 3".Eval().Should().Be(true);
    [Fact] public void NotEqualsFalse() => "4 != 4".Eval().Should().Be(false);
    [Fact] public void GreaterThanTrue() => "4 > 3".Eval().Should().Be(true);
    [Fact] public void GreaterThanFalse() => "4 > 4".Eval().Should().Be(false);
    [Fact] public void LessThanTrue() => "4 < 5".Eval().Should().Be(true);
    [Fact] public void LessThanFalse() => "4 < 4".Eval().Should().Be(false);
    [Fact] public void GreaterThanOrEqualTrue() => "4 >= 4".Eval().Should().Be(true);
    [Fact] public void GreaterThanOrEqualFalse() => "4 >= 5".Eval().Should().Be(false);
    [Fact] public void LessThanOrEqualTrue() => "4 <= 4".Eval().Should().Be(true);
    [Fact] public void LessThanOrEqualFalse() => "4 <= 3".Eval().Should().Be(false);
    [Fact] public void LogicalAndTrue() => "true && true".Eval().Should().Be(true);
    [Fact] public void LogicalAndFalse() => "true && false".Eval().Should().Be(false);
    [Fact] public void LogicalOrTrue() => "false || true".Eval().Should().Be(true);
    [Fact] public void LogicalOrFalse() => "false || false".Eval().Should().Be(false);
    [Fact] public void XOrTrue() => "false ^ true".Eval().Should().Be(true);
    [Fact] public void XOrFalse() => "false ^ false".Eval().Should().Be(false);
    [Fact] public void BitwiseAnd() => "7 & 2".Eval().Should().Be(2);
    [Fact] public void BitwiseOr() => "5 | 2".Eval().Should().Be(7);
    [Fact] public void BitwiseXOrTrue() => "false ^ true".Eval().Should().Be(true);
    [Fact] public void BitwiseXOrFalse() => "false ^ false".Eval().Should().Be(false);
    [Fact] public void BitwiseLeftShift() => "1 << 4".Eval().Should().Be(16);
    [Fact] public void BitwiseRightShift() => "32 >> 4".Eval().Should().Be(2);

    // Ternary
    [Fact] public void TernaryTrue() => "true ? \"yup\" : \"nope\"".Eval().Should().Be("yup");
    [Fact] public void TernaryFalse() => "false ? \"yup\" : \"nope\"".Eval().Should().Be("nope");
}
