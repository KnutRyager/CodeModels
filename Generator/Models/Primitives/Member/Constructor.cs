using System.Collections.Generic;
using System.Linq;
using Common.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public record Constructor(string Name, PropertyCollection Parameters, Block? Statements, IExpression? ExpressionBody = null, Modifier Modifier = Modifier.Public)
        : CodeModel<ConstructorDeclarationSyntax>
    {
        public Constructor(string name, PropertyCollection parameters, Block body, Modifier modifier = Modifier.Public)
            : this(name, parameters, body, null, modifier) { }
        public Constructor(string name, PropertyCollection parameters, IExpression? body = null, Modifier modifier = Modifier.Public)
            : this(name, parameters, null, body, modifier) { }

        public ConstructorDeclarationSyntax ToConstructorSyntax(Modifier modifiers = Modifier.None, Modifier removeModifier = Modifier.None)
            => ConstructorDeclarationCustom(
            attributeLists: List<AttributeListSyntax>(),
            modifiers: Modifier.SetModifiers(modifiers).SetFlags(removeModifier, false).Syntax(),
            identifier: Identifier(Name),
            parameterList: Parameters.ToParameters(),
            body: Statements?.Syntax(),
            initializer: null,
            expressionBody: ExpressionBody is null ? null : ArrowExpressionClause(ExpressionBody.Syntax()));

        public MemberDeclarationSyntax ToMemberSyntax(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None) => ToConstructorSyntax(modifier, removeModifier);
        public CSharpSyntaxNode SyntaxNode() => ToMemberSyntax();

        public override ConstructorDeclarationSyntax Syntax() => ToConstructorSyntax();
    }
}
