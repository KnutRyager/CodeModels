using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models;

public record ParenthesizedVariableDesignation(List<IVariableDesignation> Variables)
    : VariableDesignation<ParenthesizedVariableDesignationSyntax>
{
    public static ParenthesizedVariableDesignation Create(IEnumerable<IVariableDesignation>? variables = null)
        => new(List(variables));

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var variable in Variables) yield return variable;
    }

    public override ParenthesizedVariableDesignationSyntax Syntax()
        => SyntaxFactory.ParenthesizedVariableDesignation(SyntaxFactory.SeparatedList(Variables.Select(x => x.Syntax()).ToList()));
}