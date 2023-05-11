using CodeModels.Models;
using Common.DataStructures;
using FluentAssertions;
using TestCommon;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;
using static TestCommon.TestUtil;

namespace CodeModels.Test.Models.Primitives.Types;

public class TypeTests
{
    [Fact]
    public void CanonalizePredefinedType() => Type("System.Int32").Should().BeEquivalentTo(Type("int"));
    [Fact]
    public void FromTypeInt() => Type(typeof(int)).Syntax().CodeEqual("int");
    [Fact]
    public void FromTypeObject() => Type(typeof(object)).Syntax().CodeEqual("object");
    [Fact]
    public void ArrayToCode() => QuickType("int[]").Code().Should().BeEquivalentTo("int[]");
    [Fact]
    public void GenericToCode() => QuickType("List", new EqualityList<IType>() { QuickType("int") }).Code().Should().BeEquivalentTo("List<int>");
    [Fact]
    public void GenericWithArrayToCode() => QuickType("List", new EqualityList<IType>() { QuickType("int[]") })
        .Code().Should().BeEquivalentTo("List<int[]>");
}
