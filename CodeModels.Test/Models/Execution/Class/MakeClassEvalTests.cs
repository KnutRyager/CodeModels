using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.Class.Test;

public class MakeClassEvalTests
{
    [Fact]
    public void MakeStaticClassGetField()
        => "public static class A { public static int B = 1337; } A.B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeStaticClassSetField()
        => "public static class A { public static int B = 22; } A.B = 1337; A.B".Eval().Should().Be(1337);
    [Fact]
    public void MakeStaticClassGetPropertyDefault()
        => "public static class A { public static int B { get; set; } = 1337; } A.B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeStaticClassSetPropertyDefault()
       => "public static class A { public static int B { get; set; } = 22; } A.B = 1337; A.B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeStaticClassGetPropertyGetter()
        => "public static class A { public static int B { get => 1337; } } A.B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeStaticClassGetPropertyArrow()
        => "public static class A { public static int B => 1337; } A.B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeStaticClassGetMethodBody()
        => "public static class A { public static int B() { return 1337; } } A.B();".Eval().Should().Be(1337);
    [Fact]
    public void MakeStaticClassGetMethodExpression()
        => "public static class A { public static int B() => 1337; } A.B();".Eval().Should().Be(1337);
    [Fact]
    public void MakeStaticClassGetMethodReturnMember()
        => "public static class A { private static int a = 1337; public static int B() => a; } A.B();".Eval().Should().Be(1337);
    [Fact]
    public void MakeStaticClassGetMethodReturnParameter()
        => "public static class A { public static int B(int a) => a; } A.B(1337);".Eval().Should().Be(1337);
    [Fact]
    public void MakeStaticClassGetMethodReturnParameterDefault()
        => "public static class A { public static int B(int a = 1337) => a; } A.B();".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassGetField()
        => "public class A { public int B = 1337; } new A().B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassSetField()
        => "public class A { public int B = 22; } var a = new A(); a.B = 1337; a.B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassGetPropertyDefault()
        => "public class A { public int B { get; set; } = 1337; } new A().B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassSetPropertyDefault()
        => "public class A { public int B { get; set; } = 22; } var a = new A(); a.B = 1337; a.B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassGetPropertyGetter()
        => "public class A { public int B { get => 1337; } } new A().B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassGetPropertyArrow()
        => "public class A { public int B => 1337; } new A().B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassGetMethodBody()
        => "public class A { public int B() { return 1337; } } new A().B();".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassGetMethodExpression()
        => "public class A { public int B() => 1337; } new A().B();".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassSetConstructorFieldDefault()
        => "public class A { public A() { B = 1337; } public int B = 22; } new A().B; var a2 = new A().B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassSetConstructorFieldParameter()
        => "public class A { public A(int a) { B = a; } public int B = 22; } new A(1337).B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassSetConstructorFieldParameterDefault()
        => "public class A { public A(int a = 1337) { B = a; } public int B = 22; } new A().B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassSetConstructorProperyDefault()
        => "public class A { public A() { B = 1337; } public int B { get; } = 22; } new A().B; var a2 = new A().B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassSetConstructorPropertyParameter()
        => "public class A { public A(int a) { B = a; } public int B { get; } = 22; } new A(1337).B;".Eval().Should().Be(1337);
    [Fact]
    public void MakeInstanceClassSetConstructorPropertyParameterDefault()
        => "public class A { public A(int a = 1337) { B = a; } public int B { get; } = 22; } new A().B;".Eval().Should().Be(1337);
    [Fact(Skip ="Hard")]
    public void MakeInstanceClassRecursiveProperties()
        => "public class A { public int B { get; } = 1337; public int C => B; public int D => C; } new A().D;".Eval().Should().Be(1337);
}