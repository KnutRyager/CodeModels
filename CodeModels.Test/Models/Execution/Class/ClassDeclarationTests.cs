using System.Linq;
using CodeAnalyzation.Test;
using FluentAssertions;
using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models.Execution.Class.Test;

public class ClassDeclarationTests
{
    [Fact]
    public void EvaluateClassDeclaration()
    {
        var c = CodeModelFactory.Class(
        "ClassA",
            FieldModel("A", Literal("test")));

        var context = new ProgramModelExecutionContext();

        c.Evaluate(context);
        context.GetMember("", "ClassA").Should().Be(c);
    }

    [Fact]
    public void EvaluateClassDeclarationWithNamespace()
    {
        var c = CodeModelFactory.Class(
        "SpaceA.ClassA",
            FieldModel("A", Literal("test")));

        var context = new ProgramModelExecutionContext();

        c.Evaluate(context);
        context.GetMember("SpaceA", "ClassA").Should().Be(c);
    }

    [Fact]
    public void ResolveAndExecuteConstructorFromDeclaration()
    {
        var c = CodeModelFactory.Class(
        "SpaceA.ClassA",
            FieldModel("A", Literal("test")));

        var context = new ProgramModelExecutionContext();

        c.Evaluate(context);

        var identifier = Identifier("SpaceA.ClassA");
        var invocation = identifier.Lookup(context);

        context.GetMember("SpaceA", "ClassA").Should().Be(c);
    }
}
