using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Collectors
{

    public  class ClassVisiter2: CSharpSyntaxRewriter
    {
        private readonly List<ClassDeclarationSyntax> entries = new();
        private readonly ClassFilter filter;

        public ClassVisiter2(ClassFilter? filter = null)
        {
            this.filter = filter ?? new ClassFilter();
        }

        protected SyntaxNode HandleVisit(ClassDeclarationSyntax node)
        {
            if (filter == null || filter.Check(node))
                entries.Add(node);

            return node;
        }

        public List<ClassDeclarationSyntax> GetEntries(SyntaxNode node)
        {
            Visit(node);
            return entries;
        }

        public List<ClassDeclarationSyntax> GetEntries(IEnumerable<SyntaxNode> nodes)
        {
            foreach (var node in nodes) Visit(node);
            return entries;
        }
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) => HandleVisit(node);

        public List<ClassDeclarationSyntax> GetEntries(SyntaxTree tree) => GetEntries(tree.GetRoot() as SyntaxNode);
        public List<ClassDeclarationSyntax> GetEntries(IEnumerable<SyntaxTree> trees) => GetEntries(trees.Select(x => x.GetRoot() as SyntaxNode));
    }
}