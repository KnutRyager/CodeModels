using CodeModels.Execution.Context;
using CodeModels.Factory;
using FluentAssertions;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.Execution.Classes;

public class ClassDeclarationTests
{
    [Fact]
    public void EvaluateClassDeclaration()
    {
        var c = Class(
        "ClassA",
            Field("A", Literal("test")));

        var context = new CodeModelExecutionContext();

        c.Evaluate(context);
        context.GetMember("", "ClassA").Should().Be(c);
    }

    [Fact]
    public void EvaluateClassDeclarationWithNamespace()
    {
        var c = Class(
        "SpaceA.ClassA",
            Field("A", Literal("test")));

        var context = new CodeModelExecutionContext();

        c.Evaluate(context);
        context.GetMember("SpaceA", "ClassA").Should().Be(c);
    }

    [Fact]
    public void ResolveAndExecuteConstructorFromDeclaration()
    {
        var c = Class(
        "SpaceA.ClassA",
            Field("A", Literal("test")));

        var context = new CodeModelExecutionContext();

        c.Evaluate(context);

        var identifier = Identifier("SpaceA.ClassA");
        var invocation = identifier.Lookup(context);

        context.GetMember("SpaceA", "ClassA").Should().Be(c);
    }
}
