using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using CodeModels.Models.Primitives.Expression.Reference;

namespace CodeModels.Models.Interfaces;

public interface IIdentifiable
{
    IdentifierExpression ToIdentifierExpression();
    IdentifierNameSyntax IdentifierNameSyntax();
    SyntaxToken IdentifierSyntax();
    SimpleNameSyntax NameSyntax();
}