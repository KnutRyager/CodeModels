using CodeModels.Factory;
using FluentAssertions;
using TestCommon;
using Xunit;

namespace CodeModels.Test.Factory.JitExpression;

public class BinaryExpressionModelsFromExpressionTests
{
    private int a = 0;
    private int b = 0;
    private bool c = true;
    private bool d = false;

    [Fact] public void Addition() => CodeModelsFromExpression.GetCode(x => a + a).Should().Be("a + a");
    [Fact] public void Subtraction() => CodeModelsFromExpression.GetCode(x => a - a).Should().Be("a - a");
    [Fact] public void Multiplication() => CodeModelsFromExpression.GetCode(x => a * a).Should().Be("a * a");
    [Fact] public void Division() => CodeModelsFromExpression.GetCode(x => a / a).Should().Be("a / a");
    [Fact] public void Modulo() => CodeModelsFromExpression.GetCode(x => a % a).Should().Be("a % a");

    [Fact] public void LogicalAnd() => CodeModelsFromExpression.GetCode(x => c && d).Should().Be("c && d");
    [Fact] public void LogicalOr() => CodeModelsFromExpression.GetCode(x => c || d).Should().Be("c || d");
    [Fact] public void ExclusiveOr() => CodeModelsFromExpression.GetCode(x => c ^ d).Should().Be("c ^ d");
    
    [Fact] public void LeftShift() => CodeModelsFromExpression.GetCode(x => a << 2).Should().Be("a << 2");
    [Fact] public void RightShift() => CodeModelsFromExpression.GetCode(x => a >> 2).Should().Be("a >> 2");

    [Fact] public void Equal() => CodeModelsFromExpression.GetCode(x => a == b).Should().Be("a == b");
    [Fact] public void NotEqual() => CodeModelsFromExpression.GetCode(x => a != b).Should().Be("a != b");
    [Fact] public void GreaterThanOrEqual() => CodeModelsFromExpression.GetCode(x => a >= b).Should().Be("a >= b");
    [Fact] public void GreaterThan() => CodeModelsFromExpression.GetCode(x => a > b).Should().Be("a > b");
    [Fact] public void LessThan() => CodeModelsFromExpression.GetCode(x => a < b).Should().Be("a < b");
    [Fact] public void LessThanOrEqual() => CodeModelsFromExpression.GetCode(x => a <= b).Should().Be("a <= b");

    [Fact] public void Coalesce() => CodeModelsFromExpression.GetCode(x => "a" ?? "b").Should().Be("\"a\" ?? \"b\"");
}