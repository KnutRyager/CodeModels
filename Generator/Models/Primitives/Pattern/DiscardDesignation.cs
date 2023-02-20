using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record DiscardDesignation() 
    : VariableDesignation<DiscardDesignationSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield break;
    }

    public override DiscardDesignationSyntax Syntax()
        => SyntaxFactory.DiscardDesignation();

}