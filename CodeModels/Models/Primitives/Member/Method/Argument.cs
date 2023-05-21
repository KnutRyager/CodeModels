using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member
{
    // TODO: refKindKeyword
    public record Argument(string? Name, IExpression Expression)
        : CodeModel<ArgumentSyntax>()
    {
        public static Argument Create(string? name, IExpression expression)
            => new(name, expression);

        public override IEnumerable<ICodeModel> Children()
        {
            yield return Expression;
        }

        public override ArgumentSyntax Syntax()
            => SyntaxFactory.Argument(Name is null ? null : SyntaxFactory.NameColon(IdentifierName(Name)), Token(SyntaxKind.None), Expression.Syntax());
    }
}
