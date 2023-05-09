using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Collectors;

public class ClassFilter : AbstractFilter<ClassDeclarationSyntax>
{
    public Type[]? Attributes { get; set; }
    private string[]? AttributeNames { get; set; }

    public ClassFilter() : this(null) { }
    public ClassFilter(Type[]? attributes = null)
    {
        Attributes = attributes;
        AttributeNames = attributes?.Select(x => x.Name).ToArray();
    }

    public override bool Check(ClassDeclarationSyntax node)
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

public class ClassVisiter : AbstractVisiterCollector<ClassDeclarationSyntax, ClassFilter>
{
    public ClassVisiter(ClassFilter? filter = null) : base(filter) { }
    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) => HandleVisit(node);
}
