using CodeAnalyzation.Models;
using FluentAssertions;
using Xunit;
using static CodeAnalysisTests.TestUtil;

namespace Generator.Test;

public class MethodHolderTests
{
    [Fact]
    public void PropertiesAndFieldsWithOrdering() => new MethodHolder("ClassA", new PropertyCollection(new Property[] {
            Property.FromValue(new TType("string"),"myPrivateField",Value.FromValue("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            Property.FromValue(new TType("string"),"myPrivateReadonlyField",Value.FromValue("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            Property.FromValue(new TType("string"),"myPrivateProperty",Value.FromValue("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(new TType("int"),"p1"),
            new(new TType("string", false),"p2"),
            Property.FromValue(new TType("double"),"PI",Value.FromValue(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            Property.FromQualifiedName(new TType("double"),"PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            Property.FromValue(new TType("string"),"ThePublicStaticReadonlyField",Value.FromValue("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
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

    [Fact]
    public void StaticClass() => new StaticClass("ClassA", new PropertyCollection(new Property[] {
            Property.FromValue(new TType("string"),"myPrivateField",Value.FromValue("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            Property.FromValue(new TType("string"),"myPrivateReadonlyField",Value.FromValue("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            Property.FromValue(new TType("string"),"myPrivateProperty",Value.FromValue("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(new TType("int"),"p1"),
            new(new TType("string", false),"p2"),
            Property.FromValue(new TType("double"),"PI",Value.FromValue(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            Property.FromQualifiedName(new TType("double"),"PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            Property.FromValue(new TType("string"),"ThePublicStaticReadonlyField",Value.FromValue("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
        })).ToClass().CodeEqual(@"
public static class ClassA {
    public const double PI = 3.14D;
    private const double PI_private = Math.PI;
    public static readonly string ThePublicStaticReadonlyField = ""abc"";
    private static readonly string myPrivateReadonlyField = ""myPrivateReadonlyFieldValue"";
    private static string myPrivateField = ""myPrivateFieldValue"";
    public static int p1 { get; set; }
    public static string? p2 { get; set; }
    private static string myPrivateProperty { get; set; } = ""myPrivatePropertyValue"";
}");


    [Fact] public void ComparePublic() => new PropertyComparer().Compare(Modifier.Public, Modifier.Private).Should().Be(-(int)Modifier.Public);
    [Fact] public void CompareConst() => new PropertyComparer().Compare(Modifier.Const, Modifier.Readonly).Should().Be(-(int)Modifier.Const);
    [Fact] public void CompareConstReverse() => new PropertyComparer().Compare(Modifier.Readonly, Modifier.Const).Should().Be((int)Modifier.Const);
}
