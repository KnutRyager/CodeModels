using Common.Reflection;
using FluentAssertions;
using Xunit;

namespace Common.Tests.Reflection;

public class ReflectionSerializationTests
{
    private const string MsCorLib = "";
    private const string CommonLib = "Common.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
    private const string SystemLib = "System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e";
    private const string SystemLibSpaced = "System.Private.CoreLib, Version = 6.0.0.0, Culture = neutral, PublicKeyToken = 7cec85d7bea7798e";
    private const string TestPath = "Common.Tests.Reflection.ReflectionSerializationTests";
    public class A
    {
        public void VoidMethod() { }
        public int IntMethod(int a) { return a; }
        public int IntProperty { get; set; }
        public int IntFieldInstance;
        public static readonly int IntFieldStatic;
    }

    public class B<T>
    {
        public T Generic(T input) => input;
        public T2 Generic2<T2>(T2 input) => input;
        public List<T2> Generic3<T2>(List<T2> input) => input;
        public ValueTuple<IntPtr, IntPtr> Generic4(ValueTuple<IntPtr, IntPtr> input) => input;
        public ValueTuple<IntPtr, IntPtr> Generic5(ValueTuple<IntPtr, IntPtr> input, ValueTuple<IntPtr, IntPtr> input2) => 2 == 1 + 1 ? input : input2;
        public void Generic6<T2>(int index, T2 value) => Array.Empty<T2>()[index] = value;
        public Converter<string, T2> Generic7<T2>(Converter<string, T2> input) => input;
        public T2 Generic8<T2>(int _, T2 input) => input;
    }

    public class C { }

    public class D<T1, T2, T3> { }

    [Fact]
    public void ClassifyNormalType() => ReflectionSerialization.Classify(typeof(int)).Should().Be(TypeVariant.Normal);

    [Fact]
    public void ClassifyUnboundGenericType() => ReflectionSerialization.Classify(typeof(B<A>).GetMethod(nameof(B<A>.Generic2)).GetParameters().First().ParameterType).Should().Be(TypeVariant.GenericUnbound);

    [Fact]
    public void ClassifyBoundGenericType() => ReflectionSerialization.Classify(
        typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!.MakeGenericMethod(new Type[] { typeof(int) }).GetParameters().First().ParameterType,
        typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!.MakeGenericMethod(new Type[] { typeof(int) }).GetGenericMethodDefinition().GetParameters().First()).Should().Be(TypeVariant.GenericBound);

    [Fact]
    public void ClassifyNormalTypeInGenericMethod() => ReflectionSerialization.Classify(
        typeof(B<A>).GetMethod(nameof(B<A>.Generic8))!.MakeGenericMethod(new Type[] { typeof(int) }).GetParameters().First().ParameterType,
        typeof(B<A>).GetMethod(nameof(B<A>.Generic8))!.MakeGenericMethod(new Type[] { typeof(int) }).GetGenericMethodDefinition().GetParameters().First()).Should().Be(TypeVariant.Normal);

    [Fact]
    public void ClassifyBoundGenericTypeSkippingNormalParam() => ReflectionSerialization.Classify(
        typeof(B<A>).GetMethod(nameof(B<A>.Generic8))!.MakeGenericMethod(new Type[] { typeof(int) }).GetParameters().Skip(1).First().ParameterType,
        typeof(B<A>).GetMethod(nameof(B<A>.Generic8))!.MakeGenericMethod(new Type[] { typeof(int) }).GetGenericMethodDefinition().GetParameters().Skip(1).First()).Should().Be(TypeVariant.GenericBound);

    [Fact]
    public void ClassifyInnerBoundGenericParamFullParamParentOfGenericBound() => ReflectionSerialization.Classify(
        typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.MakeGenericMethod(new Type[] { typeof(int) }).GetParameters().First().ParameterType,
        typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.MakeGenericMethod(new Type[] { typeof(int) }).GetGenericMethodDefinition().GetParameters().First()).Should().Be(TypeVariant.ParentOfGenericBound);

