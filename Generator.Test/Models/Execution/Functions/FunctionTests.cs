using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.Function.Test;

public class FunctionTests
{
    [Fact] public void EvaluateFunctionDefinition() => @"
int newN() { return 3; }".Eval().Should().Be(3);

    [Fact] public void RunLocalFunction() => @"
int newN() { return 3; }
newN();".Eval().Should().Be(3);

    [Fact] public void StaticVariableInFunction() => @"
int x = 1;
int newN() { x *= 2; return x; }
newN();
newN();
newN();".Eval().Should().Be(8);

    [Fact] public void MainFunction() => @"
using System;

class MainClass {
  public static void Main (string[] args) {
    System.Console.Write(4+5);
  }
}
".Eval().Should().Be("9");

    [Fact] public void LocalFunction() => @"
int Squared100() {
    int sum = 0;
    for (var i = 1; i <= 100; i++) {
        sum += System.Math.Pow(i,2);
    }
    return sum;
}".Eval().Should().Be(338350);
}
