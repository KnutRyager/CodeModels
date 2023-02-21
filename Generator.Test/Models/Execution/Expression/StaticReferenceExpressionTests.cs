using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Models.Execution;
using FluentAssertions;
using Xunit;

namespace Generator.Test.Models.Execution.Expression;

public class StaticReferenceExpressionTests
{
    [Fact] public void NowDay() => @"
System.DateTime.Now.Day;
".Eval().Should().Be(DateTime.Now.Day);

    [Fact] public void NowDayToString() => @"
System.DateTime.Now.Day.ToString();
".Eval().Should().Be(DateTime.Now.Day.ToString());
}
