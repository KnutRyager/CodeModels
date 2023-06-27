using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeModels.Factory.CodeModelFactory;
using Microsoft.CodeAnalysis.CSharp;
using CodeModels.Factory;
using CodeModels.Execution.Scope;
using CodeModels.Execution.Context;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Access;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Utils;

namespace CodeModels.Models.Primitives.Member.Enum;

public record EnumMember(string Name,
    AttributeListList Attributes,
    IExpression Value,
    bool IsImplicitValue)
    : FieldOrProperty<EnumMemberDeclarationSyntax>(Name, TypeShorthands.Int, Attributes, Modifier.Public | Modifier.Static, Value),
    IEnumMember, IField<EnumMemberExpression>
{
    public static EnumMember Create(string name,
        AttributeListList? attributes = null,
        IExpression? value = null,
        bool? isImplicitValue = null)
        => new(name, attributes ?? AttributesList(), value ?? VoidValue, isImplicitValue is null ? value is null : isImplicitValue.Value);

    public override IInvocation AccessValue(IExpression? instance = null) => new EnumMemberExpression(this, instance, GetScopes(instance));

    public MemberAccessExpression Invoke(IExpression caller) => MemberAccess(this, caller);
    public MemberAccessExpression Invoke(string identifier) => Invoke(CodeModelFactory.Identifier(identifier));
    public MemberAccessExpression Invoke(string identifier, IType? type, ISymbol? symbol) => Invoke(Identifier(identifier, type, symbol));
    public MemberAccessExpression Invoke(string identifier, IType type) => Invoke(Identifier(identifier, type));
    public MemberAccessExpression Invoke(string identifier, ISymbol symbol) => Invoke(Identifier(identifier, symbol: symbol));
    public override IInvocation Invoke(IExpression? caller, IEnumerable<IExpression> _)
        => AccessValue(caller);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        foreach (var attribute in Attributes.Children()) yield return attribute;
    }

    public EnumMemberDeclarationSyntax EnumSyntax() => SyntaxWithModifiers();

    public override EnumMemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => EnumMemberDeclaration(
            Attributes.Syntax(),
            Modifier.None.Syntax(),
            Identifier(Name),
            IsImplicitValue || Value is null || ExpressionUtils.IsVoid(Value) ? null : EqualsValueClause(Value.Syntax()));

    public override IExpression EvaluateAccess(IExpression? expression, ICodeModelExecutionContext context)
    {
        var scopes = GetScopes(expression);
        try
        {
            context.EnterScopes(scopes);
            return context.GetValue(Name);
        }
        finally
        {
            context.ExitScopes(scopes);
        }
    }

    public override void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes)
    {
        try
        {
            context.EnterScopes(scopes);
            Assign(value).Evaluate(context);
        }
        finally
        {
            context.ExitScopes(scopes);
        }
    }

    public EnumMemberExpression Access(IExpression? instance = null) => new(this, instance, GetScopes());
    IFieldExpression IField.Access(IExpression? instance) => Access(instance);

    public AssignmentExpression Assign(IExpression value) => ToIdentifierExpression().Assign(value);
    public override AssignmentExpression Assign(IExpression? caller, IExpression value) => Assignment(
        MemberAccess(caller ?? Owner?.ToIdentifierExpression() ?? throw new NotImplementedException(),
            ToIdentifierExpression()), value);

    public override void Assign(IExpression? instance, IExpression value, ICodeModelExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

