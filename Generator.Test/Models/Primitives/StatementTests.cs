using System.Linq;
using CodeAnalyzation.Test;
using static CodeAnalyzation.Models.CodeModelFactory;
using Xunit;

namespace CodeAnalyzation.Models.Primitives.Test;

public class StatementTests
{
    [Fact]
    public void PlainIf() => If(Literal(true), LocalDeclaration(Declaration(Type("string"), "a", Literal("test")))).CodeModelEqual(@"
if(true) {
    string a = ""test"";
}");

    [Fact]
    public void IfWithElses() => MultiIf(new[]{
        If(Literal(true), LocalDeclaration(Declaration(Type("string"), "a", Literal("test")))),
        If(Literal(false), LocalDeclaration(Declaration(Type("int"), "b", Literal(1337)))),
        }, LocalDeclaration(Declaration(Type("double"), "c", Literal(4D))))
        .CodeModelEqual(@"
if(true) {
    string a = ""test"";
}
else if(false) {
    int b = 1337;
}
else {
    double c = 4D;
}");

    [Fact]
    public void ForLoop() => For(
         Declaration(Type("int"), "i", Literal(0)),
          BinaryExpression(Identifier("i"), OperationType.LessThan, Literal(10)),
          UnaryExpression(Identifier("i"), OperationType.UnaryAddAfter),
          Block(
              LocalDeclaration(Declaration(Type("string"), "a", Literal("test"))),
            LocalDeclaration(Declaration(Type("int"), "b", Literal(1337))))
          )
        .CodeModelEqual(@"
for(int i = 0; i < 10; i++) {
    string a = ""test"";
    int b = 1337;
}");

    [Fact]
    public void SimpleForLoop() => For(
         "i", Literal(10), LocalDeclaration(Declaration(Type("int"), "b", Literal(1337))))
        .CodeModelEqual(@"
for(int i = 0; i < 10; i++) {
    int b = 1337;
}");

    [Fact]
    public void ForEachLoop() => ForEach(
         "i", Identifier("collection"), LocalDeclaration(Declaration(Type("int"), "b", Literal(1337))))
        .CodeModelEqual(@"
foreach(var i in collection) {
    int b = 1337;
}");

    [Fact]
    public void WhileLoop() => While(Literal(true), LocalDeclaration(Declaration(Type("int"), "b", Literal(1337))))
        .CodeModelEqual(@"
while(true) {
    int b = 1337;
}");

    [Fact]
    public void DoWhileLoop() => Do(LocalDeclaration(Declaration(Type("int"), "b", Literal(1337))), Literal(true))
        .CodeModelEqual(@"
do {
    int b = 1337;
} while (true);");

    [Fact]
    public void ReturnStatement() => Return(Literal(true))
        .CodeModelEqual(@"
return true;");
}