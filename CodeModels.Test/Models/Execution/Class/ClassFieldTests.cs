using CodeModels.Factory;
using FluentAssertions;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models.Execution.Class.Test;

public class ClassFieldTests
{
    [Fact]
    public void GetStaticFieldValue() => CodeModelFactory.Class(
        "ClassA",
            FieldModel("A", Literal("test"), modifier: PropertyAndFieldTypes.PublicStaticField))
            .GetField("A").AccessValue().LiteralValue().Should().Be("test");

    [Fact]
    public void GetInstanceFieldValue()
    {
        var c = CodeModelFactory.Class(
        "ClassA",
            FieldModel("A", Literal("test"), modifier: PropertyAndFieldTypes.PublicField));
        var instance = c.CreateInstance();
        var fieldModel = c.GetField("A");
        var fieldAccess = fieldModel.Access(instance);
        fieldAccess.Eval().Should().Be("test");
        c.GetField("A").AccessValue(instance).LiteralValue().Should().Be("test");
    }

    [Fact]
    public void SetInstanceFieldValue()
    {
        var c = CodeModelFactory.Class(
        "ClassA",
            FieldModel("A", Literal("test"), modifier: PropertyAndFieldTypes.PublicField));
        var instance = c.CreateInstance();
        var fieldModel = c.GetField("A");
        var fieldAccess = fieldModel.Access(instance);
        var context = new ProgramModelExecutionContext();
        fieldAccess.Assign(Literal("test2")).Evaluate(context);
        fieldAccess.Eval(context).Should().Be("test2");
        c.GetField("A").AccessValue(instance).LiteralValue().Should().Be("test2");
    }

    [Fact]
    public void GetInstanceStaticFieldValue()
    {
        var c = CodeModelFactory.Class(
        "ClassA",
            FieldModel("A", Literal("test"), modifier: PropertyAndFieldTypes.PublicStaticField));
        var instance = c.CreateInstance();
        var fieldModel = c.GetField("A");
        var fieldAccess = fieldModel.Access(instance);
        fieldAccess.Eval().Should().Be("test");
        c.GetField("A").AccessValue(instance).LiteralValue().Should().Be("test");
    }

    [Fact]
    public void SetInstanceStaticFieldValue()
    {
        var c = CodeModelFactory.Class(
        "ClassA",
            FieldModel("A", Literal("test"), modifier: PropertyAndFieldTypes.PublicStaticField));
        var instance = c.CreateInstance();
        var fieldModel = c.GetField("A");
        var fieldAccess = fieldModel.Access(instance);
        var context = new ProgramModelExecutionContext();
        fieldAccess.Assign(Literal("test2")).Evaluate(context);
        fieldAccess.Eval(context).Should().Be("test2");
        c.GetField("A").AccessValue(instance).LiteralValue().Should().Be("test2");
    }

    [Fact]
    public void InitStaticIntFieldDefaultValue() => CodeModelFactory.Class(
        "ClassA",
            FieldModel(Type<int>(), "A", modifier: PropertyAndFieldTypes.PublicStaticField))
            .GetField("A").AccessValue().LiteralValue().Should().Be(0);

    [Fact]
    public void InitStaticBoolFieldDefaultValue() => CodeModelFactory.Class(
        "ClassA",
            FieldModel(Type<bool>(), "A", modifier: PropertyAndFieldTypes.PublicStaticField))
            .GetField("A").AccessValue().LiteralValue().Should().Be(false);

    [Fact]
    public void InitStaticStringFieldDefaultValue() => CodeModelFactory.Class(
        "ClassA",
            FieldModel(Type<string>(), "A", modifier: PropertyAndFieldTypes.PublicStaticField))
            .GetField("A").AccessValue().LiteralValue().Should().Be(null);

    [Fact]
    public void InitInstanceIntFieldDefaultValue()
    {
        var c = CodeModelFactory.Class(
        "ClassA",
            FieldModel(Type<int>(), "A", modifier: PropertyAndFieldTypes.PublicField));
        var instance = c.CreateInstance();
        var fieldModel = c.GetField("A");
        var fieldAccess = fieldModel.Access(instance);
        fieldAccess.Eval().Should().Be(0);
        c.GetField("A").AccessValue(instance).LiteralValue().Should().Be(0);
    }

    [Fact]
    public void InitInstanceBoolFieldDefaultValue()
    {
        var c = CodeModelFactory.Class(
        "ClassA",
            FieldModel(Type<bool>(), "A", modifier: PropertyAndFieldTypes.PublicField));
        var instance = c.CreateInstance();
        var fieldModel = c.GetField("A");
        var fieldAccess = fieldModel.Access(instance);
        fieldAccess.Eval().Should().Be(false);
        c.GetField("A").AccessValue(instance).LiteralValue().Should().Be(false);
    }

    [Fact]
    public void InitInstanceStringFieldDefaultValue()
    {
        var c = CodeModelFactory.Class(
        "ClassA",
            FieldModel(Type<string>(), "A", modifier: PropertyAndFieldTypes.PublicField));
        var instance = c.CreateInstance();
        var fieldModel = c.GetField("A");
        var fieldAccess = fieldModel.Access(instance);
        fieldAccess.Eval().Should().Be(null);
        c.GetField("A").AccessValue(instance).LiteralValue().Should().Be(null);
    }
}
