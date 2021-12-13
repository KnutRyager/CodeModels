using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class ClassFilter
{
    public Type[]? Attributes { get; set; }
    private string[]? AttributeNames { get; set; }

    public ClassFilter(Type[]? attributes = null)
    {
        Attributes = attributes;
        AttributeNames = attributes?.Select(x => x.Name).ToArray();
    }

    public bool Check(ClassDeclarationSyntax node)
    {
        if (Attributes != null)
        {
            var attributes = node.AttributeLists.FirstOrDefault()?.Attributes;
            if (attributes == null) return false;
            var attributeNames = attributes.Value.Select(x => x.Name.NormalizeWhitespace().ToString()).ToArray();
            return AttributeNames.All(x => attributeNames.Any(y => x == $"{y}Attribute"));
        }

        return true;
    }
}

public class ClassVirtualizationVisitor : CSharpSyntaxRewriter
{
    private readonly List<ClassDeclarationSyntax> classes = new List<ClassDeclarationSyntax>();
    private readonly ClassFilter filter;

    public ClassVirtualizationVisitor(ClassFilter? filter = null)
    {
        this.filter = filter ?? new ClassFilter();
    }

    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        node = (ClassDeclarationSyntax)base.VisitClassDeclaration(node)!;
        if (filter == null || filter.Check(node))
            classes.Add(node);

        return node;
    }

    public List<ClassDeclarationSyntax> GetClasses(SyntaxTree tree)
    {
        Visit(tree.GetRoot());
        return classes;
    }

    public List<ClassDeclarationSyntax> GetClasses(IEnumerable<SyntaxTree> trees)
    {
        foreach (var tree in trees) Visit(tree.GetRoot());
        return classes;
    }
}