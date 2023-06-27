using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.AbstractCodeModels.Member;

public record AbstractProperty(IType Type, string Name, IExpression Value, Modifier Modifier, bool IsRandomlyGeneratedName, IType? InterfaceType = null, AttributeListList? Attributes = null)
    : MemberModel<MemberDeclarationSyntax>(Type, Attributes ?? AttributesList(), Modifier, Name),
    IMember, ITypeModel, IAssignable, INamedValue,
    IToArgumentConvertible, IToArgumentListConvertible
{
    public AbstractProperty(IType type, string? name, IExpression? expression = null, Modifier? modifier = Modifier.Public, IBaseTypeDeclaration? owner = null, IType? interfaceType = null)
            : this(type, name ?? Guid.NewGuid().ToString(), expression ?? VoidValue, modifier ?? Modifier.Public, name is null, interfaceType)
    {
        Owner = owner;
    }

    public AbstractProperty(IExpression expression, string? name = null, Modifier modifier = Modifier.Public, ITypeDeclaration? owner = null, IType? interfaceType = null)
            : this(expression.Get_Type(), name, expression, modifier, owner, interfaceType) { }

    public override Parameter ToParameter() => Param(Name, Type, Value);
    public override Argument ToArgument() => Arg(Value);

    public override ParameterSyntax ToParameterSyntax() => Parameter(
            attributeLists: default,
            modifiers: default,
            type: TypeSyntax(),
            identifier: IdentifierSyntax(),
            @default: Initializer());

    public override MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifiers = Modifier.None, Modifier removeModifier = Modifier.None) => PropertyOrFieldDeclarationCustom(
            propertyType: modifiers.SetModifiers(Modifier),
            attributeLists: default,
            modifiers: modifiers.SetModifiers(Modifier).SetFlags(removeModifier, false).Syntax(),
            type: DeclarationTypeSyntax(),
            explicitInterfaceSpecifier: default,
            identifier: IdentifierSyntax(),
            accessorList: AccessorListCustom(new AccessorDeclarationSyntax[] { }).
                AddAccessors(!modifiers.SetModifiers(Modifier).IsField() ? new[] {AccessorDeclarationGetCustom(attributeLists: default,
                        modifiers: default,
                        body: default,
                        expressionBody: default) } : new AccessorDeclarationSyntax[] { })
                .AddAccessors(modifiers.SetModifiers(Modifier).IsWritable() ? new[] {AccessorDeclarationSetCustom(attributeLists: default,
                        modifiers: default,
                        body: default,
                        expressionBody: default) } : new AccessorDeclarationSyntax[] { }),
            expressionBody: default,
            initializer: Initializer());

    public override TupleElementSyntax ToTupleElement() => TupleElement(type: TypeSyntax(), identifier: TupleNameIdentifier(IsRandomlyGeneratedName ? null : Name));
    public override IExpression ToExpression() => Value;

    public override SimpleNameSyntax NameSyntax() => Name is null ? throw new Exception($"Attempted to get name from property without name: '{ToString()}'") : IdentifierName(Name);
    public AbstractPropertyExpression AccessValue(IExpression? instance = null) => new(this, instance);
    public AbstractPropertyExpression AccessValue(string identifier, IType? type = null, ISymbol? symbol = null) => AccessValue(Identifier(identifier, type, symbol));
    public ExpressionSyntax? AccessSyntax(IExpression? instance = null) => Owner is null && instance is null ? NameSyntax()
        : MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, instance is null ? IdentifierName(Owner!.Name) : IdentifierName(instance.Syntax().ToString()), Token(SyntaxKind.DotToken), NameSyntax());

    public ExpressionSyntax? ExpressionSyntax => Value switch
    {
        _ when ReferenceEquals(Value, VoidValue) => default,
        _ => Value.Syntax()
    };
    public ExpressionSyntax? DefaultValueSyntax() => ExpressionSyntax;

    public TypeSyntax DeclarationTypeSyntax() => (InterfaceType ?? Type).Syntax();

    public EqualsValueClauseSyntax? Initializer() => DefaultValueSyntax() switch
    {
        ExpressionSyntax expression => EqualsValueClause(expression),
        _ => default
    };

    public PropertyExpression GetAccess(IExpression? instance) => new(ToProperty(), instance);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Value is not null) yield return Value;
    }

    public virtual IExpression EvaluateAccess(ICodeModelExecutionContext context, IExpression? instance)
    {
        throw new NotImplementedException();
    }

    public virtual void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes)
    {
        throw new NotImplementedException();
    }

    public override IType ToType() => Type;

    public override CodeModel<MemberDeclarationSyntax> Render(Namespace @namespace)
    {
        throw new NotImplementedException();
    }

    public Property ToProperty() => Property(Type, Name, null, Value, Modifier, Attributes);
}
