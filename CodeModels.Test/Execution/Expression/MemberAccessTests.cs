using System.Net.Http;
using System.Text.RegularExpressions;
using CodeModels.Execution;
using FluentAssertions;
using Xunit;

namespace CodeModels.Test.Execution.Expression;

public class MemberAccessTests
{
    [Fact] public void EnumPropertyAccess() => @"
System.Text.RegularExpressions.RegexOptions.IgnoreCase;
".Eval().Should().Be(RegexOptions.IgnoreCase);

    [Fact] public void EnumPropertyAccess2() => @"
var x = System.Net.Http.HttpVersionPolicy.RequestVersionExact;
".Eval().Should().Be(HttpVersionPolicy.RequestVersionExact);

    [Fact] public void EnumPropertyAccess3() => @"
System.Environment.SpecialFolder.MyDocuments;
".Eval().Should().Be(System.Environment.SpecialFolder.MyDocuments);
}
