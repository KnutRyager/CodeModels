using CodeAnalyzation.Models;
using FluentAssertions;
using Xunit;
using static CodeAnalysisTests.TestUtil;

namespace Generator.Test;

public class ConstructDependencyRecordTests
{
    [Fact]
    public void ConstructDepencyRecord()
    {
        var c = PropertyCollection.Parse(@"
[Model]
public class ClassA {
    public int p1 { get; set; }
    public string p2 { get; set; }
    public long? p3 { get; set; }
    public object? p4 { get; set; } = null;
    public A p5 { get; set; } = A.Instance;
    public int[] p6 { get; set; }
    public List<int> p7 { get; set; }
}");

        var record = c.ToRecord($"{c.Name}Include");
    }

}
