using CodeModels.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Rewriters;

public class JsonRewriter : CSharpSyntaxRewriter
{
    private readonly SemanticModel _model;

    public JsonRewriter(SemanticModel model)
    {
        _model = model;
    }

    public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        return node.IsPublic() ? node : null;
    }


}
