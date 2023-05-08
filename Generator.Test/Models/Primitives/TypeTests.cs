using Common.DataStructures;
using FluentAssertions;
using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;
using static CodeAnalyzation.Test.TestUtil;

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
        => QuickType("List", new EqualityList<IType>() { QuickType("int") }).Should().BeEquivalentTo(QuickType("List<int>"));
    [Fact]
    public void ParseGenericIdentifierWithInnerGeneric()
        => QuickType("List", new EqualityList<IType>() { QuickType("List", new EqualityList<IType>() { QuickType("int") }) })
        .Should().BeEquivalentTo(QuickType("List<List<int>>"));
    [Fact]
    public void ParseGenericIdentifierWithMultipleInnerGeneric()
        => QuickType("A", new EqualityList<IType>() { QuickType("B", new EqualityList<IType>() { QuickType("C"), QuickType("D") }) })
        .Should().BeEquivalentTo(QuickType("A<B<C,D>>"));
    [Fact]
    public void ArrayToCode() => QuickType("int",isMulti:true).Code().Should().BeEquivalentTo("int[]");
    [Fact]
    public void GenericToCode() => QuickType("List", new EqualityList<IType>() { QuickType("int") }).Code().Should().BeEquivalentTo("List<int>");
    [Fact]
    public void GenericWithArrayToCode() => QuickType("List", new EqualityList<IType>() { QuickType("int",isMulti:true) })
        .Code().Should().BeEquivalentTo("List<int[]>");
}
