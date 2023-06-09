using CodeModels.Execution;
using FluentAssertions;
using Xunit;

namespace CodeModels.Test.Execution.ControlFlow;

public class SwitchStatementTests
{
    [Fact] public void SwitchStatementMatchCase() => @"
var s = 0;
switch (""b"")
{
    case ""a"":
        s += 1;
        break;
    case ""b"":
        s += 2;
        break;
    case ""c"":
        s += 4;
        break;
}
s;
".Eval().Should().Be(2);
    [Fact] public void SwitchStatementDefaultCase() => @"
var s = 0;
switch (""x"")
{
    case ""a"":
        s += 1;
        break;
    case ""b"":
        s += 2;
        break;
    default:
        s += 4;
        break;
}
s;
".Eval().Should().Be(4);

    [Fact] public void SwitchStatementIllegalMissingBreakFallsThrough() => @"
var s = 0;
switch (""b"")
{
    case ""a"":
        s += 1;
        break;
    case ""b"":
        s += 2;
    case ""c"":
        s += 4;
        break;
}
s;
".Eval().Should().Be(2);

    [Fact] public void SwitchStatementMissingDoubleLabelCatch() => @"
var s = 0;
switch (""b"")
{
    case ""a"":
        s += 1;
        break;
    case ""b"":
    case ""c"":
        s += 2;
        break;
    case ""d"":
        s += 5;
        break;
}
switch (""c"")
{
    case ""a"":
        s += 1;
        break;
    case ""b"":
    case ""c"":
        s += 2;
        break;
    case ""d"":
        s += 5;
        break;
}
s;
".Eval().Should().Be(4);
}
