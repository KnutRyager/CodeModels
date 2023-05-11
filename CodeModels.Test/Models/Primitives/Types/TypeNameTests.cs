using FluentAssertions;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.Models.Primitives.Types;

public class TypeNameTests
{
    [Fact]
    public void FromReflectionOptionalName() => Type<int?>().Name.Should().Be("int?");
    [Fact]
    public void FromReflectionOptionalTypeName() => Type<int?>().TypeName.Should().Be("int");
    [Fact]
    public void FromReflectionArrayName() => Type<int[]>().Name.Should().Be("int[]");
    [Fact]
    public void FromReflectionArrayTypeName() => Type<int[]>().TypeName.Should().Be("int");
    [Fact(Skip = "Next level")]
    public void FromReflectionOptionalArrayName() => Type<int[]?>().Name.Should().Be("int");
    [Fact(Skip = "Next level")]
    public void FromReflectionOptionalArrayTypeName() => Type<int[]?>().TypeName.Should().Be("int[]?");
    [Fact(Skip = "Next level")]
    public void FromReflectionArrayOfOptionalName() => Type<int?[]>().Name.Should().Be("int?");
    [Fact(Skip = "Next level")]
    public void FromReflectionArrayOfOptionalTypeName() => Type<int?[]?>().TypeName.Should().Be("int?[]");
    [Fact]
    public void QuickTypeOptionalName() => QuickType("int?").Name.Should().Be("int?");
    [Fact]
    public void QuickTypeOptionalTypeName() => QuickType("int?").TypeName.Should().Be("int");
    [Fact]
    public void QuickTypeReferenceOptionalName() => QuickType("string?").Name.Should().Be("string?");
    [Fact]
    public void QuickTypeReferenceOptionalTypeName() => QuickType("string?").TypeName.Should().Be("string");
    [Fact]
    public void QuickTypeArrayName() => QuickType("int[]").Name.Should().Be("int[]");
    [Fact]
    public void QuickTypeArrayTypeName() => QuickType("int[]").TypeName.Should().Be("int");
}
