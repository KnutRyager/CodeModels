using FluentAssertions;
using TestCommon;
using Xunit;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models.Primitives.Test;

public class TypeDeclarationTests
{
    [Fact]
    public void PropertiesAndFieldsWithOrdering() => new InstanceClass("ClassA", new NamedValueCollection(new Property[] {
            Property(Type<string>(),"myPrivateField", Literal("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            Property(Type<string>(),"myPrivateReadonlyField", Literal("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            Property(Type<string>(),"myPrivateProperty", Literal("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(Type<int>(),"p1"),
            new(QuickType("string", false),"p2"),   // TODO: Type<string?>()
            Property(Type<double>(),"PI", Literal(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            Property(Type<double>(),"PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            Property(Type<string>(),"ThePublicStaticReadonlyField", Literal("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
        })).ToClass().CodeEqual(@"
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
