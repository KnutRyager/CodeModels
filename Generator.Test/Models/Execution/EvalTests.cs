using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.Test;

public class EvalTests
{
    [Fact] public void AddVariables() => "var a = 2; var b = 3; a + b;".Eval().Should().Be(5);
    [Fact] public void IfTrue() => "var a = true; if(a) { \"yup\" } else { \"nope\" };".Eval().Should().Be("yup");
    [Fact] public void IfFalse() => "var a = false; if(a) { \"yup\" } else { \"nope\"; }".Eval().Should().Be("nope");
    [Fact] public void ForLoop() => "var a = 0; for(var i = 0; i < 10; i++){ a += i; } a;".Eval().Should().Be(45);
    [Fact] public void WhileLoop() => "var a = 0; var i = 0; while(i < 10){ a += i; i++; } a;".Eval().Should().Be(45);
    [Fact] public void DoWhileLoop() => "var a = 0; var i = 0; do { a += i; i++; } while(i < 10); a;".Eval().Should().Be(45);
    [Fact] public void ForEachLoop() => "var a = 0; foreach(var i in new int[]{ 1, 2, 3, 4, 5 }) { a += i; } a;".Eval().Should().Be(15);
    [Fact] public void Return() => "return 5;".Eval().Should().Be(5);
    [Fact] public void Function() => "int newN() { return 3; }".Eval().Should().Be(3);
    [Fact] public void FunctionLamda() => "int newN() => 3;".Eval().Should().Be(3);
    [Fact] public void RunLocalFunction() => "int newN() { return 3; } newN();".Eval().Should().Be(3);
    [Fact] public void RunLocalLambda() => "int newN() => 3; newN();".Eval().Should().Be(3);
    [Fact] public void StaticVariableInFunction() => "int x = 1; int newN() { x *= 2; return x; } newN();newN();newN();".Eval().Should().Be(8);

    [Fact] public void TopLevelStatements() => @"
int sum = 0;
for (var i = 1; i <= 100; i++) {
    sum += System.Math.Pow(i,2);
}

return sum;".Eval().Should().Be(338350);

    [Fact] public void LocalFunction() => @"
int Squared100() {
    int sum = 0;
    for (var i = 1; i <= 100; i++) {
        sum += System.Math.Pow(i,2);
    }

    return sum;
}".Eval().Should().Be(338350);

    [Fact] public void SquaredSum() => @"
int sum = 0;

for (int i = 1; i <= 100; i++)
{
    sum += System.Math.Pow(i, 2);
}

 sum;
".Eval().Should().Be(338350);

    [Fact] public void NowDay() => @"System.DateTime.Now.Day;".Eval().Should().Be(System.DateTime.Now.Day);
    [Fact] public void NowDayToString() => @"System.DateTime.Now.Day.ToString();".Eval().Should().Be(System.DateTime.Now.Day.ToString());
    [Fact] public void NowDayToConsoleWrite() => "// Start code\r\nSystem.Console.Write(System.DateTime.Now);".Eval().Should().Be(System.DateTime.Now.Day.ToString());
    [Fact] public void NowDayToStringConsoleWrite() => @"System.Console.Write(System.DateTime.Now.Day.ToString());".Eval().Should().Be(System.DateTime.Now.Day.ToString());

    [Fact] public void NowViaVariable() => @"
System.DateTime now = System.DateTime.Now;
return now.Day;
".Eval().Should().Be(System.DateTime.Now.Day);

    [Fact] public void ConsoleWrite() => "System.Console.Write(4+5);".Eval().Should().Be("9");
    [Fact] public void ConsoleWriteLine() => "System.Console.WriteLine(4+5);".Eval().Should().Be("9\r\n");

    [Fact]
    public void Test()
    {
        var m1 = typeof(IEnumerable<int>).GetMethods();
        var m2 = typeof(Enumerable).GetMethods();
        var method = typeof(IEnumerable<int>).GetMethod("Sum");
        var method2 = typeof(Enumerable).GetMethod("Sum", new Type[] { typeof(IEnumerable<int>) });
        var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    [Fact] public void MainFunction() => @"
using System;

class MainClass {
  public static void Main (string[] args) {
    System.Console.Write(4+5);
  }
}
".Eval().Should().Be("9");

    [Fact] public void LinqExtension() => @"
using System;
using System.Linq;
    int sum = System.Linq.Enumerable.Range(250, 571).Sum();
    double squareRoot = System.Math.Sqrt(sum);
    System.Console.Write(""It works, answer is {0:F2}"", squareRoot);
".Eval().Should().Be("It works, answer is 552.71");
}
