using System;
using CodeAnalyzation.Models.ProgramModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace CodeAnalyzation.Models;

public abstract record Expression<T>(IType Type, ISymbol? Symbol = null, IProgramContext? Context = null) : CodeModel<T>(Context), IExpression<T> where T : ExpressionSyntax
{
    public Expression(IType type) : this(type, null) { }

    // TODO: Containing type for prop
    public Expression(ISymbol symbol) : this(new TypeFromSymbol(symbol), symbol) { }

    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => EnumMemberDeclaration(
            attributeLists: default,
            identifier: Type switch
            {
                _ when Type.GetReflectedType() == typeof(string) && LiteralValue is string name => Identifier(name),
                _ => throw new ArgumentException($"Unhandled enum name: '{LiteralValue}' of type '{Type}'.")
            },
            equalsValue: value is null ? default! : EqualsValueClause(LiteralExpressionCustom(value)));

    public ArgumentSyntax ToArgument() => ArgumentCustom(
            nameColon: default,
            refKindKeyword: default,
            expression: Syntax());


    public virtual object? LiteralValue => null;
    public bool IsLiteralExpression => LiteralSyntax is not null;
    public virtual LiteralExpressionSyntax? LiteralSyntax => default;
    //public virtual ExpressionSyntax Syntax => 

    //public override T Syntax() => 
    ExpressionSyntax IExpression.Syntax() => Syntax();
    protected ExpressionSyntax PlanBSyntax() => (ExpressionSyntax?)LiteralSyntax ?? (Symbol is not null ? IdentifierName(Symbol.Name) : ReferenceEquals(this, CodeModelFactory.NullValue) ? IdentifierName("null") : throw new Exception("Expression has no syntax node or value."));
}
