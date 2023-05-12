using CodeModels.Execution;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Member;
using FluentAssertions;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.Execution.Classes;

public class EnumTests
{
    [Fact]
    public void EnumStaticAccess() => Enum("Enum", EnumField("A", 5))
            .GetField("A").AccessValue().LiteralValue().Should().Be(5);

    [Fact]
    public void EnumAutomaticValueAssignment()
    {
        var e = Enum("Enum", "A", "B", "C");
        e.GetField("A").AccessValue().LiteralValue().Should().Be(0);
        e.GetField("B").AccessValue().LiteralValue().Should().Be(1);
        e.GetField("C").AccessValue().LiteralValue().Should().Be(2);
    }

    [Fact]
    public void EnumAutomaticValueAssignmentIncrementsPreviousByOne()
    {
        var e = Enum("Enum", EnumField("A", 5), EnumField("B"), EnumField("C", 3), EnumField("D"));
        e.GetField("A").AccessValue().LiteralValue().Should().Be(5);
        e.GetField("B").AccessValue().LiteralValue().Should().Be(6);
        e.GetField("C").AccessValue().LiteralValue().Should().Be(3);
        e.GetField("D").AccessValue().LiteralValue().Should().Be(4);
    }

    [Fact(Skip ="Not implemented")]
    public void EnumToString()
    {
        var e = Enum("EnumA", EnumField("A", 5));
        var instance = e.CreateInstance();
        var toString = e.GetMethod("ToString") as Method;
        var s = toString!.Invoke(instance);
        s.Eval().Should().Be("A");
    }

    [Fact(Skip ="Not implemented")]
    public void SetInstanceFieldValueNotAllowed()
    {
        var e = Enum("Enum", EnumField("A", 1));
        var instance = e.CreateInstance();
        var fieldModel = e.GetField("A");
        var fieldAccess = fieldModel.Access(instance);
        var context = new CodeModelExecutionContext();
        fieldAccess.Assign(Literal(2)).Evaluate(context);
        fieldAccess.Eval(context).Should().Be(1);
        e.GetField("A").AccessValue(instance).LiteralValue().Should().Be(1);
    }

    [Fact(Skip ="Not implemented")]
    public void SetInstanceStaticFieldValueNotAllowed()
    {
        var e = Enum("Enum", EnumField("A", 1));
        var instance = e.CreateInstance();
        var fieldModel = e.GetField("A");
        var fieldAccess = fieldModel.Access(instance);
        var context = new CodeModelExecutionContext();
        fieldAccess.Assign(Literal(2)).Evaluate(context);
        fieldAccess.Eval(context).Should().Be(1);
        e.GetField("A").AccessValue(instance).LiteralValue().Should().Be(1);
    }
}
