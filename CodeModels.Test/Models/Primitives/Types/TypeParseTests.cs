using System;
using CodeModels.Models;
using Common.DataStructures;
using FluentAssertions;
using TestCommon;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;
using static TestCommon.TestUtil;

namespace CodeModels.Test.Models.Primitives.Types;

public class TypeParseTests
{
    [Fact]
    public void ParseInt() => Type("int").Syntax().CodeEqual("int");
    [Fact]
    public void ParseOptionalInt() => Type("int?").Syntax().CodeEqual("int?");
    [Fact(Skip = "Not supported")]
    public void ParseOutSimplified() => Type("Int64&").Syntax().CodeEqual("out long");
    [Fact]
    public void ParseArrayInt() => Type("int[]").Syntax().CodeEqual("int[]");
    [Fact]
    public void ParseListOfArray() => Type("List<int[]>").Syntax().CodeEqual("List<int[]>");
    [Fact]
    public void ParseObject() => Type("object").Syntax().CodeEqual("object");
    [Fact]
    public void ParseGenericIdentifierSimple()
        => QuickType("List", new EqualityList<IType>() { QuickType("int") }).Should().BeEquivalentTo(QuickType("List<int>"));
    [Fact]
    public void ParseGenericIdentifierWithInnerGeneric()
        => QuickType("List", new EqualityList<IType>() { QuickType("List", new EqualityList<IType>() { QuickType("int") }) })
        .Should().BeEquivalentTo(QuickType("List<List<int>>"));
    [Fact]
    public void ParseGenericIdentifierWithMultipleInnerGeneric()
        => QuickType("A", new EqualityList<IType>() { QuickType("B", new EqualityList<IType>() { QuickType("C"), QuickType("D") }) })
        .Should().BeEquivalentTo(QuickType("A<B<C,D>>"));
}
