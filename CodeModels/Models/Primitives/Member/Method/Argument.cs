using System.Collections.Generic;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member
{
    // TODO: refKindKeyword
    public record Argument(string? Name, IExpression Expression)
        : CodeModel<ArgumentSyntax>(), IToArgumentListConvertible
    {
        public static Argument Create(string? name, IExpression value)
            => new(name, value);

        public override IEnumerable<ICodeModel> Children()
        {
            yield return Expression;
        }

        public override ArgumentSyntax Syntax()
            => SyntaxFactory.Argument(Name is null ? null : NameColon(IdentifierName(Name)), Token(SyntaxKind.None), Expression.Syntax());

        public ArgumentList ToArgumentList() => ArgList(new[] { this });
    }
}
