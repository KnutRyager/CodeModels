using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record SingleVariableDesignation(string Identifier) 
    : VariableDesignation<SingleVariableDesignationSyntax>
{
    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override SingleVariableDesignationSyntax Syntax()
        => SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier(Identifier));

}