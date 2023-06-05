using System;
using System.Reflection;
using CodeModels.Factory;
using FluentAssertions;
using Xunit;

namespace CodeModels.Test.Factory.JitExpression;

public class ConstantExpressionModelsFromExpressionTests
{
    [Fact] public void Null() => CodeModelsFromExpression.GetCode(x => null).Should().Be("null");
    [Fact] public void Default() => CodeModelsFromExpression.GetCode(x => default).Should().Be("null");
    [Fact] public void DefaultWithParameter() => CodeModelsFromExpression.GetCode(x => default(int)).Should().Be("0");

    [Fact] public void True() => CodeModelsFromExpression.GetCode(x => true).Should().Be("true");
    [Fact] public void False() => CodeModelsFromExpression.GetCode(x => false).Should().Be("false");

    [Fact] public void Int() => CodeModelsFromExpression.GetCode(x => 1).Should().Be("1");

    [Fact] public void String() => CodeModelsFromExpression.GetCode(x => "hi").Should().Be("\"hi\"");

    [Fact] public void Enum() => CodeModelsFromExpression.GetCode(x => BindingFlags.Instance).Should().Be("BindingFlags.Instance");
    
    [Fact] public void StaticConst() => CodeModelsFromExpression.GetCode(x => Math.PI).Should().Be("3.141592653589793D");
}