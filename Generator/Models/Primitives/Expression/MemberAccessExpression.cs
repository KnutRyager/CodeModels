using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record MemberAccessExpression(IExpression Expression, string Name, IType? Type = null) : Expression<MemberAccessExpressionSyntax>(Type ?? TypeShorthands.NullType)  // TODO: Type from semantic analysis
{
    public override MemberAccessExpressionSyntax Syntax()
        => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Expression.Syntax(), Token(SyntaxKind.DotToken), IdentifierName(Name));
    public IdentifierNameSyntax Syntax(string name) => IdentifierName(name);
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context) => throw new NotImplementedException();
}
