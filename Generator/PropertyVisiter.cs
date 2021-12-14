using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class PropertyFilter
{
    public Type[] Attributes { get; set; }
    private string[] AttributeNames { get; set; }

    public PropertyFilter(Type[] attributes)
    {
        Attributes = attributes;
        AttributeNames = attributes.Select(x => x.Name).ToArray();
    }

    public bool Check(PropertyDeclarationSyntax node)
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

public class PropertyVisiter : CSharpSyntaxRewriter
{
    private readonly List<PropertyDeclarationSyntax> properties = new List<PropertyDeclarationSyntax>();
    private readonly PropertyFilter filter;

    public PropertyVisiter(PropertyFilter filter = null)
    {
        this.filter = filter;
    }

    public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        node = (base.VisitPropertyDeclaration(node) as PropertyDeclarationSyntax)!;
        if (filter?.Check(node) == true) properties.Add(node);

        return node;
    }

    public List<PropertyDeclarationSyntax> GetValues(SyntaxTree tree)
    {
        Visit(tree.GetRoot());
        return properties;
    }

    public List<PropertyDeclarationSyntax> GetValues(IEnumerable<SyntaxTree> trees)
    {
        foreach (var tree in trees) Visit(tree.GetRoot());
        return properties;
    }
}