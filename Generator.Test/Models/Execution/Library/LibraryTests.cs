using System.Linq;
using System;
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

    [Fact] public void LinqExtension2() => @"
using System;
using System.Linq;
using System.Collections.Generic;
Math.Sqrt(Enumerable.Sum(Enumerable.Range(200, 400).ToList()));
//System.Math.Sqrt(System.Enumerable.Range(200, 400).ToList().Sum());
".Eval().Should().Be(399.7499218261337);

    private void Test()
    {
        Math.Sqrt(Enumerable.Sum(Enumerable.Range(200, 400).ToList<int>()));
        //Math.Sqrt(Enumerable.Range(200, 400).ToList().Sum());
    }
}
