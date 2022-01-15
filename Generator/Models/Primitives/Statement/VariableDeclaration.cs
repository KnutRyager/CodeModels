using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public record VariableDeclaration(IType Type, string Name, IExpression? Value = null) : CodeModel<VariableDeclarationSyntax>
    {
        public override VariableDeclarationSyntax Syntax() => VariableDeclarationCustom(Type.Syntax()!, VariableDeclaratorCustom(Identifier(Name), Value?.Syntax()));
        public override IEnumerable<ICodeModel> Children()
        {
            yield return Type;
            if (Value is not null) yield return Value;
        }
    }
    //public record VariableDeclarationExpression(IType Type, string Name, IExpression? Value = null) : Expression<DeclarationExpressionSyntax>
    //{
    //    public override DeclarationExpressionSyntax Syntax()
    //        => DeclarationExpressionCustom(Type.Syntax!, DeclarationExpressionCustom(Name, Value.Syntax()));
    //}



}