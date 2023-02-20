using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public abstract record StatementExpression(IStatement Statement, ISymbol? Symbol = null)
    : IExpression
{
    public bool IsLiteralExpression => false;
    public LiteralExpressionSyntax? LiteralSyntax => null;
    public object? LiteralValue => null;
    public virtual ExpressionStatement AsStatement() => new(Statement);
    public IEnumerable<ICodeModel> Children() => Statement.Children();
    public string Code() => Statement.Code();
    public ISet<IType> Dependencies(ISet<IType>? set = null) => Statement.Dependencies(set);
    public virtual IExpression Evaluate(IProgramModelExecutionContext context) => throw new NotImplementedException();
    public virtual object? EvaluatePlain(IProgramModelExecutionContext context) => throw new NotImplementedException();
    public virtual IdentifierExpression GetIdentifier() => throw new NotImplementedException();
    public virtual IType Get_Type() => Statement.Get_Type();
    public ExpressionSyntax Syntax() => throw new NotImplementedException();
    ExpressionOrPatternSyntax IExpressionOrPattern.Syntax() => throw new NotImplementedException();
    public ArgumentSyntax ToArgument() => throw new NotImplementedException();
    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => throw new NotImplementedException();
    CSharpSyntaxNode ICodeModel.Syntax() => Statement.Syntax();
}
