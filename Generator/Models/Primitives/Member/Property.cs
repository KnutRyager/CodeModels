using System;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public record Property(IType Type, string Name, Expression? Value, Modifier Modifier, bool IsRandomlyGeneratedName) : IMember, ICodeModel
    {
        public MethodHolder? Owner { get; set; }

        public Property(IType type, string? name, Expression? expression = null, Modifier? modifier = Modifier.Public, MethodHolder? owner = null)
                : this(type, name ?? Guid.NewGuid().ToString(), expression, modifier ?? Modifier.Public, name is null)
        {
            Owner = owner;
        }

        public Property(PropertyDeclarationSyntax property, Modifier? modifier = null) : this(AbstractType.Parse(property.Type), property.Identifier.ToString(), Expression.FromSyntax(property.Initializer?.Value), modifier) { }
        public Property(TupleElementSyntax element, Modifier? modifier = null) : this(AbstractType.Parse(element.Type), element.Identifier.ToString(), modifier: modifier) { }
        public Property(ParameterSyntax parameter, Modifier? modifier = null) : this(AbstractType.Parse(parameter.Type), parameter.Identifier.ToString(), Expression.FromSyntax(parameter.Default?.Value), modifier) { }
        public Property(ITypeSymbol typeSymbol, string name, ExpressionSyntax? expression = null, Modifier? modifier = null) : this(new TypeFromSymbol(typeSymbol), name, Expression.FromSyntax(expression), modifier) { }
        public Property(TypeSyntax type, string name, ExpressionSyntax? expression = null, Modifier? modifier = null) : this(AbstractType.Parse(type), name, Expression.FromSyntax(expression), modifier) { }
        public Property(ITypeSymbol typeSymbol, string name, string? value = null, Modifier? modifier = null) : this(new TypeFromSymbol(typeSymbol), name, value is null ? null : new LiteralExpression(value), modifier) { }

        public static Property FromValue(AbstractType type, string name, Expression? value = null, Modifier? modifier = null) => new(type, name, value, modifier);
        public static Property FromExpression(AbstractType type, string name, ExpressionSyntax expression, Modifier? modifier = null) => new(type, name, new ExpressionFromSyntax(expression), modifier);
        public static Property FromQualifiedName(AbstractType type, string name, string qualifiedName, Modifier? modifier = null) => new(type, name, new ExpressionFromSyntax(qualifiedName), modifier);
        public static Property FromName(string name) => new(TypeShorthands.NullType, name);

        public static Property Parse(ArgumentSyntax argument) => argument.Expression switch
        {
            TypeSyntax type => new(AbstractType.Parse(type), argument.NameColon?.Name.ToString()),
            DeclarationExpressionSyntax declaration => Parse(declaration),
            _ => throw new ArgumentException($"Can't parse {nameof(Property)} from '{argument}'.")
        };

        public static Property Parse(DeclarationExpressionSyntax declaration) => new(AbstractType.Parse(declaration.Type), declaration.Designation switch
        {
            null => default,
            SingleVariableDesignationSyntax designation => designation.Identifier.ToString(),
            _ => throw new ArgumentException($"Can't parse {nameof(Property)} from '{declaration}'.")
        });

        public ParameterSyntax ToParameter() => Parameter(
                attributeLists: default,
                modifiers: default,
                type: TypeSyntax(),
                identifier: Identifier(Name!),
                @default: Initializer());

        public MemberDeclarationSyntax ToProperty(Modifier modifiers = Modifier.None) => PropertyOrFieldDeclarationCustom(
                propertyType: Modifier.SetModifiers(modifiers),
                attributeLists: default,
                modifiers: Modifier.SetModifiers(modifiers).Modifiers(),
                type: TypeSyntax(),
                explicitInterfaceSpecifier: default,
                identifier: Identifier(Name!),
                accessorList: AccessorListCustom(new AccessorDeclarationSyntax[] { }).
                    AddAccessors(!(Modifier.SetModifiers(modifiers)).IsField() ? new[] {AccessorDeclarationGetCustom(attributeLists: default,
                        modifiers: default,
                        body: default,
                        expressionBody: default) } : new AccessorDeclarationSyntax[] { })
                    .AddAccessors(Modifier.SetModifiers(modifiers).IsWritable() ? new[] {AccessorDeclarationSetCustom(attributeLists: default,
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
            : MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, instance is null ? IdentifierName(Owner.Name) : IdentifierName(instance.Syntax.ToString()), Token(SyntaxKind.DotToken), NameSyntax);

        public ExpressionSyntax? ExpressionSyntax => Value?.Syntax;
        public ExpressionSyntax? DefaultValueSyntax() => ExpressionSyntax ?? Value switch
        {
            _ when Value?.IsLiteralExpression == true => Value.LiteralSyntax,
            _ => default
        };

        public TypeSyntax TypeSyntax() => Type.TypeSyntax();

        public EqualsValueClauseSyntax? Initializer() => DefaultValueSyntax() switch
        {
            ExpressionSyntax expression => EqualsValueClause(expression),
            _ => default
        };

        public PropertyExpression GetAccess(IExpression? instance) => new(this, instance);

        private SyntaxToken TupleNameIdentifier(string? name) => name == default || new Regex("Item+[1-9]+[0-9]*").IsMatch(name) ? default : Identifier(name);
    }
}
