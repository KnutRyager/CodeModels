using CodeModels.Models;
using Common.DataStructures;
using FluentAssertions;
using TestCommon;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;
using static TestCommon.TestUtil;

namespace CodeModels.Test.Models.Primitives.Types;

public class TypeResolveReflectionTests
{
    [Fact]
    public void ResolveReflectionBasicTypes() => Type("int").ReflectedType.Should().Be(typeof(int));
    [Fact]
    public void QuickTypeResolveReflectionBasicTypes() => QuickType("int").ReflectedType.Should().Be(typeof(int));
    [Fact]
    public void ResolveReflectionOptionalBasicTypes() => Type("int?").ReflectedType.Should().Be(typeof(int?));
    [Fact]
    public void QuickTypeResolveReflectionOptionalBasicTypes() => QuickType("int?").ReflectedType.Should().Be(typeof(int?));
}
