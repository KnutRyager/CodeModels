using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using FluentAssertions;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.Execution.Scope;

public class StructCodeModelExecutionScopeTests
{
    [Fact]
    public void HasIdentifier()
    {
        var scope = new ObjectModelExecutionScope(new CodeModelExecutionContext(), new TestStruct());
        scope.HasIdentifier("field").Should().Be(true);
        scope.HasIdentifier("Property").Should().Be(true);
        scope.HasIdentifier("Method").Should().Be(true);
        scope.HasIdentifier("Non-existing").Should().Be(false);
    }

    [Fact]
    public void SetAndGetField()
    {
        var o = new TestStruct();
        var scope = new ObjectModelExecutionScope(new CodeModelExecutionContext(), o);
        o.field.Should().Be(default);
        scope.SetValue("field", Literal("a"));
        scope.GetValue("field").Should().Be(Literal("a"));
        o.field.Should().Be("a");
    }

    [Fact]
    public void SetAndGetProperty()
    {
        var o = new TestStruct();
        var scope = new ObjectModelExecutionScope(new CodeModelExecutionContext(), o);
        o.Property.Should().Be(default);
        scope.SetValue("Property", Literal("a"));
        scope.GetValue("Property").Should().Be(Literal("a"));
        o.Property.Should().Be("a");
    }

    [Fact]
    public void GetMethodValue()
    {
        var o = new TestStruct();
        var scope = new ObjectModelExecutionScope(new CodeModelExecutionContext(), o);
        scope.ExecuteMethod("Method").Should().Be(Literal("method result"));
    }

    [Fact]
    public void SetAndGetValueThroughMethod()
    {
        var o = new TestStruct();
        var scope = new ObjectModelExecutionScope(new CodeModelExecutionContext(), o);
        o.field.Should().Be(default);
        scope.ExecuteMethodPlain("SetterMethod", "new_value").Should().Be("new_value");
        o.field.Should().Be("new_value");
        o.GetterMethod().Should().Be("new_value");
        scope.ExecuteMethod("GetterMethod").Should().Be(Literal("new_value"));
    }

    public class TestStruct
    {
        public string field = default!;
        public string Property { get; set; } = default!;
        public string Method() => "method result";
        public string SetterMethod(string value) => field = value;
        public string GetterMethod() => field;
    }
}
