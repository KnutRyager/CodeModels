using System.Linq;
using static CodeModels.Factory.CodeModelFactory;
using Xunit;
using TestCommon;

namespace CodeModels.Models.Primitives.Test;

public class MethodTests
{
    [Fact]
    public void BlockMethod() => Method("test", NamedValues(new[] { Property(Type("int"), "a") }), Type("string"),
        Block(LocalDeclaration(Declaration(Type("string"), "b", Literal("test"))), Return(Identifier("a"))), Modifier.Private)
        .CodeModelEqual(@"
private string test(int a){
    string b = ""test"";
    return a;
}");

    [Fact]
    public void ExpressionMethod() => Method("test", NamedValues(new[] { Property(Type("int"), "a") }), Type("string"),
         Literal("test"), Modifier.Private)
        .CodeModelEqual(@"
private string test(int a) => ""test"";
");

    [Fact]
    public void BodyConstructor() => Constructor("Test", NamedValues(new[] { Property(Type("int"), "a") }),
        Block(LocalDeclaration(Declaration(Type("string"), "b", Literal("test")))), Modifier.Private)
        .CodeModelEqual(@"
private Test(int a){
    string b = ""test"";
}");
}