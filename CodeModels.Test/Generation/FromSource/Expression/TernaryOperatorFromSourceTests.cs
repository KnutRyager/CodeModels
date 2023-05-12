using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Expression;

public class TernaryOperatorFromSourceTests
{
    [Fact] public void Ternary() => "true ? 1 : 0;".AssertParsedAndGeneratedEqual();
}