using CodeModels.AbstractCodeModels;
using CodeModels.Factory;
using CodeModels.Models;
using TestCommon;
using Xunit;
using static CodeModels.Factory.AbstractCodeModelFactory;

namespace CodeModels.Test.AbstractCodeModels;
using static CodeModelFactory;
public class InterfaceTests
{
    [Fact]
    public void GenerateInterface() => new InterfaceModel("InterfaceA", NamedValues(new AbstractProperty[] {
            NamedValue(Type<string>(),"myPrivateField", Literal("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            NamedValue(Type<string>(),"myPrivateReadonlyField", Literal("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            NamedValue(Type<string>(),"myPrivateProperty", Literal("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(Type<int>(),"p1"),
            new(QuickType("string", false),"p2"),
            NamedValue(Type<double>(),"PI", Literal(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            NamedValue(Type<double>(),"PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            NamedValue(Type<string>(),"ThePublicStaticReadonlyField", Literal("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
        })).Syntax().CodeEqual(@"
public interface InterfaceA {
    const double PI = 3.14D;
    static readonly string ThePublicStaticReadonlyField = ""abc"";
    int p1 { get; set; }
    string? p2 { get; set; }
}");
}