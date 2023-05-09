using Xunit;
using static CodeModels.Models.CodeModelFactory;
using static CodeModels.Models.ProgramModelFactory;
using CodeModels.Models.ProgramModels;

namespace CodeModels.Generation.Test;

public class ModelRegistryTests
{
    [Fact]
    public void ConstructDepencyRecord()
    {
        var model = PropertyCollection(@"
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
        var c = ModelRegistry(new[] { model });



    }

}
