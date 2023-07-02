using Common.Reflection;
using FluentAssertions;
using Xunit;

namespace Common.Tests.Reflection;

public class MemberAccessTests
{
    [Fact]
    public void DeepMemberAccess()
    {
        var abcAccess = ReflectionPropertySearch.GetMemberAccess(typeof(A), typeof(State));
        var a = new A(new B(null, null, new C(State.SUCCESS)));
        abcAccess.Invoke(a).Should().Be(State.SUCCESS);
    }
}

file record A(B B);
file record B(A A, B TheB, C C);
file record C(State S);
file enum State { FAILURE, SUCCESS }