    [Fact]
    public void ClassifyInnerBoundGenericParamInnerNormal() => ReflectionSerialization.Classify(
        typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.MakeGenericMethod(new Type[] { typeof(int) }).GetParameters().First().ParameterType.GetGenericArguments().First(),
        genericParameterType: typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.MakeGenericMethod(new Type[] { typeof(int) }).GetGenericMethodDefinition().GetParameters().First().ParameterType.GetGenericArguments().First()).Should().Be(TypeVariant.Normal);

    [Fact]
    public void ClassifyInnerBoundGenericParamInnerGeneric() => ReflectionSerialization.Classify(
        typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.MakeGenericMethod(new Type[] { typeof(int) }).GetParameters().First().ParameterType.GetGenericArguments().Skip(1).First(),
        genericParameterType: typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.MakeGenericMethod(new Type[] { typeof(int) }).GetGenericMethodDefinition().GetParameters().First().ParameterType.GetGenericArguments().Skip(1).First()).Should().Be(TypeVariant.GenericBound);

    [Fact]
    public void ClassifyInnerUnboundGenericParamFullParamParentOfGenericUnbound() => ReflectionSerialization.Classify(
        typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.GetParameters().First().ParameterType,
        typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.GetGenericMethodDefinition().GetParameters().First()).Should().Be(TypeVariant.ParentOfGenericUnbound);

    [Fact]
    public void ClassifyInnerUnboundGenericParamInnerNormal() => ReflectionSerialization.Classify(
        typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.GetParameters().First().ParameterType.GetGenericArguments().First(),
        genericParameterType: typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.GetGenericMethodDefinition().GetParameters().First().ParameterType.GetGenericArguments().First()).Should().Be(TypeVariant.Normal);

    [Fact]
    public void ClassifyInnerUnboundGenericParamInnerGeneric() => ReflectionSerialization.Classify(
        typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.GetParameters().First().ParameterType.GetGenericArguments().Skip(1).First(),
        genericParameterType: typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!.GetGenericMethodDefinition().GetParameters().First().ParameterType.GetGenericArguments().Skip(1).First()).Should().Be(TypeVariant.GenericUnbound);

    [Fact]
    public void ClassifyBoundToGenericTypeOuterBound() => ReflectionSerialization.Classify(
        typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!.MakeGenericMethod(new Type[] { typeof(List<int>) })!.GetParameters().First().ParameterType,
        typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!.GetGenericMethodDefinition().GetParameters().First()).Should().Be(TypeVariant.GenericBound);

    [Fact]
    public void ClassifyBoundToGenericTypeInnerBound() => ReflectionSerialization.Classify(
        typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!.MakeGenericMethod(new Type[] { typeof(List<int>) })!.GetParameters().First().ParameterType.GetGenericArguments().First(),
        genericParameter: typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!.GetGenericMethodDefinition().GetParameters().First()).Should().Be(TypeVariant.GenericBound);

    [Fact]
    public void SerializeMethod()
    {
        var method = typeof(A).GetMethod(nameof(A.IntMethod));
        var serialized = ReflectionSerialization.SerializeMethod(method!);
        Compare($"{TestPath}+A, {CommonLib}:IntMethod(System.Int32, {SystemLib})", serialized);
    }

    [Fact]
    public void SerializeProperty()
    {
        var property = typeof(A).GetProperty(nameof(A.IntProperty));
        var serialized = ReflectionSerialization.SerializeProperty(property!);
        Compare($"{TestPath}+A, {CommonLib}:IntProperty", serialized);
    }

    [Fact]
    public void SerializeInstanceField()
    {
        var field = typeof(A).GetField(nameof(A.IntFieldInstance));
        var serialized = ReflectionSerialization.SerializeField(field!);
        Compare($"{TestPath}+A, {CommonLib}:IntFieldInstance", serialized);
    }

    [Fact]
    public void SerializeStaticField()
    {
        var field = typeof(A).GetField(nameof(A.IntFieldStatic));
        var serialized = ReflectionSerialization.SerializeField(field!);
        Compare($"{TestPath}+A, {CommonLib}:IntFieldStatic", serialized);
    }

    [Fact]
    public void SerializeStringMethodReference()
    {
        var serialized = ReflectionSerialization.SerializeMethod<A, int>(nameof(A.IntMethod));
        Compare($"{TestPath}+A, {CommonLib}:IntMethod(System.Int32, {SystemLib})", serialized);
    }

