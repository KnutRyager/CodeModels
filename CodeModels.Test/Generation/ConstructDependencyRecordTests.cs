using Xunit;
using static CodeModels.Factory.AbstractCodeModelFactory;

namespace CodeModels.Generation.Test;

public class ConstructDependencyRecordTests
{
    [Fact]
    public void ConstructDepencyRecord()
    {
        var c = NamedValues(@"
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
