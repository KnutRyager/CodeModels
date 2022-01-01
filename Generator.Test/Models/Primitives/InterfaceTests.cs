using CodeAnalyzation.Test;
using static CodeAnalyzation.Models.CodeModelFactory;
using Xunit;

namespace CodeAnalyzation.Models.Primitives.Test;

public class InterfaceTests
{
    [Fact]
    public void GenerateInterface() => new InterfaceModel("InterfaceA", new PropertyCollection(new Property[] {
            Property(new QuickType("string"),"myPrivateField",new LiteralExpression("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            Property(new QuickType("string"),"myPrivateReadonlyField",new LiteralExpression("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            Property(new QuickType("string"),"myPrivateProperty",new LiteralExpression("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(new QuickType("int"),"p1"),
            new(new QuickType("string", false),"p2"),
            Property(new QuickType("double"),"PI",new LiteralExpression(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            Property(new QuickType("double"),"PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            Property(new QuickType("string"),"ThePublicStaticReadonlyField",new LiteralExpression("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
        })).SyntaxNode().CodeEqual(@"
public interface InterfaceA {
    const double PI = 3.14D;
    static readonly string ThePublicStaticReadonlyField = ""abc"";
    int p1 { get; set; }
    string? p2 { get; set; }
}");
}