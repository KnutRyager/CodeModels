using System.Linq;
using CodeModels.AbstractCodeModels;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Models;
using TestCommon;
using Xunit;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Test.AbstractCodeModels;

public class StaticClassTests
{
    [Fact]
    public void GenerateStaticClass() => StaticClass("ClassA", NamedValues(new AbstractProperty[] {
            NamedValue(Type("string"),"myPrivateField",Literal("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            NamedValue(Type("string"),"myPrivateReadonlyField",Literal("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            NamedValue(Type("string"),"myPrivateProperty",Literal("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            NamedValue(Type("int"),"p1"),
            NamedValue(Type("string", false),"p2"),
            NamedValue(Type("double"),"PI",Literal(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            NamedValue(Type("double"),"PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            NamedValue(Type("string"),"ThePublicStaticReadonlyField",Literal("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
        }), topLevelModifier: Modifier.Partial, memberModifier: Modifier.Public).ToClass().CodeEqual(@"
public static partial class ClassA {
    public const double PI = 3.14D;
    private const double PI_private = Math.PI;
    public static readonly string ThePublicStaticReadonlyField = ""abc"";
    private static readonly string myPrivateReadonlyField = ""myPrivateReadonlyFieldValue"";
    private static string myPrivateField = ""myPrivateFieldValue"";
    public static int p1 { get; set; }
    public static string? p2 { get; set; }
    private static string myPrivateProperty { get; set; } = ""myPrivatePropertyValue"";
}");

    [Fact]
    public void StaticMethodsFromReflection() => StaticClass("Math",
        properties: new NamedValueCollection(typeof(System.Math).GetFields().Select(x => new PropertyFromField(x))),
        methods: typeof(System.Math).GetMethods().Where(x => x.Name.StartsWith("B")).Select(x => new MethodFromReflection(x))).ToClass().CodeEqual(@"
public static class Math {
    public const double E = 2.718281828459045D;
    public const double PI = 3.141592653589793D;
    public const double Tau = 6.283185307179586D;
    public static long BigMul(int a, int b);
    public static ulong BigMul(ulong a, ulong b, UInt64& low);
    public static long BigMul(long a, long b, Int64& low);
    public static double BitDecrement(double x);
    public static double BitIncrement(double x);
}", ignoreWhitespace: true);
}