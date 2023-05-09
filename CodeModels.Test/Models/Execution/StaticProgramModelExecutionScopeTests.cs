using CodeAnalyzation.Test;
using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;
using FluentAssertions;

namespace CodeAnalyzation.Models.Execution.Test;

[Collection("Sequential")]
public class StaticProgramModelExecutionScopeTests
{
    [Fact]
    public void HasIdentifier()
    {
        var scope = new StaticExecutionScope(new ProgramModelExecutionContext(), typeof(StaticTestClass));
        scope.HasIdentifier("field").Should().Be(true);
        scope.HasIdentifier("Property").Should().Be(true);
        scope.HasIdentifier("Method").Should().Be(true);
        scope.HasIdentifier("Non-existing").Should().Be(false);
    }

    [Fact]
    public void SetAndGetField()
    {
        StaticTestClass.field = default;
        var scope = new StaticExecutionScope(new ProgramModelExecutionContext(), typeof(StaticTestClass));
        scope.SetValue("field", Literal("a"));
        scope.GetValue("field").Should().Be(Literal("a"));
        StaticTestClass.field.Should().Be("a");
    }

    [Fact]
    public void SetAndGetProperty()
    {
        var scope = new StaticExecutionScope(new ProgramModelExecutionContext(), typeof(StaticTestClass));
        StaticTestClass.Property.Should().Be(default);
        scope.SetValue("Property", Literal("a"));
        scope.GetValue("Property").Should().Be(Literal("a"));
        StaticTestClass.Property.Should().Be("a");
    }

    [Fact]
    public void GetMethodValue()
    {
        var scope = new StaticExecutionScope(new ProgramModelExecutionContext(), typeof(StaticTestClass));
        scope.ExecuteMethod("Method").Should().Be(Literal("method result"));
    }

    [Fact]
    public void SetAndGetValueThroughMethod()
    {
        var scope = new StaticExecutionScope(new ProgramModelExecutionContext(), typeof(StaticTestClass));
        StaticTestClass.field.Should().Be(default);
        scope.ExecuteMethodPlain("SetterMethod", "new_value").Should().Be("new_value");
        StaticTestClass.field.Should().Be("new_value");
        StaticTestClass.GetterMethod().Should().Be("new_value");
        scope.ExecuteMethod("GetterMethod").Should().Be(Literal("new_value"));
    }

    public static class StaticTestClass
    {
        public static string? field;
        public static string? Property { get; set; }
        public static string Method() => "method result";
        public static string? SetterMethod(string value) => field = value;
        public static string? GetterMethod() => field;
    }
}
