using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public record VariableDeclaration(IType Type, string Name, IExpression Value) : CodeModel<VariableDeclarationSyntax>
    {
        public override VariableDeclarationSyntax Syntax() => VariableDeclarationCustom(Type.Syntax!, VariableDeclaratorCustom(Identifier(Name)));
    }
}