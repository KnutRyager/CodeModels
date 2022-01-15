using CodeAnalyzation.Parsing;
using CodeAnalyzation.Test;
using FluentAssertions;
using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models.Primitives.Test;

public class PropertyCollectionTests
{
    [Fact]
    public void ArrayField() => Field("a", Values("v")).CodeModelEqual(@"
string[] a = new string[]{ ""v"" };
");

    [Fact]
    public void EmptyArrayField() => Field("a", Values()).CodeModelEqual(@"
object[] a = new object[]{ };
");


    [Fact]
    public void ParsePropertyCollectionFromRecord() => PropertyCollection("public record RecordA(int p1, string p2, long? p3, object? p4 = null, A p5 = A.Instance);")
        .Should().BeEquivalentTo(PropertyCollection(new Property[] {
            Property(Type("int"),"p1", modifier: Modifier.Public),
            Property(Type("string"),"p2", modifier: Modifier.Public),
            Property(Type("long", false),"p3", modifier: Modifier.Public),
            Property(Type("object", false),"p4", "null".ExpressionTree(), modifier: Modifier.Public),
            Property(Type("A"),"p5", "A.Instance".ExpressionTree(), modifier: Modifier.Public),
        }, "RecordA"), o => o.Excluding(x => x.Path.Contains("Identifier") || x.Path.Contains("Type.SourceSyntax") || x.Path.Contains("NameSyntax") || x.Path.Contains("ExpressionSyntax")));

    [Fact]
    public void ParsePropertyCollectionFromTuple() => PropertyCollection("(int p1, string p2, long? p3, object? p4, A p5, uint Item6, float)")
        .Should().BeEquivalentTo(PropertyCollection(new Property[] {
            Property(Type("int"),"p1", modifier: Modifier.Public),
            Property(Type("string"),"p2", modifier: Modifier.Public),
            Property(Type("long", false),"p3", modifier: Modifier.Public),
            Property(Type("object", false),"p4", modifier: Modifier.Public),
            Property(Type("A"),"p5", modifier: Modifier.Public),
            Property(Type("uint"),"Item6", modifier: Modifier.Public),
            Property(Type("float"),"Item7", modifier: Modifier.Public),
        }), o => o.Excluding(x => x.Path.Contains("Identifier") || x.Path.Contains("Type.SourceSyntax") || x.Path.Contains("NameSyntax") || x.Path.Contains("ExpressionSyntax")));

    [Fact]
    public void ParsePropertyCollectionFromClass() => PropertyCollection(@"
public class ClassA {
    public int p1 { get; set; }
    private string p2 { get; set; }
    protected long? p3 { get; set; }
    internal object? p4 { get; set; } = null;
    public A p5 { get; set; } = A.Instance;
    public int[] p6 { get; set; }
    public List<int> p7 { get; set; }
}").Should().BeEquivalentTo(PropertyCollection(new Property[] {
            Property(Type("int"),"p1", modifier: Modifier.Public),
            Property(Type("string"),"p2", modifier: Modifier.Private),
            Property(Type("long", false),"p3", modifier: Modifier.Protected),
            Property(Type("object", false),"p4","null".ExpressionTree(), modifier: Modifier.Internal),
            Property(Type("A"),"p5","A.Instance".ExpressionTree(), modifier: Modifier.Public),
            Property(Type("int[]"),"p6", modifier: Modifier.Public),
            Property(Type("List<int>"),"p7", modifier: Modifier.Public),
        }, "ClassA"), o => o.Excluding(x => x.Path.Contains("Identifier") || x.Path.Contains("Type.SourceSyntax") || x.Path.Contains("NameSyntax") || x.Path.Contains("ExpressionSyntax")));

    [Fact]
    public void GenerateClass() => PropertyCollection(new Property[] {
            Property(Type("int"),"p1"),
            Property(Type("string"),"p2"),
            Property(Type("long", false),"p3"),
            Property(Type("object", false),"p4","null".ExpressionTree()),
            Property(Type("A"),"p5","A.Instance".ExpressionTree()),
            Property(Type("int[]"),"p6"),
            Property(Type("List<int>"),"p7"),
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
            Property(Type("int"),"p1"),
            Property(Type("string"),"p2"),
            Property(Type("long", false),"p3"),
            Property(Type("object", false),"p4","null".ExpressionTree()),
            Property(Type("A"),"p5","A.Instance".ExpressionTree()),
            Property(Type("int[]"),"p6"),
            Property(Type("List<int>"),"p7"),
        }, "RecordA").ToRecord().CodeEqual("public record RecordA(int p1, string p2, long? p3, object? p4 = null, A p5 = A.Instance, int[] p6, List<int> p7);");

    [Fact]
    public void GenerateTuple() => new PropertyCollection(new Property[] {
            Property(Type("int"),"p1"),
            Property(Type("string"),"p2"),
            Property(Type("long", false),"p3"),
            Property(Type("object", false),"p4","null".ExpressionTree()),
            Property(Type("A"),"p5","A.Instance".ExpressionTree()),
            Property(Type("uint"), "Item6"),
            Property(Type("float"), default),
            Property(Type("int[]"),"p6"),
            Property(Type("List<int>"),"p7"),
        }).ToTuple().CodeEqual("(int p1, string p2, long? p3, object? p4, A p5, uint, float, int[] p6, List<int> p7)");
}
