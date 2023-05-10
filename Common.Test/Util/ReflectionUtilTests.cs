using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using Common.Reflection;
using FluentAssertions;
using Xunit;

namespace Common.Tests
{
    public class ReflectionUtilTests
    {
        public class A
        {
            [DisplayName("Prop 1")]
            public string Prop1 { get; set; }
        }

        [Fact]
        public void GetPropertyAttributes() => Assert.Equal("Prop 1", ReflectionUtil.GetPropertyAttributes<A, DisplayNameAttribute>()[0].DisplayName);

        [Fact]
        public void GetMethodInfo_Lambda() => Assert.Equal(
            typeof(int).GetMethod(nameof(int.ToString), Array.Empty<Type>()),
            ReflectionUtil.GetMethodInfo<int, string>(x => x.ToString()));

        [Fact]
        public void GetMethodInfo_String() => Assert.Equal(
            typeof(int).GetMethod(nameof(int.ToString), Array.Empty<Type>()),
            ReflectionUtil.GetMethodInfo<int>(nameof(int.ToString)));

        [Fact]
        public void GetStaticMethodInfo_String() => Assert.Equal(
            typeof(int).GetMethod(nameof(int.Parse), new Type[] { typeof(string) }),
            ReflectionUtil.GetMethodInfo<int, string>(nameof(int.Parse)));

        [Fact]
        public void GetAssemblyDependencies()
        {
            var directDependencies = ReflectionUtil.GetDirectDependencies(typeof(Expression).Assembly);
            var directDependenciesOfDependencies = directDependencies.Select(x => ReflectionUtil.GetDirectDependencies(x)).ToArray();
            var allDependencies = ReflectionUtil.GetAllDependencies(typeof(Expression).Assembly);
            var systemAssembly = typeof(int).Assembly;
            var collectionsAssembly = typeof(BitArray).Assembly;
            Assert.Empty(ReflectionUtil.GetDirectDependencies(systemAssembly));
            Assert.Empty(ReflectionUtil.GetAllDependencies(systemAssembly));
            Assert.Contains(systemAssembly, allDependencies);
            Assert.Contains(systemAssembly, ReflectionUtil.GetDirectDependencies(collectionsAssembly));
            Assert.DoesNotContain(systemAssembly, directDependencies);
            Assert.Contains(collectionsAssembly, allDependencies);
            Assert.Contains(collectionsAssembly, directDependencies);
        }

        [Fact] public void IsNullableValueTypeTrue() => Assert.True(ReflectionUtil.IsNullable<int?>());
        [Fact] public void IsNullableValueTypeFalse() => Assert.False(ReflectionUtil.IsNullable<int>());

        [Fact] public void IsSimplyfiableNullableTypeTrue() => Assert.True(ReflectionUtil.IsSimplyfiableNullableType(typeof(int), false));
        [Fact] public void IsSimplyfiableNullableTypeNotNullable() => Assert.False(ReflectionUtil.IsSimplyfiableNullableType(typeof(int), true));
        [Fact] public void IsSimplyfiableNullableTypeAlreadyNullable() => Assert.False(ReflectionUtil.IsSimplyfiableNullableType(typeof(int?), false));
        [Fact] public void SimplifyNullableTypeTrue() => ReflectionUtil.SimplifyNullableType(typeof(int), false).Should().Be(typeof(int?));
        [Fact] public void SimplifyNullableTypeNotNullable() => ReflectionUtil.SimplifyNullableType(typeof(int), true).Should().Be(typeof(int));
        [Fact] public void SimplifyNullableTypeAlreadyNullable() => ReflectionUtil.SimplifyNullableType(typeof(int?), false).Should().Be(typeof(int?));
        [Fact] public void IsSimplyfiableArrayTypeTrue() => Assert.True(ReflectionUtil.IsSimplyfiableArrayType(typeof(int), true));
        [Fact] public void IsSimplyfiableArrayTypeNotArray() => Assert.False(ReflectionUtil.IsSimplyfiableArrayType(typeof(int), false));
        [Fact] public void IsSimplyfiableArrayTypeAlreadyArray() => Assert.False(ReflectionUtil.IsSimplyfiableArrayType(typeof(int[]), true));
        [Fact] public void SimplifyArrayTypeTrue() => ReflectionUtil.SimplifyArrayType(typeof(int), true).Should().Be(typeof(int[]));
        [Fact] public void SimplifyArrayTypeAlreadyArray() => ReflectionUtil.SimplifyArrayType(typeof(int[]), true).Should().Be(typeof(int[]));
        [Fact] public void SimplifyArrayTypeNotArray() => ReflectionUtil.SimplifyArrayType(typeof(int), false).Should().Be(typeof(int));
    }
}
