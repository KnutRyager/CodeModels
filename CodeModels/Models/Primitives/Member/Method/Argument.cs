using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member
{
    // TODO: refKindKeyword
    public record Argument(string? Name, IExpression? Expression = null)
        : CodeModel<ArgumentSyntax>()
    {
        public static Method Create(string name, NamedValueCollection parameters, IType returnType, Block? body = null, IExpression? expressionBody = null, Modifier modifier = Modifier.Public)
            => new(name, parameters, returnType, body, expressionBody, modifier);

        public override IEnumerable<ICodeModel> Children()
        {
            if (Expression is not null) yield return Expression;
        }

        public override ArgumentSyntax Syntax()
            => SyntaxFactory.Argument(Name is null ? null : SyntaxFactory.NameColon(IdentifierName(Name)), Token(SyntaxKind.None), Expression?.Syntax());
    }
}
