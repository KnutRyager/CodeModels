using System;
using CodeModels.Execution;
using FluentAssertions;
using Xunit;

namespace CodeModels.Test.Execution.Expression;

public class StaticReferenceExpressionTests
{
    [Fact] public void NowDay() => @"
System.DateTime.Now.Day;
".Eval().Should().Be(DateTime.Now.Day);

    [Fact] public void NowDayToString() => @"
System.DateTime.Now.Day.ToString();
".Eval().Should().Be(DateTime.Now.Day.ToString());
}
