using CodeModels.Models.Execution;
using FluentAssertions;
using Xunit;

namespace CodeModels.Models.Execution.ControlFlow.Test;

public class ReturnTests
{
    [Fact] public void PlainReturnLiteral() => @"
return 5;".Eval().Should().Be(5);

    [Fact] public void PlainReturnVariable() => @"
var a = 5;
return a;".Eval().Should().Be(5);

    [Fact] public void YouOnlyReturnOnce() => @"
return 5;
return 4;".Eval().Should().Be(5);
}
