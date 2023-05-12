using CodeModels.Models;
using FluentAssertions;
using TestCommon;
using Xunit;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.AbstractCodeModels;

public class TypeDeclarationTests
{
    [Fact]
    public void PropertiesAndFieldsWithOrdering() => InstanceClass("ClassA", NamedValues(
            NamedValue(Type<string>(), "myPrivateField", Literal("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            NamedValue(Type<string>(), "myPrivateReadonlyField", Literal("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            NamedValue(Type<string>(), "myPrivateProperty", Literal("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(Type<int>(), "p1"),
            new(QuickType("string?"), "p2"),   // TODO: Type<string?>()
            NamedValue(Type<double>(), "PI", Literal(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            NamedValue(Type<double>(), "PI_private", "Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            NamedValue(Type<string>(), "ThePublicStaticReadonlyField", Literal("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField)))
        .ToClass().CodeEqual(@"
public class ClassA {
    public const double PI = 3.14D;
    private const double PI_private = Math.PI;
    public static readonly string ThePublicStaticReadonlyField = ""abc"";
    private readonly string myPrivateReadonlyField = ""myPrivateReadonlyFieldValue"";
    private string myPrivateField = ""myPrivateFieldValue"";
    public int p1 { get; set; }
    public string? p2 { get; set; }
    private string myPrivateProperty { get; set; } = ""myPrivatePropertyValue"";
}");

    [Fact] public void ComparePublic() => new ModifierComparer().Compare(Modifier.Public, Modifier.Private).Should().Be(-(int)Modifier.Public);
    [Fact] public void CompareConst() => new ModifierComparer().Compare(Modifier.Const, Modifier.Readonly).Should().Be(-(int)Modifier.Const);
    [Fact] public void CompareConstReverse() => new ModifierComparer().Compare(Modifier.Readonly, Modifier.Const).Should().Be((int)Modifier.Const);
}
