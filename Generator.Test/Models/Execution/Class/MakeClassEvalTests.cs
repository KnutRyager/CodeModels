using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.Class.Test;

public class MakeClassEvalTests
{
    [Fact] public void MakeStaticClassWithField() => "public static class A { public static int B = 1337; } A.B;".Eval().Should().Be(1337);
    [Fact] public void MakeStaticClassWithProperty() => "public static class A { public static int B => 1337; } A.B;".Eval().Should().Be(1337);
    [Fact] public void MakeStaticClassWithMethod() => "public static class A { public static int B() => 1337; } A.B();".Eval().Should().Be(1337);
    [Fact] public void MakeInstanceClassWithField() => "public class A { public int B = 1337; } new A().B;".Eval().Should().Be(1337);
    [Fact] public void MakeInstanceClassWithProperty() => "public class A { public int B => 1337; } new A().B;".Eval().Should().Be(1337);
    [Fact] public void MakeInstanceClassWithMethod() => "public class A { public int B() => 1337; } new A().B();".Eval().Should().Be(1337);
}
