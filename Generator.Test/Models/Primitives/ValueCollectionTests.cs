using CodeAnalyzation.Models;
using FluentAssertions;
using Xunit;
using static Generator.Test.TestUtil;

namespace Generator.Test.Models.Primitives;

public class ValueCollectionTests
{
    [Fact]
    public void GenerateEnum() => new ExpressionCollection("Abc,Def,Ghi").ToEnum("MyEnum").CodeEqual(@"
public enum MyEnum {
    Abc, Def, Ghi
}");

    [Fact] public void BaseTypeString() => new ExpressionCollection("Abc,Def,Ghi").BaseType().CodeEqual("string");
    [Fact] public void BaseTypeInt() => new ExpressionCollection(new Expression[] { new LiteralExpression(1), new LiteralExpression(2) }).BaseType().CodeEqual("int");
    [Fact] public void BaseTypeObject() => new ExpressionCollection(new Expression[] { new LiteralExpression("a"), new LiteralExpression(2) }).BaseType().CodeEqual("object");

    [Fact]
    public void GenerateArrayInitialization() => new ExpressionCollection("Abc,Def,Ghi").ToArrayInitialization().CodeEqual(@"new string[]{""Abc"",""Def"",""Ghi""}");

    [Fact]
    public void GenerateListInitialization() => new ExpressionCollection("Abc,Def,Ghi").ToListInitialization().CodeEqual(@"new List<string>{""Abc"",""Def"",""Ghi""}");

    [Fact]
    public void GenerateKeyValueInitialization() => new ExpressionCollectionWithKey(new LiteralExpression(1), "Abc,Def,Ghi").ToKeyValueInitialization().CodeEqual(@"{1, new string[]{""Abc"",""Def"",""Ghi""} }", ignoreWhitespace: true);

    [Fact]
    public void GenerateDictionaryInitialization() => new ExpressionDictionary(new ExpressionCollectionWithKey[]{
        new (new LiteralExpression(1), "Abc,Def,Ghi"),
        new (new LiteralExpression(2), "Jkl,Mno,Pqr")})
        .ToDictionary().CodeEqual(@"new Dictionary<int, string[]>(){ 
    {1, new string[]{""Abc"",""Def"",""Ghi""} },
    {2, new string[]{""Jkl"",""Mno"",""Pqr""} }
}", ignoreWhitespace: true);

    [Fact]
    public void ParseValueCollectionFromEnum() => new ExpressionCollection(@"
public enum MyEnum {
    Abc, Def, Ghi
}").Should().Equals(new ExpressionCollection("Abc,Def,Ghi"));
}
