using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record RecursivePattern(IType Type)
    : Pattern<RecursivePatternSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override RecursivePatternSyntax Syntax()
        => SyntaxFactory.RecursivePattern(Type.Syntax(), null, null, null);
        //public static RecursivePatternSyntax RecursivePattern(
        //TypeSyntax? type,
        //PositionalPatternClauseSyntax? positionalPatternClause,
        //PropertyPatternClauseSyntax? propertyPatternClause,
        //VariableDesignationSyntax? designation)
}