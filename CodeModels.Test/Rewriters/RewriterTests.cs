using CodeModels.Extensions;
using CodeModels.Parsing;
using FluentAssertions;
using TestCommon;
using Xunit;

namespace CodeModels.Rewriters.Test;

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

    [Fact]
    public void TopLevelStatementRewriter()
        => @"
using System;

class Program
{
static void Main(string[] args)
{
Console.WriteLine(""Hello, world!"");}}".Parse().GetVisit(new TopLevelStatementRewriter()).ToString().Should().Be(
@"using System;
Console.WriteLine(""Hello, world!"");");
}
