using CodeModels.AbstractCodeModels.Member;
using CodeModels.Factory;
using CodeModels.Models;
using TestCommon;
using Xunit;
using static CodeModels.Factory.AbstractCodeModelFactory;

namespace CodeModels.Test.AbstractCodeModels.Member;
using static CodeModelFactory;
public class InterfaceTests
{
    [Fact]
    public void GenerateInterface() => new InterfaceModel("InterfaceA", NamedValues(new[] {
            NamedValue<string>("myPrivateField", Literal("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            NamedValue<string>("myPrivateReadonlyField", Literal("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            NamedValue<string>("myPrivateProperty", Literal("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(Type<int>(),"p1"),
            new(QuickType("string?"),"p2"),
            NamedValue<double>("PI", Literal(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            NamedValue<double>("PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            NamedValue<string>("ThePublicStaticReadonlyField", Literal("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
        })).Syntax().CodeEqual(@"
public interface InterfaceA {
    const double PI = 3.14D;
    static readonly string ThePublicStaticReadonlyField = ""abc"";
    int p1 { get; set; }
    string? p2 { get; set; }
}");
}