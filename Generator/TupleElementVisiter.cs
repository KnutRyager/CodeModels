using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class TupleElementFilter
{
    public TupleElementFilter()
    {
    }

    public bool Check(TupleElementSyntax node)
    {
        return true;
    }
}

public class TupleElementVisiter : CSharpSyntaxRewriter
{
    private readonly List<TupleElementSyntax> TupleElements = new();
    private readonly TupleElementFilter? filter;

    public TupleElementVisiter(TupleElementFilter? filter = null)
    {
        this.filter = filter;
    }

    public override SyntaxNode VisitTupleElement(TupleElementSyntax node)
    {
        node = (base.VisitTupleElement(node) as TupleElementSyntax)!;
        if (filter?.Check(node) == true) TupleElements.Add(node);

        return node;
    }

    public List<TupleElementSyntax> GetValues(SyntaxTree tree)
    {
        Visit(tree.GetRoot());
        return TupleElements;
    }

    public List<TupleElementSyntax> GetValues(IEnumerable<SyntaxTree> trees)
    {
        foreach (var tree in trees) Visit(tree.GetRoot());
        return TupleElements;
    }
}