using CodeModels.Execution;
using CodeModels.Execution.Context;
using CodeModels.Models;
using FluentAssertions;
using Xunit;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.Execution.Classes;

public class ClassMethodTests
{
    [Fact]
    public void EvaluateMethodBlock() => Method("get3",
            NamedValues(), Type<int>(), Block(Return(3)))
        .Invoke(Class("classA", Field("A", Literal(5))).CreateInstance())
        .Eval().Should().Be(3);

    [Fact]
    public void EvaluateMethodExpressionBody() => Method("get3",
            NamedValues(), Type<int>(), Literal(3))
        .Invoke(Class("classA", Field("A", Literal(5))).CreateInstance())
        .Eval().Should().Be(3);

    [Fact]
    public void EvaluateMethodReturnArgument() => Method("get3",
            NamedValues(NamedValue("value")), Type<int>(), IdentifierExp("value"))
        .Invoke(Class("classA", Field("A", Literal(5))).CreateInstance(), Literal(1337))
        .Eval().Should().Be(1337);

    [Fact]
    public void EvaluateMethodReturnTwoArguments() => Method("get3",
            NamedValues(NamedValue("v1"), NamedValue("v2")), Type<int>(), BinaryExpression(IdentifierExp("v1"), OperationType.Plus, IdentifierExp("v2")))
        .Invoke(Class("classA", Field("A", Literal(5))).CreateInstance(), Literal(3), Literal(7))
        .Eval().Should().Be(10);

    [Fact]
    public void ClassInstaceMethodReturnFieldValue()
    {
        var method = Method("getA",
            Type<int>(), Block(Return(IdentifierExp("A"))));
        var c = Class("classA", Field("A", Literal(5)), method);
        var instance = c.CreateInstance();

        method.Invoke(instance).Eval().Should().Be(5);
    }

    [Fact]
    public void ClassInstaceMethodModifyFieldValue()
    {
        var getter = Method("getA",
             Type<int>(), Block(Return(IdentifierExp("A"))));
        var setter = Method("setA", Type(typeof(void)), Block(Assignment(IdentifierExp("A"), Literal(6))));
        var c = Class("classA", Field("A", Literal(5)), getter, setter);
        var instance = c.CreateInstance();

        var context = new CodeModelExecutionContext();

        getter.Invoke(instance).Eval().Should().Be(5);
        setter.Invoke(instance).Evaluate(context);
        getter.Invoke(instance).Eval().Should().Be(6);
    }
}
