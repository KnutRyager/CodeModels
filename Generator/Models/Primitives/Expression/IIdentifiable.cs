using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Models;

public interface IIdentifiable
{
    IdentifierExpression ToIdentifierExpression();
    IdentifierNameSyntax IdentifierNameSyntax();
    SyntaxToken IdentifierSyntax();
    SimpleNameSyntax NameSyntax();
}