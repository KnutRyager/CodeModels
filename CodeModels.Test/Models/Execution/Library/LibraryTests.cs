using System.Linq;
using System;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;

namespace CodeModels.Models.Execution.Library.Test;

public class LibraryTests
{
    [Fact] public void LinqExtension() => @"
using System;
using System.Linq;
    int sum = System.Linq.Enumerable.Range(250, 571).Sum();
    double squareRoot = System.Math.Sqrt(sum);
    System.Console.Write(""It works, answer is {0:F2}"", squareRoot);
".Eval().Should().Be("It works, answer is 552.71");

    [Fact(Skip = "Can't resolve Enumerable.Range overload")]
    public void LinqExtensionManually() => @"
using System;
using System.Linq;
using System.Collections.Generic;
Enumerable.Range(0, 1).ToList();
//System.Math.Sqrt(System.Enumerable.Range(200, 400).ToList().Sum());
".Eval().Should().Be(new List<int>() { 0, 1 });
}
