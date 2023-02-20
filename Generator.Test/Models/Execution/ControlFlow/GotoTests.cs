using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.ControlFlow;

public class GotoTests
{
    [Fact] public void SimpleGoto() => @"
    var s = 0;
    goto Label;
    s += 1;
Label:
    s;
".Eval().Should().Be(0);

    [Fact] public void LoopGoto() => @"
    var s = 0;
Label:
    s += 1
    if(s < 10)
        goto Label;
    s;
".Eval().Should().Be(10);

    [Fact] public void BreakGoto() => @"
var s = 0;
    for (var i = 0; i < 10; i++)
    {
        s += 100;
        for (var j = 0; j < 10; j++)
        {
            s += 1;
            if(i == 2 && j == 3)
            goto Label;

        }
    }
Label:
    s;
".Eval().Should().Be(2024);
}
