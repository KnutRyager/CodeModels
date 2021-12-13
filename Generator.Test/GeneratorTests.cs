using Xunit;
using static CodeAnalysisTests.TestUtil;

namespace Generator.Test;

public class GeneratorTests
{
    [Fact]
    public void FindModels()
    {
        var code = @"
namespace Models
{
    public class ModelA
    {
        public string StringProp { get; set; }
    }
    [Model]
    public class ModelB
    {
        public string StringProp { get; set; }
    }
    public class ModelC
    {
        public string StringProp { get; set; }
    }
}";
        code.SyntaxTrees().GetModelClasses().CodeEqual(@"
[Model]
public class ModelB
{
    public string StringProp { get; set; }
}");
    }
}
