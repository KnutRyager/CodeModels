using CodeAnalyzation.Test;
using FluentAssertions;
using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models.Primitives.Test;

public class TypeDeclarationTests
{
    [Fact]
    public void PropertiesAndFieldsWithOrdering() => new InstanceClass("ClassA", new PropertyCollection(new Property[] {
            Property(new QuickType("string"),"myPrivateField",new LiteralExpression("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            Property(new QuickType("string"),"myPrivateReadonlyField",new LiteralExpression("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            Property(new QuickType("string"),"myPrivateProperty",new LiteralExpression("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(new QuickType("int"),"p1"),
            new(new QuickType("string", false),"p2"),
            Property(new QuickType("double"),"PI",new LiteralExpression(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            Property(new QuickType("double"),"PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            Property(new QuickType("string"),"ThePublicStaticReadonlyField",new LiteralExpression("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
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
