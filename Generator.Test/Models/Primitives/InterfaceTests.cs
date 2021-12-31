using CodeAnalyzation.Test;
using Xunit;

namespace CodeAnalyzation.Models.Primitives.Test;

public class InterfaceTests
{
    [Fact]
    public void GenerateInterface() => new InterfaceModel("InterfaceA", new PropertyCollection(new Property[] {
            Property.FromValue(new QuickType("string"),"myPrivateField",new LiteralExpression("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            Property.FromValue(new QuickType("string"),"myPrivateReadonlyField",new LiteralExpression("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            Property.FromValue(new QuickType("string"),"myPrivateProperty",new LiteralExpression("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(new QuickType("int"),"p1"),
            new(new QuickType("string", false),"p2"),
            Property.FromValue(new QuickType("double"),"PI",new LiteralExpression(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            Property.FromQualifiedName(new QuickType("double"),"PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            Property.FromValue(new QuickType("string"),"ThePublicStaticReadonlyField",new LiteralExpression("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
        })).SyntaxNode().CodeEqual(@"
public interface InterfaceA {
    const double PI = 3.14D;
    static readonly string ThePublicStaticReadonlyField = ""abc"";
    int p1 { get; set; }
    string? p2 { get; set; }
}");
}