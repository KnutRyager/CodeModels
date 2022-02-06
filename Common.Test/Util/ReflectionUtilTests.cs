using Common.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
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
    }
}
