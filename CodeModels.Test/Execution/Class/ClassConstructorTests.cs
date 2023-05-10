using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Models;
using FluentAssertions;
using Xunit;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.Execution.Class;

public class ClassConstructorTests
{
    [Fact]
    public void ConstructorInitiedInline()
    {
        var c = CodeModelFactory.Class("classA",
            Constructor(),
            Field("A", Literal(5)),
            Method("get3", Type<int>(), Block(Return(3))));
        var constructorFromClass = c.GetConstructor();
        var invocation = constructorFromClass.Invoke();
        var instance = invocation.Evaluate(new CodeModelExecutionContext());
        instance.Should().BeEquivalentTo(new InstantiatedObject(c,
            c.CreateInstanceScope(true),
            c.GetStaticScope()));
    }

    [Fact]
    public void ConstructorAdded()
    {
        var c = CodeModelFactory.Class("classA", Field("A", Literal(5)));
        Method("get3", Type<int>(), Block(Return(3)));
        var constructor = Constructor();
        c.AddMember(constructor);
        var constructorFromClass = c.GetConstructor();
        var invocation = constructorFromClass.Invoke();
        var instance = invocation.Evaluate(new CodeModelExecutionContext());
        instance.Should().BeEquivalentTo(new InstantiatedObject(c,
            c.CreateInstanceScope(true),
            c.GetStaticScope()));
    }

    [Fact]
    public void DefaultConstructor()
    {
        var c = CodeModelFactory.Class("classA", Field("A", Literal(5)));
        Method("get3", Type<int>(), Block(Return(3)));
        var constructorFromClass = c.GetConstructor();
        var invocation = constructorFromClass.Invoke();
        var instance = invocation.Evaluate(new CodeModelExecutionContext());
        instance.Should().BeEquivalentTo(new InstantiatedObject(c,
            c.CreateInstanceScope(true),
            c.GetStaticScope()));
    }

    [Fact]
    public void ConstructorWithParameterInitField()
    {
        var c = CodeModelFactory.Class("classA",
            Constructor(NamedValue("a"), Block(Assignment(Identifier("A"), Identifier("a")))),
            Field("A", Literal(5)));
        var constructorFromClass = c.GetConstructor();
        var invocation = constructorFromClass.Invoke(Literal(3));
        var instance = (invocation.Evaluate(new CodeModelExecutionContext()) as InstantiatedObject)!;
        instance.GetValue("A").Should().BeEquivalentTo(Literal(3));
    }

    [Fact]
    public void ConstructorWithParameterDefaultValueInitField()
    {
        var c = CodeModelFactory.Class("classA",
            Constructor(NamedValue("a", Literal(7)), Block(Assignment(Identifier("A"), Identifier("a")))),
            Field("A", Literal(5)));
        var constructorFromClass = c.GetConstructor();
        var invocation = constructorFromClass.Invoke();
        var instance = (invocation.Evaluate(new CodeModelExecutionContext()) as InstantiatedObject)!;
        instance.GetValue("A").Should().BeEquivalentTo(Literal(7));
    }
}
