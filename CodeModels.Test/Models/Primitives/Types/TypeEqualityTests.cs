using FluentAssertions;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.Models.Primitives.Types;

public class TypeEqualityTests
{
    [Fact]
    public void Equality() => Type("int").Should().Be(Type("int"));

    [Fact]
    public void QuickTypeEquality() => QuickType("int").Should().Be(QuickType("int"));
    [Fact]
    public void QuickTypeOptionalEquality() => QuickType("int?").Should().Be(QuickType("int?"));
    [Fact]
    public void QuickTypeArrayEquality() => QuickType("int[]").Should().Be(QuickType("int[]"));
    [Fact]
    public void QuickTypeGenericEquality() => QuickType("List<int>").Should().Be(QuickType("List<int>"));

    [Fact]
    public void FromReflectionVsParse() => Type<int>().Should().BeEquivalentTo(Type("int"));
    [Fact]
    public void FromReflectionVsParseOptional() => Type<int?>().Should().BeEquivalentTo(Type("int?"));
    [Fact]
    public void FromReflectionVsParseArray() => Type<int[]>().Should().BeEquivalentTo(Type("int[]"));

    [Fact]
    public void FromReflectionVsQuickType() => Type<int>().Should().BeEquivalentTo(QuickType("int"));
    [Fact]
    public void FromReflectionVsQuickTypeOptional() => Type<int?>().Should().BeEquivalentTo(QuickType("int?"));
    [Fact]
    public void FromReflectionVsQuickTypeArray() => Type<int[]>().Should().BeEquivalentTo(QuickType("int[]"));
}
