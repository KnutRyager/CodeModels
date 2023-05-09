using TestCommon;
using Xunit;
using static CodeModels.Models.CodeModelFactory;

namespace CodeModels.Models.Primitives.Test;

public class ClassModelTests
{
    [Fact(Skip = "Old stuff")]
    public void GetPropertyAccessValue() => Class(
        "ClassA",
            Property("A", Literal("B"), modifier: PropertyAndFieldTypes.PublicField),
            Property<int>("B")).GetProperty("A").AccessValue("abc").CodeModelEqual("abc.A");

    [Fact(Skip = "Old stuff")]
    public void GetPropertyStaticAccessValueFromInstance() => Class(
        "ClassA",
            Property("A", Literal("B"), modifier: PropertyAndFieldTypes.PublicStaticField),
            Property<int>("B")).GetProperty("A").AccessValue("abc").CodeModelEqual("abc.A");

    [Fact(Skip = "Old stuff")]
    public void GetPropertyStaticAccessValueFromStaticReference() => Class(
        "ClassA",
            PropertyModel("A", Literal("B"), modifier: PropertyAndFieldTypes.PublicStaticField),
            PropertyModel<int>("B")).GetProperty("A").AccessValue().CodeModelEqual("ClassA.A");

    [Fact(Skip = "Old stuff")]
    public void GetFieldStaticAccessValueFromStaticReference() => Class(
        "ClassA",
            FieldModel("A", Literal("B"), modifier: PropertyAndFieldTypes.PublicStaticField),
            FieldModel<int>("B")).GetProperty("A").AccessValue().CodeModelEqual("ClassA.A");
}
