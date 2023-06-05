using System;
using System.Collections.Generic;
using System.Reflection;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models.Primitives.Member;

public record PropertyExpressionFromReflection(PropertyInfo Property, IExpression? Instance = null, IList<ICodeModelExecutionScope>? Scopes = null, ISymbol? Symbol = null)
    : Expression<ExpressionSyntax>(Type(Property.PropertyType), Symbol),
    IPropertyExpression
{
    IProperty IPropertyExpression.Property => throw new NotImplementedException();
    public IBaseTypeDeclaration? Owner => throw new NotImplementedException();

    public static PropertyExpressionFromReflection Create(PropertyInfo property, IExpression? caller)
        => new(property, caller);

    public override ExpressionSyntax Syntax()
    => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
        Instance?.Syntax() ?? Identifier(Property.DeclaringType.Name).Syntax(),
        SyntaxFactory.IdentifierName(Property.Name));

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context) => throw new NotImplementedException();
    public override IdentifierExpression ToIdentifierExpression() => Instance is IdentifierExpression idetifier ? idetifier : base.ToIdentifierExpression();
    public void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes) => throw new NotImplementedException();
    //public void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes) => Property.SetMethod.Invoke(value, context, scopes);
}
