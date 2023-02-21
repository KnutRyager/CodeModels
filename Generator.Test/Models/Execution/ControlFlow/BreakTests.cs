using CodeAnalyzation.Models.Execution;
using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.ControlFlow.Test;

public class BreakTests
{
    [Fact] public void ForBreak() => @"
var s = 0;
for(var i = 0; i < 10; i++) {
    if(i >= 4) break;
    s += 1;
}
s;".Eval().Should().Be(4);

    [Fact] public void InnerForBreak() => @"
var s = 0;
for(var i = 0; i < 3; i++) {
    for(var j = 0; j < 10; j++) {
        if(j > 0) break; 
        s += 100;
    }
    s += 1;
}
s;".Eval().Should().Be(303);

    [Fact] public void WhileBreak() => @"
var s = 0;
var i = 0;
while(i < 10) {
    if(i >= 4) break;
    s += 1;
    i++;
}
s;".Eval().Should().Be(4);

    [Fact] public void DoWhileBreak() => @"
var s = 0;
var i = 0;
do {
    if(i >= 4) break;
    s += 1;
    i++;
} while(i < 10);
s;".Eval().Should().Be(4);
    
}
