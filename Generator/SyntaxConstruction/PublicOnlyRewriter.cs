using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.SyntaxConstruction
{
    public class PublicOnlyRewriter : CSharpSyntaxRewriter
    {
        //private readonly SemanticModel _model;

        public PublicOnlyRewriter()
        {
            //_model = model;
        }

        public override SyntaxNode? Visit(SyntaxNode? node)
        {
            try
            {
                return node is not MemberDeclarationSyntax memberNode || memberNode.IsPublic() ? base.Visit(node) : null;
            }
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            {
                return null;
            }
        }
    }
}