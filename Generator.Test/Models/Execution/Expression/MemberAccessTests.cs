using System.Net.Http;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.Expression.Test;

public class MemberAccessTests
{
    [Fact] public void EnumPropertyAccess() => @"
System.Text.RegularExpressions.RegexOptions.IgnoreCase;
".Eval().Should().Be(RegexOptions.IgnoreCase);

    [Fact] public void EnumPropertyAccess2() => @"
var x = System.Net.Http.HttpVersionPolicy.RequestVersionExact;
".Eval().Should().Be(HttpVersionPolicy.RequestVersionExact);
}
