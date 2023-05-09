using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using static CodeModels.Models.CodeModelFactory;

namespace CodeModels.Models.Execution.Class.Test;

public class ClassConstructorTests
{
    [Fact]
    public void ConstructorInitiedInline()
    {
        var c = CodeModelFactory.Class("classA",
            Constructor(),
            FieldModel("A", Literal(5)),
            Method("get3", Type<int>(), Block(Return(3))));
        var constructorFromClass = c.GetConstructor();
        var invocation = constructorFromClass.Invoke();
        var instance = invocation.Evaluate(new ProgramModelExecutionContext());
        instance.Should().BeEquivalentTo(new InstantiatedObject(c,
            c.CreateInstanceScope(true),
            c.GetStaticScope()));
    }

    [Fact]
    public void ConstructorAdded()
    {
        var c = CodeModelFactory.Class("classA", FieldModel("A", Literal(5)));
        Method("get3", Type<int>(), Block(Return(3)));
        var constructor = Constructor();
        c.AddMember(constructor);
        var constructorFromClass = c.GetConstructor();
        var invocation = constructorFromClass.Invoke();
        var instance = invocation.Evaluate(new ProgramModelExecutionContext());
        instance.Should().BeEquivalentTo(new InstantiatedObject(c,
            c.CreateInstanceScope(true),
            c.GetStaticScope()));
    }

    [Fact]
    public void DefaultConstructor()
    {
        var c = CodeModelFactory.Class("classA", FieldModel("A", Literal(5)));
        Method("get3", Type<int>(), Block(Return(3)));
        var constructorFromClass = c.GetConstructor();
        var invocation = constructorFromClass.Invoke();
        var instance = invocation.Evaluate(new ProgramModelExecutionContext());
        instance.Should().BeEquivalentTo(new InstantiatedObject(c,
            c.CreateInstanceScope(true),
            c.GetStaticScope()));
    }

    [Fact]
    public void ConstructorWithParameterInitField()
    {
        var c = CodeModelFactory.Class("classA",
            Constructor(Property("a"), Block(Assignment(Identifier("A"), Identifier("a")))),
            FieldModel("A", Literal(5)));
        var constructorFromClass = c.GetConstructor();
        var invocation = constructorFromClass.Invoke(Literal(3));
        var instance = (invocation.Evaluate(new ProgramModelExecutionContext()) as InstantiatedObject)!;
        instance.GetValue("A").Should().BeEquivalentTo(Literal(3));
    }

    [Fact]
    public void ConstructorWithParameterDefaultValueInitField()
    {
        var c = CodeModelFactory.Class("classA",
            Constructor(Property("a", Literal(7)), Block(Assignment(Identifier("A"), Identifier("a")))),
            FieldModel("A", Literal(5)));
        var constructorFromClass = c.GetConstructor();
        var invocation = constructorFromClass.Invoke();
        var instance = (invocation.Evaluate(new ProgramModelExecutionContext()) as InstantiatedObject)!;
        instance.GetValue("A").Should().BeEquivalentTo(Literal(7));
    }
}
