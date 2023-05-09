using TestCommon;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models.Primitives.Test;

public class ClassModelTests
{
    [Fact(Skip = "Old stuff")]
    public void GetPropertyAccessValue() => Class(
        "ClassA",
            NamedValue("A", Literal("B"), modifier: PropertyAndFieldTypes.PublicField),
            NamedValue<int>("B")).GetProperty("A").AccessValue("abc").CodeModelEqual("abc.A");

    [Fact(Skip = "Old stuff")]
    public void GetPropertyStaticAccessValueFromInstance() => Class(
        "ClassA",
            NamedValue("A", Literal("B"), modifier: PropertyAndFieldTypes.PublicStaticField),
            NamedValue<int>("B")).GetProperty("A").AccessValue("abc").CodeModelEqual("abc.A");

    [Fact(Skip = "Old stuff")]
    public void GetPropertyStaticAccessValueFromStaticReference() => Class(
        "ClassA",
            Property("A", Literal("B"), modifier: PropertyAndFieldTypes.PublicStaticField),
            Property<int>("B")).GetProperty("A").AccessValue().CodeModelEqual("ClassA.A");

    [Fact(Skip = "Old stuff")]
    public void GetFieldStaticAccessValueFromStaticReference() => Class(
        "ClassA",
            Field("A", Literal("B"), modifier: PropertyAndFieldTypes.PublicStaticField),
            Field<int>("B")).GetProperty("A").AccessValue().CodeModelEqual("ClassA.A");
}
