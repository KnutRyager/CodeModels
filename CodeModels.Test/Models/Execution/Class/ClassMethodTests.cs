using FluentAssertions;
using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models.Execution.Class.Test;

public class ClassMethodTests
{
    [Fact]
    public void EvaluateMethodBlock() => Method("get3",
            PropertyCollection(), Type<int>(), Block(Return(3)))
        .Invoke(CodeModelFactory.Class("classA", FieldModel("A", Literal(5))).CreateInstance())
        .Eval().Should().Be(3);

    [Fact]
    public void EvaluateMethodExpressionBody() => Method("get3",
            PropertyCollection(), Type<int>(), Literal(3))
        .Invoke(CodeModelFactory.Class("classA", FieldModel("A", Literal(5))).CreateInstance())
        .Eval().Should().Be(3);

    [Fact]
    public void EvaluateMethodReturnArgument() => Method("get3",
            PropertyCollection(Property("value")), Type<int>(), ExpressionFromQualifiedName("value"))
        .Invoke(CodeModelFactory.Class("classA", FieldModel("A", Literal(5))).CreateInstance(), Literal(1337))
        .Eval().Should().Be(1337);

    [Fact]
    public void EvaluateMethodReturnTwoArguments() => Method("get3",
            PropertyCollection(Property("v1"), Property("v2")), Type<int>(), BinaryExpression(ExpressionFromQualifiedName("v1"), OperationType.Plus, ExpressionFromQualifiedName("v2")))
        .Invoke(CodeModelFactory.Class("classA", FieldModel("A", Literal(5))).CreateInstance(), Literal(3), Literal(7))
        .Eval().Should().Be(10);

    [Fact]
    public void ClassInstaceMethodReturnFieldValue()
    {
        var method = Method("getA",
            Type<int>(), Block(Return(ExpressionFromQualifiedName("A"))));
        var c = CodeModelFactory.Class("classA", FieldModel("A", Literal(5)), method);
        var instance = c.CreateInstance();

        method.Invoke(instance).Eval().Should().Be(5);
    }

    [Fact]
    public void ClassInstaceMethodModifyFieldValue()
    {
        var getter = Method("getA",
             Type<int>(), Block(Return(ExpressionFromQualifiedName("A"))));
        var setter = Method("setA", Type(typeof(void)), Block(Assignment(ExpressionFromQualifiedName("A"), Literal(6))));
        var c = CodeModelFactory.Class("classA", FieldModel("A", Literal(5)), getter, setter);
        var instance = c.CreateInstance();

        var context = new ProgramModelExecutionContext();

        getter.Invoke(instance).Eval().Should().Be(5);
        setter.Invoke(instance).Evaluate(context);
        getter.Invoke(instance).Eval().Should().Be(6);
    }
}
