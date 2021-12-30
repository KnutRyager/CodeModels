using CodeAnalyzation.Models;
using Xunit;

namespace Generator.Test.Models.Primitives;

public class EnumTests
{
    [Fact]
    public void GenerateEnum() => new EnumModel("MyEnum", new[] { "Abc", "Def", "Ghi" }).ToEnum().CodeEqual(@"
public enum MyEnum {
    Abc, Def, Ghi
}");

    [Fact]
    public void GenerateEnumWithNone() => new EnumModel("MyEnum", new[] { "Abc", "Def", "Ghi" }, hasNoneValue: true).ToEnum().CodeEqual(@"
public enum MyEnum {
    None, Abc, Def, Ghi
}");

    [Fact]
    public void GenerateFlagsEnum() => new EnumModel("MyEnum", new[] { "A", "B", "C", "D", "E" }, isFlags: true).ToEnum().CodeEqual(@"
public enum MyEnum {
    A=1, B=2, C=4, D=8, E=16
}");

    [Fact]
    public void GenerateFlagsEnumWithNone() => new EnumModel("MyEnum", new[] { "A", "B", "C", "D", "E" }, isFlags: true, hasNoneValue: true).ToEnum().CodeEqual(@"
public enum MyEnum {
    None=0, A=1, B=2, C=4, D=8, E=16
}");

}
