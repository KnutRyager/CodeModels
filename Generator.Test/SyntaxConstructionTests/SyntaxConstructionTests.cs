using CodeAnalyzation;
using CodeAnalyzation.SyntaxConstruction;
using static CodeAnalysisTests.TestUtil;
using Xunit;

namespace CodeAnalysisTests;

[Collection("Sequential")]
public class SyntaxConstructionTests
{
    public class A { };
    public class B : A { };
    public class C : A { };
    [Fact]
    public void PublicRewriter()
        => CodeEqual("using System; public class A {public int P1 { get; set;} }",
                       "using System; public class A {public int P1 { get; set;} private int P2 { get; set;} }"
                       .Parse().GetVisit(new PublicOnlyRewriter()));

    [Fact]
    public void PublicRewriter2()
        => CodeEqual("using System; public class A {public int P1 { get; set;} }",
                       "using System; public class A {public int P1 { get; set;} private int P2 { get; set;} }"
                       .Parse().GetVisit(new PublicOnlyRewriter()));
}
