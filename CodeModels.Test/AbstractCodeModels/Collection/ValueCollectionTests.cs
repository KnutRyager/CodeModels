using System.Linq;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Parsing;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestCommon;
using Xunit;
using static TestCommon.TestUtil;

namespace CodeModels.Test.AbstractCodeModels;

public class ValueCollectionTests
{
    [Fact]
    public void GenerateEnum() => new ExpressionCollection("Abc,Def,Ghi").ToEnum("MyEnum").CodeEqual(@"
public enum MyEnum {
    Abc, Def, Ghi
}");

    [Fact] public void BaseTypeString() => new ExpressionCollection("Abc,Def,Ghi").BaseTypeSyntax().CodeEqual("string");
    [Fact] public void BaseTypeInt() => new ExpressionCollection(new IExpression[] { CodeModelFactory.Literal(1), CodeModelFactory.Literal(2) }).BaseTypeSyntax().CodeEqual("int");
    [Fact] public void BaseTypeObject() => new ExpressionCollection(new IExpression[] { CodeModelFactory.Literal("a"), CodeModelFactory.Literal(2) }).BaseTypeSyntax().CodeEqual("object");

    [Fact]
    public void GenerateArrayInitialization() => new ExpressionCollection("Abc,Def,Ghi").ToArrayInitialization().CodeEqual(@"new string[]{""Abc"",""Def"",""Ghi""}");

    [Fact]
    public void GenerateListInitialization() => new ExpressionCollection("Abc,Def,Ghi").ToListInitialization().CodeEqual(@"new List<string>{""Abc"",""Def"",""Ghi""}");

    [Fact]
    public void GenerateKeyValueInitialization() => new ExpressionsMap(CodeModelFactory.Literal(1), "Abc,Def,Ghi").ToKeyValueInitializationSyntax().CodeEqual(@"{1, new string[]{""Abc"",""Def"",""Ghi""} }", ignoreWhitespace: true);

    [Fact]
    public void GenerateDictionaryInitialization() => new ExpressionMap(new ExpressionsMap[]{
        new (CodeModelFactory.Literal(1), "Abc,Def,Ghi"),
        new (CodeModelFactory.Literal(2), "Jkl,Mno,Pqr")})
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
