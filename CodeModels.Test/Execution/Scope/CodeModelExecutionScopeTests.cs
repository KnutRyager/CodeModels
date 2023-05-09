using CodeModels.Execution.Scope;
using FluentAssertions;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.Execution.Scope;

public class CodeModelExecutionScopeTests
{
    [Fact]
    public void HasIdentifier()
    {
        var context = new CodeModelExecutionScope();
        context.HasIdentifier("test").Should().Be(false);
        context.SetValue("test", Literal("a"));
        context.HasIdentifier("test").Should().Be(false);
    }

    [Fact]
    public void SetAndGetVariable()
    {
        var context = new CodeModelExecutionScope();
        context.SetValue("test", Literal("a"));
        context.GetValue("test").Should().Be(Literal("a"));
    }


}
