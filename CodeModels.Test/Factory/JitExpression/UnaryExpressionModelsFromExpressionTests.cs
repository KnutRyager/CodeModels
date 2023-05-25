using CodeModels.Factory;
using FluentAssertions;
using Xunit;

namespace CodeModels.Test.Factory.JitExpression;

public class UnaryExpressionModelsFromExpressionTests
{
    private int a = 0;
    private bool b = true;

    [Fact] public void UnaryPlus() => CodeModelsFromExpression.GetCode(x => +a).Should().Be("a");
    [Fact] public void Negate() => CodeModelsFromExpression.GetCode(x => -a).Should().Be("-a");
    [Fact] public void Not() => CodeModelsFromExpression.GetCode(x => !b).Should().Be("!b");
    [Fact] public void Cast() => CodeModelsFromExpression.GetCode(x => (int)a).Should().Be("(int)a");

    // Not supported by Expression API
    //[Fact] public void DynamicCast() => CodeModelsFromExpression.GetCode(x => (dynamic)a).Should().Be("(dyamic)a");
    //[Fact] public void Complement() => CodeModelsFromExpression.GetCode(x => ~a).Should().Be("~a");
}