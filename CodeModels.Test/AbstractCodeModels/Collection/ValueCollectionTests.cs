using System.Linq;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Factory;
using CodeModels.Parsing;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestCommon;
using Xunit;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;
using static TestCommon.TestUtil;

namespace CodeModels.Test.AbstractCodeModels;

public class ValueCollectionTests
{
    [Fact]
    public void GenerateEnum() => Expressions("Abc,Def,Ghi").ToEnum("MyEnum").CodeEqual(@"
public enum MyEnum {
    Abc, Def, Ghi
}");

    [Fact] public void BaseTypeString() => Expressions("Abc,Def,Ghi").BaseTypeSyntax().CodeEqual("string");
    [Fact] public void BaseTypeInt() => Expressions(Literal(1), Literal(2)).BaseTypeSyntax().CodeEqual("int");
    [Fact] public void BaseTypeObject() => Expressions(Literal("a"), Literal(2)).BaseTypeSyntax().CodeEqual("object");

    [Fact]
    public void GenerateArrayInitialization() => Expressions("Abc,Def,Ghi").ToArrayInitialization().CodeEqual(@"new string[]{""Abc"",""Def"",""Ghi""}");

    [Fact]
    public void GenerateListInitialization() => Expressions("Abc,Def,Ghi").ToListInitialization().CodeEqual(@"new List<string>{""Abc"",""Def"",""Ghi""}");

    [Fact]
    public void GenerateKeyValueInitialization() => ExpressionsMap(Literal(1), "Abc,Def,Ghi").ToKeyValueInitializationSyntax().CodeEqual(@"{1, new string[]{""Abc"",""Def"",""Ghi""} }", ignoreWhitespace: true);

    [Fact]
    public void GenerateDictionaryInitialization() => ExpressionMap(new ExpressionsMap[]{
        ExpressionsMap(Literal(1), "Abc,Def,Ghi"),
        ExpressionsMap(Literal(2), "Jkl,Mno,Pqr")})
        .ToDictionary().CodeEqual(@"new Dictionary<int, string[]>(){ 
    {1, new string[]{""Abc"",""Def"",""Ghi""} },
    {2, new string[]{""Jkl"",""Mno"",""Pqr""} }
}", ignoreWhitespace: true);

    [Fact]
    public void ParseValueCollectionFromEnum() => Expressions(@"
public enum MyEnum {
    Abc, Def, Ghi
}".Parse().DescendantNodes().OfType<EnumDeclarationSyntax>().First()).Should()
        .BeEquivalentTo(Expressions("Abc,Def,Ghi"), o => o.Excluding(x => x.Path.Contains("LiteralSyntax")));
}
