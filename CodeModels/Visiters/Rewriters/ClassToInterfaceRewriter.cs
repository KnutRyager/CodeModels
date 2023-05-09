using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Rewriters;

public class ClassToInterfaceRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        return InterfaceDeclaration(node.AttributeLists, node.Modifiers, node.Identifier, node.TypeParameterList, node.BaseList, node.ConstraintClauses, node.Members);
    }

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        return MethodDeclaration(node.AttributeLists, node.Modifiers, node.ReturnType, node.ExplicitInterfaceSpecifier, node.Identifier
            , node.TypeParameterList, node.ParameterList, node.ConstraintClauses, null, null);
    }
}
