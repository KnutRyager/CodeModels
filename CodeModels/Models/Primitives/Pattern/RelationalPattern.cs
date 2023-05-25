using System.Collections.Generic;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record RelationalPattern(SyntaxToken Operator, IExpression Expression)
    : Pattern<RelationalPatternSyntax>
{
    public static RelationalPattern Create(SyntaxToken @operator, IExpression expression)
        => new(@operator, expression);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public override RelationalPatternSyntax Syntax()
        => SyntaxFactory.RelationalPattern(Operator, Expression.Syntax());
}