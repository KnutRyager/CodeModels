using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface INamedCodeModel : ICodeModel, IIdentifiable
{
}

public interface INamedCodeModel<T> : INamedCodeModel where T : CSharpSyntaxNode
{
}

public abstract record NamedCodeModel<T>(string Name) : CodeModel<T>, INamedCodeModel<T>
    where T : CSharpSyntaxNode
{
    public virtual SyntaxToken ToIdentifier() => SyntaxFactory.Identifier(Name);

    public virtual SimpleNameSyntax NameSyntax() => SyntaxFactory.IdentifierName(Name);

    public IdentifierNameSyntax IdentifierNameSyntax()
        => SyntaxFactory.IdentifierName(Name);

    public virtual SyntaxToken IdentifierSyntax()
        => SyntaxFactory.Identifier(Name);

    public virtual IdentifierExpression ToIdentifierExpression()
        => new(Name);
}
