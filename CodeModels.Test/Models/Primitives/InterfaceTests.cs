using static CodeModels.Factory.CodeModelFactory;
using Xunit;
using TestCommon;
using CodeModels.Factory;

namespace CodeModels.Models.Primitives.Test;
using static CodeModelFactory;
public class InterfaceTests
{
    [Fact]
    public void GenerateInterface() => new InterfaceModel("InterfaceA", new NamedValueCollection(new Property[] {
            Property(Type<string>(),"myPrivateField", Literal("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            Property(Type<string>(),"myPrivateReadonlyField", Literal("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            Property(Type<string>(),"myPrivateProperty", Literal("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(Type<int>(),"p1"),
            new(QuickType("string", false),"p2"),
            Property(Type<double>(),"PI", Literal(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            Property(Type<double>(),"PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            Property(Type<string>(),"ThePublicStaticReadonlyField", Literal("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
        })).Syntax().CodeEqual(@"
public interface InterfaceA {
    const double PI = 3.14D;
    static readonly string ThePublicStaticReadonlyField = ""abc"";
    int p1 { get; set; }
    string? p2 { get; set; }
}");
}