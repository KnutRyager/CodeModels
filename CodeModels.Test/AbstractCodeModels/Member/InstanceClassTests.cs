using CodeModels.Models;
using TestCommon;
using Xunit;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.AbstractCodeModels.Member;

public class InstanceClassTests
{
    [Fact]
    public void GetPropertyAccessValue() => InstanceClass("ClassA", NamedValues(
            NamedValue(Type("string"), "A", Literal("B"), modifier: PropertyAndFieldTypes.PublicField),
            NamedValue(Type("int"), "B")))
        .GetProperty("A").AccessValue("abc").CodeModelEqual("abc.A");
}
