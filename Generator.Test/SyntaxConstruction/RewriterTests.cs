using CodeAnalyzation;
using CodeAnalyzation.SyntaxConstruction;
using Xunit;

namespace Generator.Test.SyntaxConstruction;

public class RewriterTests
{
    public class A { };
    public class B : A { };
    public class C : A { };
    [Fact]
    public void PublicRewriter()
        => "using System; public class A {public int P1 { get; set;} }".CodeEqual(
                       "using System; public class A {public int P1 { get; set;} private int P2 { get; set;} }"
                       .Parse().GetVisit(new PublicOnlyRewriter()));

    [Fact]
    public void PublicRewriter2()
        => "using System; public class A {public int P1 { get; set;} }".CodeEqual(
                       "using System; public class A {public int P1 { get; set;} private int P2 { get; set;} }"
                       .Parse().GetVisit(new PublicOnlyRewriter()));
}
