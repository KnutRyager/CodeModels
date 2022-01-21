using CodeAnalyzation.Test;
using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;
using FluentAssertions;

namespace CodeAnalyzation.Models.Primitives.Test;

public class ProgramModelExecutionContextTests
{
    [Fact]
    public void SetAndGetVariable()
    {
        var context = new ProgramModelExecutionContext();
        context.EnterScope();
        context.SetValue("test", Literal("a"), true);
        context.GetValue("test").Should().Be(Literal("a"));
    }

    [Fact]
    public void SetUndefinedVariable() => Assert.Throws<ProgramModelExecutionException>(() => new ProgramModelExecutionContext().SetValue("test", Literal("a")));
    [Fact]
    public void SetVariableWhenNoScope() => Assert.Throws<ProgramModelExecutionException>(() => new ProgramModelExecutionContext().SetValue("test", Literal("a"), true));
    [Fact]
    public void PopWhenNoScopes() => Assert.Throws<ProgramModelExecutionException>(() => new ProgramModelExecutionContext().ExitScope());
    [Fact]
    public void StackOverflow() => Assert.Throws<ProgramModelExecutionException>(() =>
    {
        var context = new ProgramModelExecutionContext();
        for (var i = 0; i <= 10001; i++) context.EnterScope();
    });


}
