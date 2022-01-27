using System.Linq;
using CodeAnalyzation.Parsing;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using static CodeAnalyzation.Test.TestUtil;

namespace CodeAnalyzation.Models.Primitives.Test;

public class ValueCollectionTests
{
    [Fact]
    public void GenerateEnum() => new ExpressionCollection("Abc,Def,Ghi").ToEnum("MyEnum").CodeEqual(@"
public enum MyEnum {
    Abc, Def, Ghi
}");

    [Fact] public void BaseTypeString() => new ExpressionCollection("Abc,Def,Ghi").BaseTypeSyntax().CodeEqual("string");
    [Fact] public void BaseTypeInt() => new ExpressionCollection(new IExpression[] { new LiteralExpression(1), new LiteralExpression(2) }).BaseTypeSyntax().CodeEqual("int");
    [Fact] public void BaseTypeObject() => new ExpressionCollection(new IExpression[] { new LiteralExpression("a"), new LiteralExpression(2) }).BaseTypeSyntax().CodeEqual("object");

    [Fact]
    public void GenerateArrayInitialization() => new ExpressionCollection("Abc,Def,Ghi").ToArrayInitialization().CodeEqual(@"new string[]{""Abc"",""Def"",""Ghi""}");

    [Fact]
    public void GenerateListInitialization() => new ExpressionCollection("Abc,Def,Ghi").ToListInitialization().CodeEqual(@"new List<string>{""Abc"",""Def"",""Ghi""}");

    [Fact]
    public void GenerateKeyValueInitialization() => new ExpressionsMap(new LiteralExpression(1), "Abc,Def,Ghi").ToKeyValueInitialization().CodeEqual(@"{1, new string[]{""Abc"",""Def"",""Ghi""} }", ignoreWhitespace: true);

    [Fact]
    public void GenerateDictionaryInitialization() => new ExpressionMap(new ExpressionsMap[]{
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
}".Parse().DescendantNodes().OfType<EnumDeclarationSyntax>().First()).Should()
        .BeEquivalentTo(new ExpressionCollection("Abc,Def,Ghi"), o => o.Excluding(x => x.Path.Contains("LiteralSyntax")));
}
