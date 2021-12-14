using CodeAnalyzation.Models;
using FluentAssertions;
using Xunit;
using static CodeAnalysisTests.TestUtil;

namespace Generator.Test;

public class ValueCollectionTests
{
    [Fact]
    public void GenerateEnum() => new ValueCollection("Abc,Def,Ghi").ToEnum("MyEnum").CodeEqual(@"
public enum MyEnum {
    Abc, Def, Ghi
}");

    [Fact]
    public void ParseValueCollectionFromEnum() => new ValueCollection(@"
public enum MyEnum {
    Abc, Def, Ghi
}").Should().Equals(new ValueCollection("Abc,Def,Ghi"));
}
