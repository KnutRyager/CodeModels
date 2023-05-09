using Xunit;
using static CodeModels.Models.CodeModelFactory;
using static TestCommon.TestUtil;
using FluentAssertions;

namespace CodeModels.Models.Primitives.Test;

public class ExpressionTests
{
    [Fact] public void LiteralInt() => Literal(4).Syntax().ToString().Should().Be("4");
    [Fact] public void LiteralString() => Literal("A").Syntax().ToString().Should().Be(@"""A""");
    [Fact] public void LiteralDouble() => Literal(4.1).Syntax().ToString().Should().Be("4.1D");
    [Fact] public void LiteralBool() => Literal(true).Syntax().ToString().Should().Be("true");
}
