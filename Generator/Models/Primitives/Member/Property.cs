using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record Property(IType Type, string Name, IExpression Value, Modifier Modifier, bool IsRandomlyGeneratedName, IType? InterfaceType = null, List<AttributeList>? Attributes = null)
    : MemberModel<MemberDeclarationSyntax>(Name, Type, Attributes ?? new List<AttributeList>(), Modifier), IMember, ITypeModel
{
    public IMethodHolder? Owner { get; set; }

    public Property(IType type, string? name, IExpression? expression = null, Modifier? modifier = Modifier.Public, IMethodHolder? owner = null, IType? interfaceType = null)
            : this(type, name ?? Guid.NewGuid().ToString(), expression ?? VoidValue, modifier ?? Modifier.Public, name is null, interfaceType)
    {
        Owner = owner;
    }

    public Property(IExpression expression, string? name = null, Modifier modifier = Modifier.Public, IMethodHolder? owner = null, IType? interfaceType = null)
            : this(expression.Get_Type(), name, expression, modifier, owner, interfaceType) { }

    public Property(PropertyDeclarationSyntax property, Modifier modifier = Modifier.None, IType? interfaceType = null)
        : this(Type(property.Type), property.Identifier.ToString(), Expression(property.Initializer?.Value, Type(property.Type)), Modifiers(property.Modifiers).SetModifiers(modifier), interfaceType: interfaceType) { }
    public Property(TupleElementSyntax element, Modifier? modifier = null, IType? interfaceType = null)
        : this(Type(element.Type), element.Identifier.ToString(), modifier: modifier, interfaceType: interfaceType) { }
    public Property(ParameterSyntax parameter, Modifier? modifier = null, IType? interfaceType = null)
        : this(Type(parameter.Type), parameter.Identifier.ToString(), Expression(parameter.Default?.Value, Type(parameter.Type)), modifier, interfaceType: interfaceType) { }
    public Property(ITypeSymbol typeSymbol, string name, ExpressionSyntax? expression = null, Modifier? modifier = null, IType? interfaceType = null)
        : this(new TypeFromSymbol(typeSymbol), name, Expression(expression, new TypeFromSymbol(typeSymbol)), modifier, interfaceType: interfaceType) { }
    public Property(TypeSyntax type, string name, ExpressionSyntax? expression = null, Modifier? modifier = null, TypeSyntax? interfaceType = null)
        : this(Type(type), name, Expression(expression, Type(type)), modifier, interfaceType: Type(interfaceType)) { }
    public Property(ITypeSymbol typeSymbol, string name, string? value = null, Modifier? modifier = null, IType? interfaceType = null)
        : this(new TypeFromSymbol(typeSymbol), name, value is null ? null : new LiteralExpression(value), modifier, interfaceType: interfaceType) { }

    public ParameterSyntax ToParameter() => Parameter(
            attributeLists: default,
            modifiers: default,
            type: TypeSyntax(),
            identifier: Identifier(Name!),
            @default: Initializer());

    public override MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifiers = Modifier.None, Modifier removeModifier = Modifier.None) => PropertyOrFieldDeclarationCustom(
            propertyType: modifiers.SetModifiers(Modifier),
            attributeLists: default,
            modifiers: modifiers.SetModifiers(Modifier).SetFlags(removeModifier, false).Syntax(),
            type: DeclarationTypeSyntax(),
            explicitInterfaceSpecifier: default,
            identifier: Identifier(Name!),
            accessorList: AccessorListCustom(new AccessorDeclarationSyntax[] { }).
                AddAccessors(!(modifiers.SetModifiers(Modifier)).IsField() ? new[] {AccessorDeclarationGetCustom(attributeLists: default,
                        modifiers: default,
                        body: default,
                        expressionBody: default) } : new AccessorDeclarationSyntax[] { })
                .AddAccessors(modifiers.SetModifiers(Modifier).IsWritable() ? new[] {AccessorDeclarationSetCustom(attributeLists: default,
                        modifiers: default,
                        body: default,
                        expressionBody: default) } : new AccessorDeclarationSyntax[] { }),
            expressionBody: default,
            initializer: Initializer());

    public TupleElementSyntax ToTupleElement() => TupleElement(type: TypeSyntax(), identifier: TupleNameIdentifier(IsRandomlyGeneratedName ? null : Name));

    public SimpleNameSyntax NameSyntax => Name is null ? throw new Exception($"Attempted to get name from property without name: '{ToString()}'") : IdentifierName(Name);
    public PropertyExpression AccessValue(IExpression? instance = null) => new(this, instance);
    public PropertyExpression AccessValue(string identifier, IType? type = null, ISymbol? symbol = null) => AccessValue(Identifier(identifier, type, symbol));
    public ExpressionSyntax? AccessSyntax(IExpression? instance = null) => Owner is null && instance is null ? NameSyntax
        : MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, instance is null ? IdentifierName(Owner!.Name) : IdentifierName(instance.Syntax().ToString()), Token(SyntaxKind.DotToken), NameSyntax);

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

    public PropertyExpression GetAccess(IExpression? instance) => new(this, instance);

    private SyntaxToken TupleNameIdentifier(string? name) => name == default || new Regex("Item+[1-9]+[0-9]*").IsMatch(name) ? default : Identifier(name);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Value is not null) yield return Value;
    }

    public virtual IExpression EvaluateAccess(IProgramModelExecutionContext context, IExpression instance)
    {
        throw new NotImplementedException();
    }
}
