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
    public record Method(string Name, PropertyCollection Parameters, IType ReturnType, Block? Statements, IExpression? ExpressionBody = null, Modifier Modifier = Modifier.Public)
        : IMethod
    {
        public Method(string name, PropertyCollection parameters, IType returnType, Block body, Modifier modifier = Modifier.Public)
            : this(name, parameters, returnType, body, null, modifier) { }
        public Method(string name, PropertyCollection parameters, IType returnType, IExpression? body = null, Modifier modifier = Modifier.Public)
            : this(name, parameters, returnType, null, body, modifier) { }

        public MethodDeclarationSyntax ToMethodSyntax(Modifier modifiers = Modifier.None, Modifier removeModifier = Modifier.None) => MethodDeclarationCustom(
            attributeLists: new List<AttributeListSyntax>(),
            modifiers: Modifier.SetModifiers(modifiers).SetFlags(removeModifier, false).Syntax(),
            returnType: ReturnType.TypeSyntax(),
            explicitInterfaceSpecifier: default,
            identifier: Identifier(Name),
            typeParameterList: default,
            parameterList: Parameters.ToParameters(),
            constraintClauses: default,
            body: Statements?.Syntax(),
            expressionBody: ExpressionBody is null ? null : ArrowExpressionClause(ExpressionBody.Syntax()));

        public MemberDeclarationSyntax ToMemberSyntax(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None) => ToMethodSyntax(modifier, removeModifier);
        public CSharpSyntaxNode SyntaxNode() => ToMemberSyntax();

        public MethodDeclarationSyntax Syntax() => ToMethodSyntax();
        CSharpSyntaxNode ICodeModel.Syntax() => Syntax();
    }
}
