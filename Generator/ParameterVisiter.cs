using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class ParameterFilter
{
    public Type[] Attributes { get; set; }
    private string[] AttributeNames { get; set; }

    public ParameterFilter(Type[] attributes)
    {
        Attributes = attributes;
        AttributeNames = attributes.Select(x => x.Name).ToArray();
    }

    public bool Check(ParameterSyntax node)
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

public class ParameterVisiter : CSharpSyntaxRewriter
{
    private readonly List<ParameterSyntax> parameters = new();
    private readonly ParameterFilter? filter;

    public ParameterVisiter(ParameterFilter? filter = null)
    {
        this.filter = filter;
    }

    public override SyntaxNode VisitParameter(ParameterSyntax node)
    {
        node = (base.VisitParameter(node) as ParameterSyntax)!;
        if (filter?.Check(node) == true) parameters.Add(node);

        return node;
    }

    public List<ParameterSyntax> GetValues(SyntaxTree tree)
    {
        Visit(tree.GetRoot());
        return parameters;
    }

    public List<ParameterSyntax> GetValues(IEnumerable<SyntaxTree> trees)
    {
        foreach (var tree in trees) Visit(tree.GetRoot());
        return parameters;
    }
}