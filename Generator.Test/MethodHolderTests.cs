using CodeAnalyzation.Models;
using FluentAssertions;
using Xunit;
using static CodeAnalysisTests.TestUtil;

namespace Generator.Test;

public class MethodHolderTests
{

    [Fact]
    public void PropertiesAndFieldsWithOrdering() => new MethodHolder(new PropertyCollection(new Property[] {
            Property.FromValue(new TType("string"),"myPrivateField",Value.FromValue("myPrivateFieldValue"), propertyType: PropertyType.PrivateField),
            Property.FromValue(new TType("string"),"myPrivateReadonlyField",Value.FromValue("myPrivateReadonlyFieldValue"), propertyType: PropertyType.PrivateReadonlyField),
            Property.FromValue(new TType("string"),"myPrivateProperty",Value.FromValue("myPrivatePropertyValue"), propertyType: PropertyType.PrivateProperty),
            new(new TType("int"),"p1"),
            new(new TType("string", false),"p2"),
            Property.FromValue(new TType("double"),"PI",Value.FromValue(3.14), propertyType: PropertyType.PublicConst),
            Property.FromQualifiedName(new TType("double"),"PI_private","Math.PI", propertyType: PropertyType.PrivateConst),
            Property.FromValue(new TType("string"),"ThePublicStaticReadonlyField",Value.FromValue("abc"), propertyType: PropertyType.PublicStaticReadonlyField),
        }), "ClassA").ToClass().CodeEqual(@"
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
}
