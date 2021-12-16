using CodeAnalyzation.Models;
using FluentAssertions;
using Xunit;
using static CodeAnalysisTests.TestUtil;

namespace Generator.Test;

public class PropertyCollectionTests
{
    [Fact]
    public void ParsePropertyCollectionFromRecord() => PropertyCollection.Parse("public record RecordA(int p1, string p2, long? p3, object? p4 = null, A p5 = A.Instance);")
        .Should().Equals(new PropertyCollection(new Property[] {
            new(new TType("int"),"p1"),
            new(new TType("string"),"p2"),
            new(new TType("long", false),"p3"),
            Property.FromExpression(new TType("object", false),"p4", "null".ExpressionTree()),
            Property.FromExpression(new TType("A"),"p5", "A.Instance".ExpressionTree()),
        }, "RecordA"));

    [Fact]
    public void ParsePropertyCollectionFromTuple() => PropertyCollection.Parse("(int p1, string p2, long? p3, object? p4, A p5, uint Item6, float)")
        .Should().Equals(new PropertyCollection(new Property[] {
            new(new TType("int"),"p1"),
            new(new TType("string"),"p2"),
            new(new TType("long", false),"p3"),
            new(new TType("object", false),"p4"),
            new(new TType("A"),"p5"),
            new(new TType("A"),"Item6"),
            new(new TType("A"),"Item7"),
        }));

    [Fact]
    public void ParsePropertyCollectionFromClass() => PropertyCollection.Parse(@"
public class ClassA {
    public int p1 { get; set; }
    public string p2 { get; set; }
    public long? p3 { get; set; }
    public object? p4 { get; set; } = null;
    public A p5 { get; set; } = A.Instance;
    public int[] p6 { get; set; }
    public List<int> p7 { get; set; }
}").Should().Equals(new PropertyCollection(new Property[] {
            new(new TType("int"),"p1"),
            new(new TType("string"),"p2"),
            new(new TType("long", false),"p3"),
            Property.FromExpression(new TType("object", false),"p4","null".ExpressionTree()),
            Property.FromExpression(new TType("A"),"p5","A.Instance".ExpressionTree()),
            new(new TType("int[]"),"p6"),
            new(new TType("List<int>"),"p7"),
        }, "ClassA"));

    [Fact]
    public void GenerateClass() => new PropertyCollection(new Property[] {
            new(new TType("int"),"p1"),
            new(new TType("string"),"p2"),
            new(new TType("long", false),"p3"),
            Property.FromExpression(new TType("object", false),"p4","null".ExpressionTree()),
            Property.FromExpression(new TType("A"),"p5","A.Instance".ExpressionTree()),
            new(new TType("int[]"),"p6"),
            new(new TType("List<int>"),"p7"),
        }, "ClassA").ToClass().CodeEqual(@"
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
    public void GenerateRecord() => new PropertyCollection(new Property[] {
            new(new TType("int"),"p1"),
            new(new TType("string"),"p2"),
            new(new TType("long", false),"p3"),
            Property.FromExpression(new TType("object", false),"p4","null".ExpressionTree()),
            Property.FromExpression(new TType("A"),"p5","A.Instance".ExpressionTree()),
            new(new TType("int[]"),"p6"),
            new(new TType("List<int>"),"p7"),
        }, "RecordA").ToRecord().CodeEqual("public record RecordA(int p1, string p2, long? p3, object? p4 = null, A p5 = A.Instance, int[] p6, List<int> p7);");

    [Fact]
    public void GenerateTuple() => new PropertyCollection(new Property[] {
            new(new TType("int"),"p1"),
            new(new TType("string"),"p2"),
            new(new TType("long", false),"p3"),
            Property.FromExpression(new TType("object", false),"p4","null".ExpressionTree()),
            Property.FromExpression(new TType("A"),"p5","A.Instance".ExpressionTree()),
            new(new TType("uint"), "Item6"),
            new(new TType("float"), default),
            new(new TType("int[]"),"p6"),
            new(new TType("List<int>"),"p7"),
        }).ToTuple().CodeEqual("(int p1, string p2, long? p3, object? p4, A p5, uint, float, int[] p6, List<int> p7)");
}