    [Fact]
    public void SerializeGenericMethodReferenceBound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!.MakeGenericMethod(new Type[] { typeof(int) });
        var serialized = ReflectionSerialization.SerializeMethod(method);
        Compare($"{TestPath}+B`1[[/{TestPath}+A, {CommonLib}]], {CommonLib}:Generic2(/System.Int32, {SystemLib})", serialized);
    }

    [Fact]
    public void DeserializeGenericMethodReferenceBound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!.MakeGenericMethod(new Type[] { typeof(int) });
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"{TestPath}+B`1[[{TestPath}+A, {CommonLib}]], {CommonLib}:Generic2(/System.Int32)");
        Assert.NotNull(deserialized);
    }

    [Fact]
    public void SerializeGenericListMethodReferenceBound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic3))!.MakeGenericMethod(new Type[] { typeof(List<int>) });
        var serialized = ReflectionSerialization.SerializeMethod(method);
        Compare($"{TestPath}+B`1[[/{TestPath}+A, {CommonLib}]], {CommonLib}:Generic3(System.Collections.Generic.List`1[[/System.Collections.Generic.List`1[[System.Int32, {SystemLib}]], {SystemLib}]], {SystemLib})", serialized);
    }

    [Fact]
    public void DeserializeGenericListMethodReferenceBound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic3))!.MakeGenericMethod(new Type[] { typeof(List<int>) });
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"{TestPath}+B`1[[/{TestPath}+A, {CommonLib}]], {CommonLib}:Generic3(System.Collections.Generic.List`1[[/System.Collections.Generic.List`1[[System.Int32, {SystemLib}]], {SystemLib}]], {SystemLib})");
        Assert.NotNull(deserialized);
    }

    [Fact]
    public void SerializeGenericMethodReferenceUnbound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!;
        var serialized = ReflectionSerialization.SerializeMethod(method);
        Compare($"{TestPath}+B`1[[/{TestPath}+A, {CommonLib}]], {CommonLib}:Generic2(-T2, {CommonLib})", serialized);
    }

    [Fact]
    public void DeserializeGenericMethodReferenceUnbound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!;
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"{TestPath}+B`1[[{TestPath}+A, {CommonLib}]], {CommonLib}:Generic2(-)");
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void DeserializeMethodWithGenericParameterReference()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic4))!;
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"{TestPath}+B`1[[{TestPath}+A, {CommonLib}]], {CommonLib}:Generic4(System.ValueTuple`2[[System.IntPtr, {SystemLib}],[System.IntPtr, {SystemLib}]], {SystemLib})");
        Assert.NotNull(deserialized);
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void DeserializeMethodWithDoubleGenericParameterReference()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic5))!;
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"{TestPath}+B`1[[{TestPath}+A, {CommonLib}]], {CommonLib}:Generic5(System.ValueTuple`2[[System.IntPtr, {SystemLib}],[System.IntPtr, {SystemLib}]], {SystemLib}_;_System.ValueTuple`2[[System.IntPtr, {SystemLib}],[System.IntPtr, {SystemLib}]], {SystemLib})");
        Assert.NotNull(deserialized);
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void SerializeMixedGenericMethod()
    {
        var serialized = ReflectionSerialization.SerializeMethod(typeof(B<A>).GetMethod(nameof(B<A>.Generic6))!);
        Compare($"{TestPath}+B`1[[/{TestPath}+A, {CommonLib}]], {CommonLib}:Generic6(System.Int32, {SystemLib}_;_-T2, {CommonLib})", serialized);
    }

    [Fact]
    public void DeserializeMixedGenericMethod()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic6))!;
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"{TestPath}+B`1[[/{TestPath}+A, {CommonLib}]], {CommonLib}:Generic6(System.Int32, {SystemLib}_;_-T, {CommonLib})");
        Assert.NotNull(deserialized);
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void SerializeInnerGenericMethod()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!;
        var reflectedType = method.ReflectedType;
        var qualifiedName = reflectedType.AssemblyQualifiedName;
        var names = method.GetGenericArguments().Select(x => x.Name).ToArray();
        var serialized = ReflectionSerialization.SerializeMethod(method);
        Compare($"{TestPath}+B`1[[/{TestPath}+A, {CommonLib}]], {CommonLib}:Generic7(System.Converter`2[[/System.String, {SystemLib}],[-T2, {CommonLib}]], {SystemLib})", serialized);
    }

    [Fact]
    public void DeserializeInnerGenericMethod()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!;
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"{TestPath}+B`1[[/{TestPath}+A, {CommonLib}]], {CommonLib}:Generic7(System.Converter`2[[System.String, {SystemLib}],[-T2, {CommonLib}]], {SystemLib})");
        Assert.NotNull(deserialized);
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void SerializeLambdaMethodReference()
    {
        var serialized = ReflectionSerialization.SerializeMethod<A, int>(x => x.IntMethod(0));
        Compare($"{TestPath}+A, {CommonLib}:IntMethod(System.Int32, {SystemLib})", serialized);
    }

    [Fact]
    public void DeserializeMethod()
    {
        var method = typeof(A).GetMethod(nameof(A.IntMethod));
        var deserialized = ReflectionSerialization.DeserializeMethod<A>($"{TestPath}+A, {CommonLib}:IntMethod(System.Int32, {SystemLib})");
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void DeserializeJoinMethod()
    {
        var method = typeof(string).GetMethod(nameof(string.Join), new[] { typeof(string[]) });
        var deserialized = ReflectionSerialization.DeserializeMethod<A>($"System.String, {SystemLibSpaced}: Join(System.CharSystem.String[])");
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void DeserializeProperty()
    {
        var property = typeof(A).GetProperty(nameof(A.IntProperty));
        var deserialized = ReflectionSerialization.DeserializeProperty($"{TestPath}+A, {CommonLib}:IntProperty");
        Assert.Equal(property, deserialized);
    }

    [Fact]
    public void DeserializeInstanceField()
    {
        var field = typeof(A).GetField(nameof(A.IntFieldInstance));
        var deserialized = ReflectionSerialization.DeserializeField($"{TestPath}+A, {CommonLib}:IntFieldInstance");
        Assert.Equal(field, deserialized);
    }

    [Fact]
    public void DeserializeStaticField()
    {
        var field = typeof(A).GetField(nameof(A.IntFieldStatic));
        var deserialized = ReflectionSerialization.DeserializeField($"{TestPath}+A, {CommonLib}:IntFieldStatic");
        Assert.Equal(field, deserialized);
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(char))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(int[][]))]
    [InlineData(typeof(List<int>))]
    [InlineData(typeof(List<List<int>>))]
    [InlineData(typeof(ValueTuple<IntPtr, IntPtr>))]
    [InlineData(typeof(List<Converter<string, string>>))]
    public void SerializeDeserializeTypeQuickTest(Type type) => type.Should().Be(ReflectionSerialization.DeserializeType(ReflectionSerialization.SerializeType(type)));

    [Theory]
    [InlineData($"System.Int32, {SystemLib}", typeof(int))]
    [InlineData($"System.DateTime, {SystemLib}", typeof(DateTime))]
    [InlineData($"{TestPath}+A, {CommonLib}", typeof(A))]
    [InlineData($"{TestPath}, {CommonLib}", typeof(ReflectionSerializationTests))]
    [InlineData($"System.ValueTuple`2[[/System.IntPtr, {SystemLib}],[/System.IntPtr, {SystemLib}]], {SystemLib}", typeof(ValueTuple<IntPtr, IntPtr>))]
    public void SerializeType(string name, Type type) => Compare(name, ReflectionSerialization.SerializeType(type));

    [Theory]
    [InlineData("System.String", typeof(string))]
    [InlineData($"System.String, {SystemLib}", typeof(string))]
    [InlineData("System.Int32", typeof(int))]
    [InlineData("System.DateTime", typeof(DateTime))]
    [InlineData($"{TestPath}+A", typeof(A))]
    [InlineData($"{TestPath}", typeof(ReflectionSerializationTests))]
    [InlineData("System.ValueTuple`2[System.IntPtr,System.IntPtr]", typeof(ValueTuple<IntPtr, IntPtr>))]
    [InlineData($"System.ValueTuple`2[[System.IntPtr, {SystemLib}],[System.IntPtr, {SystemLib}]]", typeof(ValueTuple<IntPtr, IntPtr>))]
    public void DeserializeType(string name, Type type) => Assert.Equal(type, ReflectionSerialization.DeserializeType(name));

    [Fact]
    public void SerializeArrayType() => Compare($"System.Int32[], {SystemLib}", ReflectionSerialization.SerializeType(typeof(int[])));

    [Fact]
    public void DeserializeArrayType() => Assert.Equal(typeof(int[]), ReflectionSerialization.DeserializeType($"System.Int32[], {SystemLib}"));

    [Fact]
    public void SerializeArrayArrayType() => Compare($"System.Int32[][], {SystemLib}", ReflectionSerialization.SerializeType(typeof(int[][])));

    [Fact]
    public void DeserializeArrayArrayType() => Assert.Equal(typeof(int[][]), ReflectionSerialization.DeserializeType($"System.Int32[][], {SystemLib}"));

    [Fact]
    public void SerializeListType() => Compare($"System.Collections.Generic.List`1[[/System.Int32, {SystemLib}]], {SystemLib}",
            ReflectionSerialization.SerializeType(typeof(List<int>)));

    [Fact]
    public void DeserializeListType() => Assert.Equal(typeof(List<int>),
            ReflectionSerialization.DeserializeType($"System.Collections.Generic.List`1[[/System.Int32, {SystemLib}]], {SystemLib}"));

    [Fact]
    public void SerializeTypeFromGenericReturn() => Compare($"System.Int32, {SystemLib}",
            ReflectionSerialization.SerializeType(typeof(B<int>).GetMethod(nameof(B<int>.Generic))!.ReturnType));

    [Fact]
    public void SerializeGenericType()
    {
        var BA_C = ReflectionUtil.GetOrMakeGenericMethod<B<A>, C>(nameof(B<A>.Generic2))!.ReturnType;
        Compare($"{TestPath}+C, {CommonLib}", ReflectionSerialization.SerializeType(BA_C));
    }

    [Fact]
    public void DeserializeGenericType()
    {
        var deserialized = ReflectionSerialization.DeserializeType($"{TestPath}+C, {CommonLib}");
        var BA_C = ReflectionUtil.GetOrMakeGenericMethod<B<A>, C>(nameof(B<A>.Generic2))!.ReturnType;
        Assert.Equal(BA_C, deserialized);
    }

    [Fact]
    public void DeserializeGenericTypeFromGenericName()
    {
        var deserialized = ReflectionSerialization.DeserializeType("System.Collections.Generic.List`1[System.Int32]");
        var listType = typeof(List<int>);
        Assert.Equal(listType, deserialized);
    }

    [Fact]
    public void DeserializeGenericTypeFromMultiGenericName()
    {
        var deserialized = ReflectionSerialization.DeserializeType($"{TestPath}+D`3[[{TestPath}+A, {CommonLib}],[{TestPath}+B`1[[System.Int32, {SystemLib}]], {CommonLib}],[{TestPath}+D`3[[{TestPath}+C, {CommonLib}],[{TestPath}+C, {CommonLib}],[{TestPath}+C, {CommonLib}]], {CommonLib}]]");
        var D_AB_int_DCCCC = typeof(D<A, B<int>, D<C, C, C>>);
        Assert.Equal(D_AB_int_DCCCC, deserialized);
    }

    [Fact]
    public void DeserializeGenericTypeFromSymbolName()
    {
        var deserialized = ReflectionSerialization.DeserializeType("System.Collections.Generic.List<int>");
        var listType = typeof(List<int>);
        Assert.Equal(listType, deserialized);
    }

    [Fact]
    public void SimplifyGenericName()
    {
        var listType = typeof(List<IDictionary<int, string>>);
        Assert.Equal("List<IDictionary<int,string>>", ReflectionSerialization.SimplifyGenericName(listType.FullName));
    }

    private static void Compare(string s1, string s2) => Clean(s1).Should().Be(Clean(s2));
    private static string Clean(string s) => s.Replace(SystemLib, "{SystemLib}").Replace(CommonLib, "{CommonLib}").Replace(TestPath, "{TestPath}");
}
