using System;
using System.Text.RegularExpressions;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public record Property(IType Type, string Name, IExpression? Value, Modifier Modifier, bool IsRandomlyGeneratedName)
        : CodeModel<CSharpSyntaxNode>, IMember, ITypeModel
    {
        public IMethodHolder? Owner { get; set; }

        public Property(IType type, string? name, IExpression? expression = null, Modifier? modifier = Modifier.Public, IMethodHolder? owner = null)
                : this(type, name ?? Guid.NewGuid().ToString(), expression, modifier ?? Modifier.Public, name is null)
        {
            Owner = owner;
        }

        public Property(PropertyDeclarationSyntax property, Modifier? modifier = null) : this(Type(property.Type), property.Identifier.ToString(), Expression(property.Initializer?.Value), modifier) { }
        public Property(TupleElementSyntax element, Modifier? modifier = null) : this(Type(element.Type), element.Identifier.ToString(), modifier: modifier) { }
        public Property(ParameterSyntax parameter, Modifier? modifier = null) : this(Type(parameter.Type), parameter.Identifier.ToString(), Expression(parameter.Default?.Value), modifier) { }
        public Property(ITypeSymbol typeSymbol, string name, ExpressionSyntax? expression = null, Modifier? modifier = null) : this(new TypeFromSymbol(typeSymbol), name, Expression(expression), modifier) { }
        public Property(TypeSyntax type, string name, ExpressionSyntax? expression = null, Modifier? modifier = null) : this(Type(type), name, Expression(expression), modifier) { }
        public Property(ITypeSymbol typeSymbol, string name, string? value = null, Modifier? modifier = null) : this(new TypeFromSymbol(typeSymbol), name, value is null ? null : new LiteralExpression(value), modifier) { }

        public ParameterSyntax ToParameter() => Parameter(
                attributeLists: default,
                modifiers: default,
                type: TypeSyntax(),
                identifier: Identifier(Name!),
                @default: Initializer());

        public MemberDeclarationSyntax ToMemberSyntax(Modifier modifiers = Modifier.None, Modifier removeModifier = Modifier.None) => PropertyOrFieldDeclarationCustom(
                propertyType: modifiers.SetModifiers(Modifier),
                attributeLists: default,
                modifiers: modifiers.SetModifiers(Modifier).SetFlags(removeModifier, false).Syntax(),
                type: TypeSyntax(),
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
        public PropertyExpression AccessValue(string identifier) => AccessValue(new LiteralExpression(identifier));
        public ExpressionSyntax? AccessSyntax(IExpression? instance = null) => Owner is null ? NameSyntax
            : MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, instance is null ? IdentifierName(Owner.Name) : IdentifierName(instance.Syntax().ToString()), Token(SyntaxKind.DotToken), NameSyntax);

        public ExpressionSyntax? ExpressionSyntax => Value?.Syntax();
        public ExpressionSyntax? DefaultValueSyntax() => ExpressionSyntax ?? Value switch
        {
            _ when Value?.IsLiteralExpression == true => Value.LiteralSyntax,
            _ => default
        };

        public TypeSyntax TypeSyntax() => Type.TypeSyntax();
        public override CSharpSyntaxNode Syntax() => ToMemberSyntax();

        public EqualsValueClauseSyntax? Initializer() => DefaultValueSyntax() switch
        {
            ExpressionSyntax expression => EqualsValueClause(expression),
            _ => default
        };

        public PropertyExpression GetAccess(IExpression? instance) => new(this, instance);

        private SyntaxToken TupleNameIdentifier(string? name) => name == default || new Regex("Item+[1-9]+[0-9]*").IsMatch(name) ? default : Identifier(name);
    }
}
