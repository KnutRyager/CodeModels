using System.Linq;
using CodeModels.Execution;
using CodeModels.Execution.Context;
using CodeModels.Models;
using FluentAssertions;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Factory.AbstractCodeModelFactory;
using CodeModels.Models.Primitives.Member;

namespace CodeModels.Test.Execution.Classes;

public class ClassInheritanceTests
{
    [Fact]
    public void EvaluateGetterBlock() => Property(Type<int>(), "get3",
        Accessors(Accessor(AccessorType.Get, Block(Return(3)))))
        .GetGetter()!
        .Invoke(Class("classA", Field("A", Literal(5))).CreateInstance())
        .Eval().Should().Be(3);

    [Fact]
    public void EvaluateGetterExpressionBody() => Property(Type<int>(), "get3",
        Accessors(Accessor(AccessorType.Get, expressionBody: Literal(3))))
        .GetGetter()!
        .Invoke(Class("classA", Field("A", Literal(5))).CreateInstance())
        .Eval().Should().Be(3);

    [Fact]
    public void ClassInstaceMethodReturnFieldValue()
    {
        var method = Method("getA",
            NamedValues(), Type<int>(), Block(Return(IdentifierExp("A"))));
        var c = Class("classA", Field("A", Literal(5)), method);
        var instance = c.CreateInstance();

        method.Invoke(instance).Eval().Should().Be(5);
    }

    [Fact]
    public void ClassInstacePropertyModifyFieldValueAutoProperty()
    {
        var property = Property(Type<string>(), "A", Accessors(
            Accessor(AccessorType.Get),
            Accessor(AccessorType.Set)));
        var c = Class("classA",
            new IFieldOrProperty[] { property });
        var instance = c.CreateInstance();
        var getter = property.GetGetter()!;
        var setter = property.GetSetter()!;

        var context = new CodeModelExecutionContext();
        setter.Invoke(instance, new[] { Literal(6) }).Evaluate(context);
        getter.Invoke(instance).Eval().Should().Be(6);
    }

    [Fact]
    public void ClassInstacePropertyModifyBackingField()
    {
        var property = Property(Type<string>(), "A", Accessors(
            Accessor(AccessorType.Get),
            Accessor(AccessorType.Set)));
        var c = Class("classA",
            new IFieldOrProperty[] { property });
        var instance = c.CreateInstance();
        var getter = property.GetGetter()!;
        var backingField = c.GetFields().FirstOrDefault(x => x.Name == $"<A>k__BackingField")!;
        backingField.Should().NotBeNull();
        var field = backingField.AccessValue(instance) as FieldExpression;
        var context = new CodeModelExecutionContext();
        var fieldAccess = backingField.Access(instance);
        fieldAccess.EvaluatePlain(context).Should().Be(null);
        fieldAccess.Assign(Literal(5)).Evaluate(context);
        fieldAccess.EvaluatePlain(context).Should().Be(5);
        fieldAccess.Eval(context).Should().Be(5);
        getter.Invoke(instance).Eval().Should().Be(5);
    }

    [Fact]
    public void ClassInstanceStaticPropertyModifyBackingField()
    {
        var property = Property(Type<string>(), "A", Accessors(
            Accessor(AccessorType.Get),
            Accessor(AccessorType.Set)), modifier: PropertyAndFieldTypes.PublicStaticField);
        var c = Class("classA",
            new IFieldOrProperty[] { property });
        var instance = c.CreateInstance();
        var getter = property.GetGetter()!;
        var backingField = c.GetFields().FirstOrDefault(x => x.Name == $"<A>k__BackingField")!;
        backingField.Should().NotBeNull();
        var fieldModel = backingField.AccessValue(instance) as FieldExpression;
        var context = new CodeModelExecutionContext();
        var fieldAccess = backingField.Access(instance);
        fieldAccess.EvaluatePlain(context).Should().Be(null);
        fieldAccess.Assign(Literal(5)).Evaluate(context);
        fieldAccess.EvaluatePlain(context).Should().Be(5);
        fieldAccess.Eval(context).Should().Be(5);
        getter.Invoke(instance).Eval().Should().Be(5);
    }

    [Fact]
    public void StaticPropertyBackingFieldIsStatic()
    {
        var property = Property(Type<string>(), "A", Accessors(
            Accessor(AccessorType.Get),
            Accessor(AccessorType.Set)), modifier: PropertyAndFieldTypes.PublicStaticField);
        var c = Class("classA",
            new IFieldOrProperty[] { property });
        var backingField = c.GetFields().FirstOrDefault(x => x.Name == $"<A>k__BackingField")!;
        backingField.IsStatic.Should().Be(true);
    }

    [Fact]
    public void InstancePropertyBackingFieldIsNotStatic()
    {
        var property = Property(Type<string>(), "A", Accessors(
            Accessor(AccessorType.Get),
            Accessor(AccessorType.Set)), modifier: PropertyAndFieldTypes.PublicField);
        var c = Class("classA",
            new IFieldOrProperty[] { property });
        var backingField = c.GetFields().FirstOrDefault(x => x.Name == $"<A>k__BackingField")!;
        backingField.IsStatic.Should().Be(false);
    }
}
