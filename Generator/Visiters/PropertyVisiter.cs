using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class PropertyFilter : AbstractFilter<PropertyDeclarationSyntax>
{
    public Type[]? Attributes { get; set; }
    private string[]? AttributeNames { get; set; }

    public PropertyFilter() : this(null) { }
    public PropertyFilter(Type[]? attributes =null)
    {
        Attributes = attributes;
        AttributeNames = attributes?.Select(x => x.Name).ToArray();
    }

    public override bool Check(PropertyDeclarationSyntax node)
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

public class PropertyVisiter : AbstractVisiterCollector<PropertyDeclarationSyntax, PropertyFilter>
{
    public PropertyVisiter(PropertyFilter? filter = null) : base(filter) { }
    public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) => HandleVisit(node);
}