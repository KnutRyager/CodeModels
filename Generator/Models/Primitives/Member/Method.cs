using System.Collections.Generic;
using System.Linq;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public record Method(string Name, PropertyCollection Parameters, IType ReturnType, Block? Statements, IExpression? ExpressionBody = null,
        Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        : MemberModel<MethodDeclarationSyntax>(Name, ReturnType, Attributes ?? new List<AttributeList>(), Modifier), IMethod
    {
        public Method(string name, PropertyCollection parameters, IType returnType, Block body, Modifier modifier = Modifier.Public)
            : this(name, parameters, returnType, body, null, modifier) { }
        public Method(string name, PropertyCollection parameters, IType returnType, IExpression? body = null, Modifier modifier = Modifier.Public)
            : this(name, parameters, returnType, null, body, modifier) { }

        public MethodDeclarationSyntax ToMethodSyntax(Modifier modifiers = Modifier.None, Modifier removeModifier = Modifier.None) => MethodDeclarationCustom(
            attributeLists: new List<AttributeListSyntax>(),
            modifiers: Modifier.SetModifiers(modifiers).SetFlags(removeModifier, false).Syntax(),
            returnType: ReturnType.Syntax() ?? TypeShorthands.VoidType.Syntax()!,
            explicitInterfaceSpecifier: default,
            identifier: Identifier(Name),
            typeParameterList: default,
            parameterList: Parameters.ToParameters(),
            constraintClauses: default,
            body: Statements?.Syntax(),
            expressionBody: ExpressionBody is null ? null : ArrowExpressionClause(ExpressionBody.Syntax()));

        public InvocationExpression Invoke(IExpression caller, IEnumerable<IExpression> arguments) => Invocation(this, caller, arguments);
        public InvocationExpression Invoke(IExpression caller, params IExpression[] arguments) => Invocation(this, caller, arguments);
        public InvocationExpression Invoke(string identifier, IEnumerable<IExpression> arguments) => Invoke(CodeModelFactory.Identifier(identifier), arguments);
        public InvocationExpression Invoke(string identifier, params IExpression[] arguments) => Invoke(CodeModelFactory.Identifier(identifier), arguments);
        public InvocationExpression Invoke(string identifier, IType? type, ISymbol? symbol, IEnumerable<IExpression> arguments) => Invoke(Identifier(identifier, type, symbol), arguments);
        public InvocationExpression Invoke(string identifier, IType? type, ISymbol? symbol, params IExpression[] arguments) => Invoke(Identifier(identifier, type, symbol), arguments);
        public InvocationExpression Invoke(string identifier, IType type, IEnumerable<IExpression> arguments) => Invoke(Identifier(identifier, type), arguments);
        public InvocationExpression Invoke(string identifier, IType type, params IExpression[] arguments) => Invoke(Identifier(identifier, type), arguments);
        public InvocationExpression Invoke(string identifier, ISymbol symbol, IEnumerable<IExpression> arguments) => Invoke(Identifier(identifier, symbol: symbol), arguments);
        public InvocationExpression Invoke(string identifier, ISymbol symbol, params IExpression[] arguments) => Invoke(Identifier(identifier, symbol: symbol), arguments);

        public override MethodDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
            => ToMethodSyntax(modifier, removeModifier);

        public override IEnumerable<ICodeModel> Children()
        {
            foreach (var property in Parameters.Properties) yield return property;
            if (Statements is not null) yield return Statements;
            if (ExpressionBody is not null) yield return ExpressionBody;
            yield return ReturnType;
        }
    }
}
