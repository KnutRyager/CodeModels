using CodeModels.Factory;
using FluentAssertions;
using Xunit;

namespace CodeModels.Test.Factory.JitExpression;

public class TernaryExpressionModelsFromExpressionTests
{
    private bool a = true;

    [Fact] public void Ternary() => CodeModelsFromExpression.GetCode(x => a ? 1 : 2).Should().Be("a ? 1 : 2");
}