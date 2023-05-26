using System.Text;
using CodeModels.Factory;
using FluentAssertions;
using Xunit;

namespace CodeModels.Test.Factory.JitExpression;

public class ObjectCreationExpressionModelsFromExpressionTests
{
    [Fact] public void Object() => CodeModelsFromExpression.GetCode(x => new StringBuilder()).Should().Be("new StringBuilder()");
    [Fact] public void ObjectWithParameters() => CodeModelsFromExpression.GetCode(x => new string("hi")).Should().Be("new string(\"hi\")");
    [Fact] public void Array() => CodeModelsFromExpression.GetCode(x => new int[] { 1, 2, 3 }).Should().Be(@"new int[]
{
    1,
    2,
    3
}");
}