using Common.Reflection;
using FluentAssertions;
using Xunit;

namespace Common.Tests.Reflection;

public class ReflectionSerializationTests
{
    private const string MsCorLib = "";
    private const string CommonLib = "Common.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
    private const string SystemLib = "System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e";
    private const string SystemLibSpaced = "System.Private.CoreLib, Version = 6.0.0.0, Culture = neutral, PublicKeyToken = 7cec85d7bea7798e";
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
    }

    public class C { }

    public class D<T1, T2, T3> { }

    [Fact]
    public void SerializeMethod()
    {
        var method = typeof(A).GetMethod(nameof(A.IntMethod));
        var serialized = ReflectionSerialization.SerializeMethod(method!);
        $"Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}:IntMethod(System.Int32)".Should().Be(serialized);
    }

    [Fact]
    public void SerializeProperty()
    {
        var property = typeof(A).GetProperty(nameof(A.IntProperty));
        var serialized = ReflectionSerialization.SerializeProperty(property!);
        $"Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}:IntProperty".Should().Be(serialized);
    }

    [Fact]
    public void SerializeInstanceField()
    {
        var field = typeof(A).GetField(nameof(A.IntFieldInstance));
        var serialized = ReflectionSerialization.SerializeField(field!);
        $"Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}:IntFieldInstance".Should().Be(serialized);
    }

    [Fact]
    public void SerializeStaticField()
    {
        var field = typeof(A).GetField(nameof(A.IntFieldStatic));
        var serialized = ReflectionSerialization.SerializeField(field!);
        $"Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}:IntFieldStatic".Should().Be(serialized);
    }

    [Fact]
    public void SerializeStringMethodReference()
    {
        var serialized = ReflectionSerialization.SerializeMethod<A, int>(nameof(A.IntMethod));
        $"Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}:IntMethod(System.Int32)".Should().Be(serialized);
    }

    [Fact]
    public void SerializeGenericMethodReferenceBound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!.MakeGenericMethod(new Type[] { typeof(int) });
        var serialized = ReflectionSerialization.SerializeMethod(method);
        Assert.Equal($"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic2(-System.Int32)", serialized);
    }

    [Fact]
    public void DeserializeGenericMethodReferenceBound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!.MakeGenericMethod(new Type[] { typeof(int) });
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic2(-System.Int32)");
        Assert.NotNull(deserialized);
        //Assert.Equal(method, deserialized);
    }

    [Fact]
    public void SerializeGenericListMethodReferenceBound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic3))!.MakeGenericMethod(new Type[] { typeof(List<int>) });
        var serialized = ReflectionSerialization.SerializeMethod(method);
        Assert.Equal($"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic3(System.Collections.Generic.List`1[System.Collections.Generic.List`1[System.Int32]])", serialized);
    }

    [Fact]
    public void DeserializeGenericListMethodReferenceBound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic3))!.MakeGenericMethod(new Type[] { typeof(List<int>) });
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic3(-System.Int32)");
        Assert.NotNull(deserialized);
    }

    [Fact]
    public void SerializeGenericMethodReferenceUnbound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!;
        var serialized = ReflectionSerialization.SerializeMethod(method);
        Assert.Equal($"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic2(-T2)", serialized);
    }

    [Fact]
    public void DeserializeGenericMethodReferenceUnbound()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic2))!;
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic2(-)");
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void DeserializeMethodWithGenericParameterReference()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic4))!;
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic4(System.ValueTuple`2[[System.IntPtr, {SystemLib}],[System.IntPtr, {SystemLib}]], {SystemLib})");
        Assert.NotNull(deserialized);
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void DeserializeMethodWithDoubleGenericParameterReference()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic5))!;
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic5(System.ValueTuple`2[[System.IntPtr, {SystemLib}],[System.IntPtr, {SystemLib}]], {SystemLib}_;_System.ValueTuple`2[[System.IntPtr, {SystemLib}],[System.IntPtr, {SystemLib}]], {SystemLib})");
        Assert.NotNull(deserialized);
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void SerializeMixedGenericMethod()
    {
        var serialized = ReflectionSerialization.SerializeMethod(typeof(B<A>).GetMethod(nameof(B<A>.Generic6))!);
        Assert.Equal($"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic6(System.Int32_;_-T2)", serialized);
    }

    [Fact]
    public void DeserializeMixedGenericMethod()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic6))!;
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic6(System.Int32_;_-T)");
        Assert.NotNull(deserialized);
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void SerializeInnerGenericMethod()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!;
        var serialized = ReflectionSerialization.SerializeMethod(method);
        $"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic7(System.Converter`2[System.String,-T2])".Should().Be(serialized);
    }

    [Fact]
    public void DeserializeInnerGenericMethod()
    {
        var method = typeof(B<A>).GetMethod(nameof(B<A>.Generic7))!;
        var deserialized = ReflectionSerialization.DeserializeMethod<B<A>>($"Common.Tests.Reflection.ReflectionSerializationTests+B`1[[Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}]], {CommonLib}:Generic7(System.Converter`2[System.String,-T2])");
        Assert.NotNull(deserialized);
        Assert.Equal(method, deserialized);
    }

    [Fact]
    public void SerializeLambdaMethodReference()
    {
        var serialized = ReflectionSerialization.SerializeMethod<A, int>(x => x.IntMethod(0));
        Assert.Equal($"Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}:IntMethod(System.Int32)", serialized);
    }

    [Fact]
    public void DeserializeMethod()
    {
        var method = typeof(A).GetMethod(nameof(A.IntMethod));
        var deserialized = ReflectionSerialization.DeserializeMethod<A>($"Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}:IntMethod(System.Int32)");
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
        var deserialized = ReflectionSerialization.DeserializeProperty($"Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}:IntProperty");
        Assert.Equal(property, deserialized);
    }

    [Fact]
    public void DeserializeInstanceField()
    {
        var field = typeof(A).GetField(nameof(A.IntFieldInstance));
        var deserialized = ReflectionSerialization.DeserializeField($"Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}:IntFieldInstance");
        field.Should().Be(deserialized);
    }

    [Fact]
    public void DeserializeStaticField()
    {
        var field = typeof(A).GetField(nameof(A.IntFieldStatic));
        var deserialized = ReflectionSerialization.DeserializeField($"Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}:IntFieldStatic");
        field.Should().Be(deserialized);
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
    [InlineData($"Common.Tests.Reflection.ReflectionSerializationTests+A, {CommonLib}", typeof(A))]
    [InlineData($"Common.Tests.Reflection.ReflectionSerializationTests, {CommonLib}", typeof(ReflectionSerializationTests))]
    [InlineData($"System.ValueTuple`2[[System.IntPtr, {SystemLib}],[System.IntPtr, {SystemLib}]], {SystemLib}", typeof(ValueTuple<IntPtr, IntPtr>))]
    public void SerializeType(string name, Type type) => name.Should().Be(ReflectionSerialization.SerializeType(type));

    [Theory]
    [InlineData("System.String", typeof(string))]
    [InlineData($"System.String, {SystemLib}", typeof(string))]
    [InlineData("System.Int32", typeof(int))]
    [InlineData("System.DateTime", typeof(DateTime))]
    [InlineData("Common.Tests.Reflection.ReflectionSerializationTests+A", typeof(A))]
    [InlineData("Common.Tests.Reflection.ReflectionSerializationTests", typeof(ReflectionSerializationTests))]
    [InlineData("System.ValueTuple`2[System.IntPtr,System.IntPtr]", typeof(ValueTuple<IntPtr, IntPtr>))]
    [InlineData($"System.ValueTuple`2[[System.IntPtr, {SystemLib}],[System.IntPtr, {SystemLib}]]", typeof(ValueTuple<IntPtr, IntPtr>))]
    public void DeserializeType(string name, Type type) => type.Should().Be(ReflectionSerialization.DeserializeType(name));

    [Fact]
    public void SerializeArrayType() => $"System.Int32[], {SystemLib}".Should().Be(ReflectionSerialization.SerializeType(typeof(int[])));

    [Fact]
    public void DeserializeArrayType() => typeof(int[]).Should().Be(ReflectionSerialization.DeserializeType($"System.Int32[], {SystemLib}"));

    [Fact]
    public void SerializeArrayArrayType() => $"System.Int32[][], {SystemLib}".Should().Be(ReflectionSerialization.SerializeType(typeof(int[][])));

    [Fact]
    public void DeserializeArrayArrayType() => typeof(int[][]).Should().Be(ReflectionSerialization.DeserializeType($"System.Int32[][], {SystemLib}"));

    [Fact]
    public void SerializeListType() => $"System.Collections.Generic.List`1[[System.Int32, {SystemLib}]], {SystemLib}".Should().Be(
            ReflectionSerialization.SerializeType(typeof(List<int>)));

    [Fact]
    public void DeserializeListType() => typeof(List<int>).Should().Be(
            ReflectionSerialization.DeserializeType($"System.Collections.Generic.List`1[[System.Int32, {SystemLib}]], {SystemLib}"));

    [Fact]
    public void SerializeTypeFromGenericReturn() => $"System.Int32, {SystemLib}".Should().Be(
            ReflectionSerialization.SerializeType(typeof(B<int>).GetMethod(nameof(B<int>.Generic))!.ReturnType));

    [Fact]
    public void SerializeGenericType()
    {
        var BA_C = ReflectionUtil.GetOrMakeGenericMethod<B<A>, C>(nameof(B<A>.Generic2))!.ReturnType;
        $"Common.Tests.Reflection.ReflectionSerializationTests+C, {CommonLib}".Should().Be(ReflectionSerialization.SerializeType(BA_C));
    }

    [Fact]
    public void DeserializeGenericType()
    {
        var deserialized = ReflectionSerialization.DeserializeType($"Common.Tests.Reflection.ReflectionSerializationTests+C, {CommonLib}");
        var BA_C = ReflectionUtil.GetOrMakeGenericMethod<B<A>, C>(nameof(B<A>.Generic2))!.ReturnType;
        BA_C.Should().Be(deserialized);
    }

    [Fact]
    public void DeserializeGenericTypeFromGenericName()
    {
        var deserialized = ReflectionSerialization.DeserializeType("System.Collections.Generic.List`1[System.Int32]");
        var listType = typeof(List<int>);
        listType.Should().Be(deserialized);
    }

    [Fact]
    public void DeserializeGenericTypeFromMultiGenericName()
    {
        var deserialized = ReflectionSerialization.DeserializeType("Common.Tests.Reflection.ReflectionSerializationTests+D`3[[Common.Tests.Reflection.ReflectionSerializationTests+A, Common.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[Common.Tests.Reflection.ReflectionSerializationTests+B`1[[System.Int32, System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], Common.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[Common.Tests.Reflection.ReflectionSerializationTests+D`3[[Common.Tests.Reflection.ReflectionSerializationTests+C, Common.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[Common.Tests.Reflection.ReflectionSerializationTests+C, Common.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[Common.Tests.Reflection.ReflectionSerializationTests+C, Common.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Common.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]");
        var D_AB_int_DCCCC = typeof(D<A, B<int>, D<C, C, C>>);
        D_AB_int_DCCCC.Should().Be(deserialized);
    }

    [Fact]
    public void DeserializeGenericTypeFromSymbolName()
    {
        var deserialized = ReflectionSerialization.DeserializeType("System.Collections.Generic.List<int>");
        var listType = typeof(List<int>);
        listType.Should().Be(deserialized);
    }
}
