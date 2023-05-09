using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Collectors;

public class ParameterFilter : AbstractFilter<ParameterSyntax>
{
    public Type[]? Attributes { get; set; }
    private string[]? AttributeNames { get; set; }

    public ParameterFilter() : this(null) { }
    public ParameterFilter(Type[]? attributes)
    {
        Attributes = attributes;
        AttributeNames = attributes?.Select(x => x.Name).ToArray();
    }

    public override bool Check(ParameterSyntax node)
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

public class ParameterVisiter : AbstractVisiterCollector<ParameterSyntax, ParameterFilter>
{
    public ParameterVisiter(ParameterFilter? filter = null) : base(filter) { }
    public override SyntaxNode VisitParameter(ParameterSyntax node) => HandleVisit(node);
}
