using CodeAnalyzation.Test;
using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;
using FluentAssertions;

namespace CodeAnalyzation.Models.Execution.Test;

public class ProgramModelExecutionScopeTests
{
    [Fact]
    public void HasIdentifier() {
        var context = new ProgramModelExecutionScope();
        context.HasIdentifier("test").Should().Be(false);
        context.SetValue("test", Literal("a"));
        context.HasIdentifier("test").Should().Be(false);
    }

    [Fact]
    public void SetAndGetVariable() {
        var context = new ProgramModelExecutionScope();
        context.SetValue("test", Literal("a"));
        context.GetValue("test").Should().Be(Literal("a"));
    }


}
