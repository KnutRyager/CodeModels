using CodeAnalyzation.Models;
using FluentAssertions;
using Xunit;

namespace Generator.Test.Models.Primitives;

public class MethodHolderTests
{
    [Fact]
    public void PropertiesAndFieldsWithOrdering() => new MethodHolder("ClassA", new PropertyCollection(new Property[] {
            Property.FromValue(new QuickType("string"),"myPrivateField",new LiteralExpression("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            Property.FromValue(new QuickType("string"),"myPrivateReadonlyField",new LiteralExpression("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            Property.FromValue(new QuickType("string"),"myPrivateProperty",new LiteralExpression("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(new QuickType("int"),"p1"),
            new(new QuickType("string", false),"p2"),
            Property.FromValue(new QuickType("double"),"PI",new LiteralExpression(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            Property.FromQualifiedName(new QuickType("double"),"PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            Property.FromValue(new QuickType("string"),"ThePublicStaticReadonlyField",new LiteralExpression("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
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

    [Fact] public void ComparePublic() => new PropertyComparer().Compare(Modifier.Public, Modifier.Private).Should().Be(-(int)Modifier.Public);
    [Fact] public void CompareConst() => new PropertyComparer().Compare(Modifier.Const, Modifier.Readonly).Should().Be(-(int)Modifier.Const);
    [Fact] public void CompareConstReverse() => new PropertyComparer().Compare(Modifier.Readonly, Modifier.Const).Should().Be((int)Modifier.Const);


}
