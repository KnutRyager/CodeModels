using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record SingleVariableDesignation(string Identifier)
    : VariableDesignation<SingleVariableDesignationSyntax>
{
    public static SingleVariableDesignation Create(string identifier) => new(identifier);

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override SingleVariableDesignationSyntax Syntax()
        => SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier(Identifier));

}