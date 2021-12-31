using System.Linq;
using CodeAnalyzation.Test;
using Xunit;

namespace CodeAnalyzation.Models.Primitives.Test;

public class StaticClassTests
{
    [Fact]
    public void GenerateStaticClass() => new StaticClass("ClassA", new PropertyCollection(new Property[] {
            Property.FromValue(new QuickType("string"),"myPrivateField",new LiteralExpression("myPrivateFieldValue"), modifier: PropertyAndFieldTypes.PrivateField),
            Property.FromValue(new QuickType("string"),"myPrivateReadonlyField",new LiteralExpression("myPrivateReadonlyFieldValue"), modifier: PropertyAndFieldTypes.PrivateReadonlyField),
            Property.FromValue(new QuickType("string"),"myPrivateProperty",new LiteralExpression("myPrivatePropertyValue"), modifier: PropertyAndFieldTypes.PrivateProperty),
            new(new QuickType("int"),"p1"),
            new(new QuickType("string", false),"p2"),
            Property.FromValue(new QuickType("double"),"PI",new LiteralExpression(3.14), modifier: PropertyAndFieldTypes.PublicConst),
            Property.FromQualifiedName(new QuickType("double"),"PI_private","Math.PI", modifier: PropertyAndFieldTypes.PrivateConst),
            Property.FromValue(new QuickType("string"),"ThePublicStaticReadonlyField",new LiteralExpression("abc"), modifier: PropertyAndFieldTypes.PublicStaticReadonlyField),
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

    [Fact]
    public void StaticMethodsFromReflection() => new StaticClass("Math",
        properties: new PropertyCollection(typeof(System.Math).GetFields().Select(x => new PropertyFromField(x))),
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