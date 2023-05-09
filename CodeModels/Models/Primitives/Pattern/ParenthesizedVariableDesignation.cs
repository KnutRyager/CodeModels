using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record ParenthesizedVariableDesignation(List<IVariableDesignation> Variables) 
    : VariableDesignation<ParenthesizedVariableDesignationSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var variable in Variables) yield return variable;
    }

    public override ParenthesizedVariableDesignationSyntax Syntax()
        => SyntaxFactory.ParenthesizedVariableDesignation(SyntaxFactory.SeparatedList(Variables.Select(x => x.Syntax()).ToList()));
}