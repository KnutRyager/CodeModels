using System;
using CodeModels.Models.Execution;
using CodeModels.Models.Execution.ControlFlow;
using FluentAssertions;
using Xunit;

namespace CodeModels.Models.Execution.ControlFlow.Test;

public class ExceptionTests
{
    [Fact]
    public void Throw()
    {
        Func<object?> func = () => "throw new System.ArgumentException(\"a\")".Eval(catchExceptions: true);
        func().Should().BeEquivalentTo(new ArgumentException("a"));
        Action action = () => "throw new System.ArgumentException(\"a\")".Eval();
        action.Should().Throw<ThrowException>().WithInnerException<ArgumentException>("a");
    }

    [Fact] public void TryCatchFlow() => @"
using System;
var s = 0;
try
{
    s += 1;
    if (Math.Pow(2,2) > 1)
        throw new Exception();
    s += 10;
}
catch (Exception)
{
    s += 100;
}
s;
".Eval().Should().Be(101);

    [Fact] public void CatchExceptionMessage() => @"
using System;
var a = null;
try
{
    throw new ArgumentException(""abc"");
}
catch (ArgumentException e)
{
    a = e.Message;
}
a;
".Eval().Should().Be("abc");

    [Fact] public void CatchCorrectException() => @"
using System;
var a = null;
try
{
    throw new ArgumentException(""abc"");
}
catch (NullReferenceException e)
{
    a = ""wrong exception matched"";
}
catch (ArgumentException e)
{
    a = e.Message;
}
catch (DivideByZeroException e)
{
    a = ""wrong exception matched"";
}
a;
".Eval().Should().Be("abc");

    [Fact] public void CatchInheritedException() => @"
using System;
var a = null;
try
{
    throw new ArgumentException(""abc"");
}
catch (Exception e)
{
    a = e.Message;
}
a;
".Eval().Should().Be("abc");

    [Fact] public void FinallyTriggersOnNoThrow() => @"
using System;
var s = 0;
try
{
    s += 1;
}
catch (Exception)
{
    s += 10;
}
finally {
    s += 100;
}
s;
".Eval().Should().Be(101);

    [Fact] public void FinallyTriggersOnThrow() => @"
using System;
var s = 0;
try
{
    s += 1;
    throw new ArgumentException(""abc"");
}
catch (ArgumentException)
{
    s += 10;
}
finally {
    s += 100;
}
s;
".Eval().Should().Be(111);
}
