using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record DiscardDesignation() 
    : VariableDesignation<DiscardDesignationSyntax>
{
    public static DiscardDesignation Create() => new();
    
    public override IEnumerable<ICodeModel> Children()
    {
        yield break;
    }

    public override DiscardDesignationSyntax Syntax()
        => SyntaxFactory.DiscardDesignation();

}