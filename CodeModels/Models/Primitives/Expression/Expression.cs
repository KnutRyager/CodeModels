using System;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace CodeModels.Models;

public abstract record Expression<T>(IType Type, ISymbol? Symbol = null, string? Name = null)
    : NamedCodeModel<T>(Name ?? Type.Name), IExpression<T> where T : ExpressionSyntax
{
    public Expression(IType type) : this(type, null) { }

    // TODO: Containing type for prop
    public Expression(ITypeSymbol symbol) : this(new TypeFromSymbol(symbol), symbol) { }

    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => EnumMemberDeclaration(
            attributeLists: default,
            identifier: Type switch
            {
                _ when Type.GetReflectedType() == typeof(string) && LiteralValue() is string name => SyntaxFactory.Identifier(name),
                _ => throw new ArgumentException($"Unhandled enum name: '{LiteralValue()}' of type '{Type}'.")
            },
            equalsValue: value is null ? default! : EqualsValueClause(LiteralExpressionCustom(value)));

    public ArgumentSyntax ToArgument() => ArgumentCustom(
            nameColon: default,
            refKindKeyword: default,
            expression: Syntax());


    public virtual object? LiteralValue() => null;
    public bool IsLiteralExpression => LiteralSyntax() is not null;
    public virtual LiteralExpressionSyntax? LiteralSyntax() => default;
    public IType Get_Type() => Type;
    public TypeSyntax TypeSyntax() => Get_Type().Syntax();

    ExpressionSyntax IExpression.Syntax() => Syntax();
    ExpressionOrPatternSyntax IExpressionOrPattern.Syntax() => Syntax();
    protected ExpressionSyntax PlanBSyntax() => (ExpressionSyntax?)LiteralSyntax() ?? (Symbol is not null ? IdentifierName(Symbol.Name) : ReferenceEquals(this, CodeModelFactory.NullValue) ? IdentifierName("null") : throw new Exception("Expression has no syntax node or value."));

    public abstract IExpression Evaluate(ICodeModelExecutionContext context);
    public virtual object? EvaluatePlain(ICodeModelExecutionContext context) => Evaluate(context).LiteralValue();
    public ExpressionStatement AsStatement() => new(this);
    public new virtual IdentifierExpression ToIdentifierExpression() => new(Type.Name, Type, Symbol: Symbol, Model: this);
}
