using Common.Reflection;
using Xunit;

namespace Common.Test.Reflection
{
    public class ExpressionReflectionUtilTests
    {
        [Fact]
        public void InstanceFieldInfo()
        {
            var actual = ExpressionReflectionUtil.GetFieldInfo<A>(x => x.InstanceField);
            var expected = typeof(A).GetField("InstanceField");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FieldInfoStatic()
        {
            var actual = ExpressionReflectionUtil.GetFieldInfo(x => A.StaticField);
            var expected = typeof(A).GetField("StaticField");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Constructor()
        {
            var actual = ExpressionReflectionUtil.GetConstructorInfo(x => new A());
            var expected = typeof(A).GetConstructor(new Type[] { });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PropertyInfoInstance()
        {
            var actual = ExpressionReflectionUtil.GetPropertyInfo<A>(x => x.InstanceProperty);
            var expected = typeof(A).GetProperty("InstanceProperty");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PropertyInfoStatic()
        {
            var actual = ExpressionReflectionUtil.GetPropertyInfo(x => A.StaticProperty);
            var expected = typeof(A).GetProperty("StaticProperty");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MethodInfoInstance()
        {
            var actual = ExpressionReflectionUtil.GetMethodInfo<A>(x => x.InstanceMethod1);
            var expected = typeof(A).GetMethod("InstanceMethod1");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MethodInfoTwoParamsInstance()
        {
            var actual = ExpressionReflectionUtil.GetMethodInfo<A>(x => x.InstanceMethodTwoParams);
            var expected = typeof(A).GetMethod("InstanceMethodTwoParams");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MethodInfoInstanceGeneric()
        {
            var actual = ExpressionReflectionUtil.GetMethodInfo<A>(x => x.InstanceMethodGeneric<int>);
            var expected = ReflectionUtil.GetGenericMethod<A>("InstanceMethodGeneric", genericArguments: new[] { typeof(int) });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MethodInfoStatic()
        {
            var actual = ExpressionReflectionUtil.GetMethodInfo(x => A.StaticMethod);
            var expected = typeof(A).GetMethod("StaticMethod");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MethodInfoGenericStatic()
        {
            var actual = ExpressionReflectionUtil.GetMethodInfo(x => A.StaticMethodGeneric<int>);
            var expected = ReflectionUtil.GetGenericMethod<A>("StaticMethodGeneric", genericArguments: new[] { typeof(int) });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MethodInfoInstanceResolveOverload()
        {
            var actualInt = ExpressionReflectionUtil.GetMethodInfo<A>(x => x.InstanceMethod2(T<int>()));
            var expectedInt = typeof(A).GetMethod("InstanceMethod2", new[] { typeof(int) });
            Assert.Equal(expectedInt, actualInt);
            var actualString = ExpressionReflectionUtil.GetMethodInfo<A>(x => x.InstanceMethod2(T<string>()));
            var expectedString = typeof(A).GetMethod("InstanceMethod2", new[] { typeof(string) });
            Assert.Equal(expectedString, actualString);
        }

        [Fact]
        public void MethodInfoStaticResolveOverload()
        {
            var actualInt = ExpressionReflectionUtil.GetMethodInfo(x => A.StaticMethod2(T<int>()));
            var expectedInt = typeof(A).GetMethod("StaticMethod2", new[] { typeof(int) });
            Assert.Equal(expectedInt, actualInt);
            var actualString = ExpressionReflectionUtil.GetMethodInfo(x => A.StaticMethod2(T<string>()));
            var expectedString = typeof(A).GetMethod("StaticMethod2", new[] { typeof(string) });
            Assert.Equal(expectedString, actualString);
        }

        private static T T<T>() => default;
    }
}

file class A
{
    public string InstanceField = "";
    public string InstanceProperty { get; set; }
    public string InstanceMethod1() => "";
    public string InstanceMethod2(int x) => "";
    public string InstanceMethod2(string x) => "";
    public string InstanceMethodTwoParams(int x, string y) => "";
    public string InstanceMethodGeneric<T>() => "";
    public static string StaticField = "";
    public static string StaticProperty { get; set; }
    public static string StaticMethod() => "";
    public static string StaticMethodGeneric<T>() => "";
    public static string StaticMethod2(int x) => "";
    public static string StaticMethod2(string x) => "";
}

// TODO
//ExpressionType.Call => _Parse(type),
//        ExpressionType.Conditional => _Parse(type),
//        ExpressionType.Constant => _Parse(type),
//        ExpressionType.Invoke => _Parse(type),
//        ExpressionType.Lambda => _Parse(type),
//        ExpressionType.ListInit => _Parse(type),
//        ExpressionType.MemberAccess => _Parse(type),
//        ExpressionType.MemberInit => _Parse(type),
//        ExpressionType.New => _Parse(type),
//        ExpressionType.NewArrayInit => _Parse(type),
//        ExpressionType.NewArrayBounds => _Parse(type),
//        ExpressionType.Parameter => _Parse(type),
//        ExpressionType.TypeIs => _Parse(type),
//        ExpressionType.Assign => _Parse(type),
//        ExpressionType.Block => _Parse(type),
//        ExpressionType.DebugInfo => _Parse(type),
//        ExpressionType.Decrement => _Parse(type),
//        ExpressionType.Dynamic => _Parse(type),
//        ExpressionType.Default => _Parse(type),
//        ExpressionType.Extension => _Parse(type),
//        ExpressionType.Goto => _Parse(type),
//        ExpressionType.Increment => _Parse(type),
//        ExpressionType.Index => _Parse(type),
//        ExpressionType.Label => _Parse(type),
//        ExpressionType.RuntimeVariables => _Parse(type),
//        ExpressionType.Loop => _Parse(type),
//        ExpressionType.Switch => _Parse(type),
//        ExpressionType.Throw => _Parse(type),
//        ExpressionType.Try => _Parse(type),
//        ExpressionType.Unbox => _Parse(type),
//        ExpressionType.AddAssign => _Parse(type),
//        ExpressionType.AndAssign => _Parse(type),
//        ExpressionType.DivideAssign => _Parse(type),
//        ExpressionType.ExclusiveOrAssign => _Parse(type),
//        ExpressionType.LeftShiftAssign => _Parse(type),
//        ExpressionType.ModuloAssign => _Parse(type),
//        ExpressionType.PowerAssign => _Parse(type),
//        ExpressionType.MultiplyAssign => _Parse(type),
//        ExpressionType.OrAssign => _Parse(type),
//        ExpressionType.RightShiftAssign => _Parse(type),
//        ExpressionType.SubtractAssign => _Parse(type),
//        ExpressionType.AddAssignChecked => _Parse(type),
//        ExpressionType.MultiplyAssignChecked => _Parse(type),
//        ExpressionType.SubtractAssignChecked => _Parse(type),
//        ExpressionType.PreIncrementAssign => _Parse(type),
//        ExpressionType.PreDecrementAssign => _Parse(type),
//        ExpressionType.PostIncrementAssign => _Parse(type),
//        ExpressionType.PostDecrementAssign => _Parse(type),
//        ExpressionType.TypeEqual => _Parse(type),
//        ExpressionType.OnesComplement => _Parse(type),
//        ExpressionType.IsTrue => _Parse(type),
//        ExpressionType.IsFalse => _Parse(type),