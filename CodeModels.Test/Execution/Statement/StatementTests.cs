using CodeModels.Execution;
using FluentAssertions;
using Xunit;

namespace CodeModels.Test.Execution.Statement;

public class StatementTests
{
    [Fact] public void AddVariables() => @"
var a = 2;
var b = 3;
a + b;".Eval().Should().Be(5);

    [Fact] public void TopLevelStatements() => @"
int sum = 0;
for (var i = 1; i <= 100; i++) {
    sum += System.Math.Pow(i,2);
}
return sum;
".Eval().Should().Be(338350);

    [Fact] public void SquaredSum() => @"
int sum = 0;
for (int i = 1; i <= 100; i++)
{
    sum += System.Math.Pow(i, 2);
}
 sum;
".Eval().Should().Be(338350);

    [Fact] public void CommentLineIgnored() => "// Start code\r\n5;".Eval().Should().Be(5);

    [Fact] public void NowViaVariable() => @"
System.DateTime now = System.DateTime.Now;
return now.Day;
".Eval().Should().Be(System.DateTime.Now.Day);
}
