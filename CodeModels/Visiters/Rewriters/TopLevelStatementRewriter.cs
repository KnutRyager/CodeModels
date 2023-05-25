using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class TopLevelStatementRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
    {
        var isMainClass = node.Members.Any(x => x is ClassDeclarationSyntax @class && IsMainClass(@class));
        if (!isMainClass) return node;
        var members = new List<MemberDeclarationSyntax>();
        foreach (var member in node.Members)
        {
            if (member is ClassDeclarationSyntax @class)
            {
                var topLevelStatements = new List<GlobalStatementSyntax>();
                foreach (var classMember in @class.Members)
                {
                    if (classMember is MethodDeclarationSyntax { Body: BlockSyntax body } method && IsMainMethod(method))
                    {
                        foreach (var methodStatement in body.Statements)
                        {
                            topLevelStatements.Add(SyntaxFactory.GlobalStatement(methodStatement));
                        }
                    }
                    else
                    {
                        //topLevelStatements.Add(SyntaxFactory.GlobalStatement(classMember));
                    }

                }
                members.AddRange(topLevelStatements);
            }
            else
            {
                members.Add(member);
            }
        }
        return node.WithMembers(SyntaxFactory.List(members));
        //return base.VisitClassDeclaration(node.WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>()));
    }

    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        // Remove the class declaration
        return node;
        //return base.VisitClassDeclaration(node.WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>()));
    }

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        return node;
        //if (node.Identifier.ValueText == "Main" && node.Modifiers.Any(SyntaxKind.StaticKeyword))
        //{
        //    // Remove the method declaration and convert the method body into top-level statements
        //    return SyntaxFactory.GlobalStatement(node.Body.Statements.Select(x => x.).ToArray());
        //}
        //else
        //{
        //    return base.VisitMethodDeclaration(node);
        //}
    }

    private static bool IsMainClass(ClassDeclarationSyntax @class)
        => @class.Members.Any(x => x is MethodDeclarationSyntax method && IsMainMethod(method));

    private static bool IsMainMethod(MethodDeclarationSyntax method)
        => method.Identifier.ValueText == "Main" && method.Modifiers.Any(SyntaxKind.StaticKeyword);
}