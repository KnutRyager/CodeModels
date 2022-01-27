using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;
using static CodeAnalyzation.Test.TestUtil;
using FluentAssertions;
using System.Collections.Generic;
using System.Collections.Immutable;
using Common.DataStructures;

namespace CodeAnalyzation.Models.Primitives.Test;


public class TypeTests
{
    [Fact]
    public void Equality() => Type("int").Should().Be(Type("int"));
    [Fact]
    public void ParseInt() => Type("int").Syntax().CodeEqual("int");
    [Fact]
    public void ParseOptionalInt() => Type("int?").Syntax().CodeEqual("int?");
    [Fact]
    public void ParseObject() => Type("object").Syntax().CodeEqual("object");
    [Fact]
    public void FromTypeInt() => Type(typeof(int)).Syntax().CodeEqual("int");
    [Fact]
    public void FromTypeObject() => Type(typeof(object)).Syntax().CodeEqual("object");
    [Fact]
    public void ParseGenericIdentifierSimple()
        => new QuickType("List", new EqualityList<IType>() { new QuickType("int") }).Should().BeEquivalentTo(new QuickType("List<int>"));
    [Fact]
    public void ParseGenericIdentifierWithInnerGeneric()
        => new QuickType("List", new EqualityList<IType>() { new QuickType("List", new EqualityList<IType>() { new QuickType("int") }) })
        .Should().BeEquivalentTo(new QuickType("List<List<int>>"));
    [Fact]
    public void ParseGenericIdentifierWithMultipleInnerGeneric()
        => new QuickType("A", new EqualityList<IType>() { new QuickType("B", new EqualityList<IType>() { new QuickType("C"), new QuickType("D") }) })
        .Should().BeEquivalentTo(new QuickType("A<B<C,D>>"));
    [Fact]
    public void ArrayToCode() => new QuickType("int",isMulti:true).Code().Should().BeEquivalentTo("int[]");
    [Fact]
    public void GenericToCode() => new QuickType("List", new EqualityList<IType>() { new QuickType("int") }).Code().Should().BeEquivalentTo("List<int>");
    [Fact]
    public void GenericWithArrayToCode() => new QuickType("List", new EqualityList<IType>() { new QuickType("int",isMulti:true) })
        .Code().Should().BeEquivalentTo("List<int[]>");
}
