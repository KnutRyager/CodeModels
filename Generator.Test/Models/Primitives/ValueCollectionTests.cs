using CodeAnalyzation.Models;
using FluentAssertions;
using Xunit;
using static Generator.Test.TestUtil;

namespace Generator.Test.Models.Primitives;

public class ValueCollectionTests
{
    [Fact]
    public void GenerateEnum() => new ValueCollection("Abc,Def,Ghi").ToEnum("MyEnum").CodeEqual(@"
public enum MyEnum {
    Abc, Def, Ghi
}");

    [Fact] public void BaseTypeString() => new ValueCollection("Abc,Def,Ghi").BaseType().CodeEqual("string");
    [Fact] public void BaseTypeInt() => new ValueCollection(new Value[] { Value.FromValue(1), Value.FromValue(2) }).BaseType().CodeEqual("int");
    [Fact] public void BaseTypeObject() => new ValueCollection(new Value[] { Value.FromValue("a"), Value.FromValue(2) }).BaseType().CodeEqual("object");

    [Fact]
    public void GenerateArrayInitialization() => new ValueCollection("Abc,Def,Ghi").ToArrayInitialization().CodeEqual(@"new string[]{""Abc"",""Def"",""Ghi""}");

    [Fact]
    public void GenerateListInitialization() => new ValueCollection("Abc,Def,Ghi").ToListInitialization().CodeEqual(@"new List<string>{""Abc"",""Def"",""Ghi""}");

    [Fact]
    public void GenerateKeyValueInitialization() => new ValueCollectionWithKey(Value.FromValue(1), "Abc,Def,Ghi").ToKeyValueInitialization().CodeEqual(@"{1, new string[]{""Abc"",""Def"",""Ghi""} }", ignoreWhitespace: true);

    [Fact]
    public void GenerateDictionaryInitialization() => new ValueDictionary(new ValueCollectionWithKey[]{
        new (Value.FromValue(1), "Abc,Def,Ghi"),
        new (Value.FromValue(2), "Jkl,Mno,Pqr")})
        .ToDictionary().CodeEqual(@"new Dictionary<int, string[]>(){ 
    {1, new string[]{""Abc"",""Def"",""Ghi""} },
    {2, new string[]{""Jkl"",""Mno"",""Pqr""} }
}", ignoreWhitespace: true);

    [Fact]
    public void ParseValueCollectionFromEnum() => new ValueCollection(@"
public enum MyEnum {
    Abc, Def, Ghi
}").Should().Equals(new ValueCollection("Abc,Def,Ghi"));
}
