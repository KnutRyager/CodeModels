using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models;

public record ListPattern(List<IPattern> Patterns, IVariableDesignation? Designation)
    : Pattern<ListPatternSyntax>
{
    public static ListPattern Create(IEnumerable<IPattern>? patterns = null, IVariableDesignation? designation = null)
        => new(List(patterns), designation);

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override ListPatternSyntax Syntax()
        => SyntaxFactory.ListPattern(SyntaxFactory.SeparatedList(Patterns.Select(x => x.Syntax())), Designation?.Syntax());
}