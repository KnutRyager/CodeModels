using TestCommon;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models.Primitives.Test;

public class InstanceClassTests
{
    [Fact]
    public void GetPropertyAccessValue() => new InstanceClass("ClassA", NamedValues(
            NamedValue(Type("string"), "A", Literal("B"), modifier: PropertyAndFieldTypes.PublicField),
            NamedValue(Type("int"), "B")
        )).GetProperty("A").AccessValue("abc").CodeModelEqual("abc.A");
}
