using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace CodeModels.Models.Execution.Function.Test;

public class LambdaTests
{
    [Fact] public void EvaluateLamdaDefinition() => @"
int newN() => 3;".Eval().Should().Be(3);

    [Fact] public void SimpleLambdaExpression() => @"
System.Func<int, int> f = x => 3 * x;
f(5);
".Eval().Should().Be(15);

    [Fact] public void SimpleLambdaExpressionBracketed() => @"
System.Func<int, int> f = x => { return 3 * x; };
f(5);
".Eval().Should().Be(15);

    // Note: This shouldn't happen. But better than nothing when no capture.
    [Fact] public void LambdaExpressionFallbackOuterScope() => @"
var y = 3;
System.Func<int, int> f = x => y * x;
f(5);
".Eval().Should().Be(15);

    [Fact(Skip = "Hard")] public void LambdaExpressionCapturesVariables() => @"
System.Func<int, int> f;
for(var i = 0; i < 1; i++){
    var y = 3;
    f = x => y * x;
}
f(5);
".Eval().Should().Be(15);

    [Fact] public void CapturesVariablesCanBeUpdated() => @"
System.Func<int, int> f;
var y = 3;
for(var i = 0; i < 1; i++){
    f = x =>
    {
        y += x;
        return 0;
    };
}
f(5);
y;
".Eval().Should().Be(8);

    [Fact]
    public void Test()
    {
        System.Func<int, int> f = x => 0;
        var y = 3;
        for (var i = 0; i < 1; i++)
        {
            f = x =>
            {
                y += x;
                return x;
            };
        }
        f(2);
        y.Should().Be(5);
    }

    [Fact] public void SimpleLambdaExpressionAction() => @"
System.Action<int> a = x => System.Console.Write(3 * x);
a(5);
".Eval().Should().Be("15");

    [Fact] public void SimpleLambdaExpressionActionBracketed() => @"
System.Action<int> a = x => { System.Console.Write(3 * x); };
a(5);
".Eval().Should().Be("15");

    [Fact] public void ParenthesizedLambdaExpressionAction0() => @"
System.Action a = () => System.Console.Write(""Success"");
a();
".Eval().Should().Be("Success");

    [Fact] public void ParenthesizedLambdaExpressionAction0Bracketed() => @"
System.Action a = () => { System.Console.Write(""Success""); };
a();
".Eval().Should().Be("Success");

    [Fact] public void ParenthesizedLambdaExpressionAction1() => @"
System.Action<string> a = (x) => System.Console.Write(x);
a(""Success"");
".Eval().Should().Be("Success");

    [Fact] public void ParenthesizedLambdaExpressionAction2() => @"
System.Action<string, string> a = (x1,x2) => System.Console.Write(x1 + x2);
a(""a"", ""b"");
".Eval().Should().Be("ab");

    [Fact] public void ParenthesizedLambdaExpressionAction3() => @"
System.Action<string, string, string> a = (x1,x2, x3) => System.Console.Write(x1 + x2 + x3);
a(""a"", ""b"", ""c"");
".Eval().Should().Be("abc");

    [Fact] public void ParenthesizedLambdaExpressionAction4() => @"
System.Action<string, string, string, string> a = (x1,x2, x3, x4) => System.Console.Write(x1 + x2 + x3 + x4);
a(""a"", ""b"", ""c"", ""d"");
".Eval().Should().Be("abcd");

    [Fact] public void ParenthesizedLambdaExpressionAction5() => @"
System.Action<string, string, string, string, string> a = (x1,x2, x3, x4, x5) => System.Console.Write(x1 + x2 + x3 + x4 + x5);
a(""a"", ""b"", ""c"", ""d"", ""e"");
".Eval().Should().Be("abcde");

    [Fact] public void ParenthesizedLambdaExpressionAction6() => @"
System.Action<string, string, string, string, string, string> a = (x1,x2, x3, x4, x5, x6) => System.Console.Write(x1 + x2 + x3 + x4 + x5 + x6);
a(""a"", ""b"", ""c"", ""d"", ""e"", ""f"");
".Eval().Should().Be("abcdef");

    [Fact] public void ParenthesizedLambdaExpressionFunc0() => @"
System.Func<string> f = () => ""Success"";
f();
".Eval().Should().Be("Success");

    [Fact] public void ParenthesizedLambdaExpressionFunc0Bracketed() => @"
System.Func<string> f = () => { return ""Success"" };
f();
".Eval().Should().Be("Success");

    [Fact] public void ParenthesizedLambdaExpressionFunc1() => @"
System.Func<string, string> f = (x) => x;
f(""Success"");
".Eval().Should().Be("Success");

    [Fact] public void ParenthesizedLambdaExpressionFunc2() => @"
System.Func<string, string, string> f = (x1, x2) => x1 + x2;
f(""a"", ""b"");
".Eval().Should().Be("ab");

    [Fact] public void ParenthesizedLambdaExpressionFunc3() => @"
System.Func<string, string, string, string> f = (x1, x2, x3) => x1 + x2 + x3;
f(""a"", ""b"", ""c"");
".Eval().Should().Be("abc");

    [Fact] public void ParenthesizedLambdaExpressionFunc4() => @"
System.Func<string, string, string, string, string> f = (x1, x2, x3, x4) => x1 + x2 + x3 + x4;
f(""a"", ""b"", ""c"", ""d"");
".Eval().Should().Be("abcd");

    [Fact] public void ParenthesizedLambdaExpressionFunc5() => @"
System.Func<string, string, string, string, string, string> f = (x1, x2, x3, x4, x5) => x1 + x2 + x3 + x4 + x5;
f(""a"", ""b"", ""c"", ""d"", ""e"");
".Eval().Should().Be("abcde");

    [Fact] public void ParenthesizedLambdaExpressionFunc6() => @"
System.Func<string, string, string, string, string, string, string> f = (x1, x2, x3, x4, x5, x6) => x1 + x2 + x3 + x4 + x5 + x6;
f(""a"", ""b"", ""c"", ""d"", ""e"", ""f"");
".Eval().Should().Be("abcdef");
}
