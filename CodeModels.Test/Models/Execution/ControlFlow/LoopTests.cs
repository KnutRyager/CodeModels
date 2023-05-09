using CodeModels.Models.Execution;
using FluentAssertions;
using Xunit;

namespace CodeModels.Models.Execution.ControlFlow.Test;

public class LoopTests
{
    [Fact] public void ForLoop() => @"
var a = 0;
for(var i = 0; i < 10; i++){
    a += i;
}
a;".Eval().Should().Be(45);

    [Fact] public void WhileLoop() => @"
var a = 0;
var i = 0;
while(i < 10){
    a += i; i++;
}
a;".Eval().Should().Be(45);

    [Fact] public void DoWhileLoop() => @"
var a = 0; var i = 0;
do {
    a += i; i++;
} while(i < 10);
a;".Eval().Should().Be(45);

    [Fact] public void ForEachLoop() => @"
var a = 0;
foreach(var i in new int[]{ 1, 2, 3, 4, 5 }) {
    a += i;
}
a;".Eval().Should().Be(15);

    [Fact] public void ForLoopIncrementorNotRecorded() => @"
""correct"";
for(var i = 0; i < 1; i++){ }".Eval().Should().Be("correct");

    [Fact] public void ForEachLoopIncrementorNotRecorded() => @"
""correct"";
foreach(var i in new int[]{ 1 }) { }".Eval().Should().Be("correct");
}
