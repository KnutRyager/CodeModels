using System.Collections.Generic;
using CodeModels.Models;
using CodeModels.Parsing;
using FluentAssertions;
using TestCommon;
using Xunit;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.AbstractCodeModels;

public class NamedValueCollectionTests
{
    [Fact]
    public void ArrayField() => FieldNamedValue("a", Values("v")).CodeModelEqual(@"
string[] a = new string[]{ ""v"" };
");

    [Fact]
    public void EmptyArrayField() => FieldNamedValue("a", Values()).CodeModelEqual(@"
object[] a = new object[]{ };
");

    [Fact]
    public void ParseNamedValueCollectionFromRecord() => NamedValues(@"
public record RecordA(
    int p1,
    string p2,
    long? p3,
    object? p4 = null,
    A p5 = A.Instance
);").Should().BeEquivalentTo(NamedValues(new[] {
            NamedValue(Type("int"),"p1", modifier: Modifier.Public),
            NamedValue(Type("string"),"p2", modifier: Modifier.Public),
            NamedValue(Type("long?"),"p3", modifier: Modifier.Public),
            NamedValue(Type("object?"),"p4", "null".ExpressionTree(), modifier: Modifier.Public),
            NamedValue(Type("A"),"p5", "A.Instance".ExpressionTree(), modifier: Modifier.Public),
        }, "RecordA"), o => o.Excluding(x => x.Path.Contains("Identifier") || x.Path.Contains("Type.SourceSyntax") || x.Path.Contains("NameSyntax") || x.Path.Contains("ExpressionSyntax")));

    // TODO: Parse without SemanticModel(?)
    //[Fact]
    //public void ParseNamedValueCollectionFromTuple() => NamedValueCollection("(int p1, string p2, long? p3, object? p4, A p5, uint Item6, float)")
    //    .Should().BeEquivalentTo(NamedValueCollection(new[] {
    //        Property(Type("int"),"p1", modifier: Modifier.Public),
    //        Property(Type("string"),"p2", modifier: Modifier.Public),
    //        Property(Type("long", false),"p3", modifier: Modifier.Public),
    //        Property(Type("object", false),"p4", modifier: Modifier.Public),
    //        Property(Type("A"),"p5", modifier: Modifier.Public),
    //        Property(Type("uint"),"Item6", modifier: Modifier.Public),
    //        Property(Type("float"),"Item7", modifier: Modifier.Public),
    //    }), o => o.Excluding(x => x.Path.Contains("Identifier") || x.Path.Contains("Type.SourceSyntax") || x.Path.Contains("NameSyntax") || x.Path.Contains("ExpressionSyntax")));

    [Fact]
    public void ParseNamedValueCollectionFromClass() => NamedValues(@"
public class ClassA {
    public int p1 { get; set; }
    private string p2 { get; set; }
    protected long? p3 { get; set; }
    internal object? p4 { get; set; } = null;
    public A p5 { get; set; } = A.Instance;
    public int[] p6 { get; set; }
    public List<int> p7 { get; set; }
}").Should().BeEquivalentTo(NamedValues("ClassA",
            NamedValue<int>("p1", modifier: Modifier.Public),
            NamedValue<string>("p2", modifier: Modifier.Private),
            NamedValue<long?>("p3", modifier: Modifier.Protected),
            NamedValue(Type("object?"), "p4", "null".ExpressionTree(), modifier: Modifier.Internal),
            NamedValue(Type("A"), "p5", "A.Instance".ExpressionTree(), modifier: Modifier.Public),
            NamedValue<int[]>("p6", modifier: Modifier.Public),
            NamedValue<List<int>>("p7", modifier: Modifier.Public)),
        o => o.Excluding(x => x.Path.Contains("ReflectedType") || x.Path.Contains("Identifier") || x.Path.Contains("Type.SourceSyntax") || x.Path.Contains("NameSyntax") || x.Path.Contains("ExpressionSyntax")));

    [Fact]
    public void GenerateClass() => NamedValues("ClassA",
            NamedValue<int>("p1"),
            NamedValue<string>("p2"),
            NamedValue<long?>("p3"),
            NamedValue(Type("object?"), "p4", "null".ExpressionTree()),
            NamedValue(Type("A"), "p5", "A.Instance".ExpressionTree()),
            NamedValue(Type("int[]"), "p6"),
            NamedValue(Type("List<int>"), "p7"))
        .ToClass().CodeEqual(@"
public class ClassA {
    public int p1 { get; set; }
    public string p2 { get; set; }
    public long? p3 { get; set; }
    public object? p4 { get; set; } = null;
    public A p5 { get; set; } = A.Instance;
    public int[] p6 { get; set; }
    public List<int> p7 { get; set; }
}");

    [Fact]
    public void GenerateRecord() => NamedValues("RecordA",
            NamedValue(Type("int"), "p1"),
            NamedValue(Type("string"), "p2"),
            NamedValue(Type("long?"), "p3"),
            NamedValue(Type("object?"), "p4", "null".ExpressionTree()),
            NamedValue(Type("A"), "p5", "A.Instance".ExpressionTree()),
            NamedValue(Type("int[]"), "p6"),
            NamedValue(Type("List<int>"), "p7"))
        .ToRecord().CodeEqual("public record RecordA(int p1, string p2, long? p3, object? p4 = null, A p5 = A.Instance, int[] p6, List<int> p7);");

    [Fact]
    public void GenerateTuple() => NamedValues(
            NamedValue(Type("int"), "p1"),
            NamedValue(Type("string"), "p2"),
            NamedValue(Type("long?"), "p3"),
            NamedValue(Type("object?"), "p4", "null".ExpressionTree()),
            NamedValue(Type("A"), "p5", "A.Instance".ExpressionTree()),
            NamedValue(Type("uint"), "Item6"),
            NamedValue(Type("float"), default),
            NamedValue(Type("int[]"), "p6"),
            NamedValue(Type("List<int>"), "p7"))
        .ToTupleType().CodeEqual("(int p1, string p2, long? p3, object? p4, A p5, uint, float, int[] p6, List<int> p7)");
}
