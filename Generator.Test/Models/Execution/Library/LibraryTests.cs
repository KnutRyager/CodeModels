using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.Library.Test;

public class LibraryTests
{
    [Fact] public void LinqExtension() => @"
using System;
using System.Linq;
    int sum = System.Linq.Enumerable.Range(250, 571).Sum();
    double squareRoot = System.Math.Sqrt(sum);
    System.Console.Write(""It works, answer is {0:F2}"", squareRoot);
".Eval().Should().Be("It works, answer is 552.71");
}
