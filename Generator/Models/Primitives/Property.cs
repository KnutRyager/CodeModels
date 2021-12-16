using System;
using System.Text.RegularExpressions;
using CodeAnalyzation.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public class Property
    {
        public string? Name { get; set; }
        public TType Type { get; set; }
        public Expression? Value { get; set; }
        public PropertyType PropertyType { get; set; } = PropertyType.PublicReadWrite;

        public Property(TType type, string? name, Expression? expression = null, PropertyType? propertyType = null)
        {
            Type = type;
            Name = name;
            Value = expression;
            PropertyType = propertyType ?? PropertyType;
        }

        public Property(PropertyDeclarationSyntax property) : this(TType.Parse(property.Type), property.Identifier.ToString(), new Expression(property.Initializer?.Value)) { }
        public Property(TupleElementSyntax element) : this(TType.Parse(element.Type), element.Identifier.ToString()) { }
        public Property(ParameterSyntax parameter) : this(TType.Parse(parameter.Type), parameter.Identifier.ToString(), new Expression(parameter.Default?.Value)) { }
        public Property(ITypeSymbol typeSymbol, string name, ExpressionSyntax? expression = null) : this(new TType(typeSymbol), name, new Expression(expression)) { }
        public Property(ITypeSymbol typeSymbol, string name, string? value = null) : this(new TType(typeSymbol), name, Expression.FromValue(value)) { }

        public static Property FromValue(TType type, string? name, Value? value = null, PropertyType? propertyType = null) => new(type, name, value == null ? null : new Expression(value), propertyType);
        public static Property FromExpression(TType type, string? name, ExpressionSyntax expression, PropertyType? propertyType = null) => new(type, name, new Expression(expression), propertyType);
        public static Property FromQualifiedName(TType type, string? name, string qualifiedName, PropertyType? propertyType = null) => new(type, name, Expression.FromQualifiedName(qualifiedName), propertyType);

        public static Property Parse(ArgumentSyntax argument) => argument.Expression switch
        {
            TypeSyntax type => new(TType.Parse(type), default),
            DeclarationExpressionSyntax declaration => Parse(declaration),
            _ => throw new ArgumentException($"Can't parse {nameof(Property)} from '{argument}'.")
        };

        public static Property Parse(DeclarationExpressionSyntax declaration) => new(TType.Parse(declaration.Type), declaration.Designation switch
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

        public MemberDeclarationSyntax ToProperty(PropertyType? type = null) => PropertyOrFieldDeclarationCustom(
                propertyType: PropertyType,
                attributeLists: default,
                modifiers: (type ?? PropertyType).Modifiers(),
                type: TypeSyntax(),
                explicitInterfaceSpecifier: default,
                identifier: Identifier(Name!),
                accessorList: AccessorListCustom(new AccessorDeclarationSyntax[] { }).
                    AddAccessors(!(type ?? PropertyType).IsField() ? new[] {AccessorDeclarationGetCustom(attributeLists: default,
                        modifiers: default,
                        body: default,
                        expressionBody: default) } : new AccessorDeclarationSyntax[] { })
                    .AddAccessors((type ?? PropertyType).IsWritable() ? new[] {AccessorDeclarationSetCustom(attributeLists: default,
                        modifiers: default,
                        body: default,
                        expressionBody: default) } : new AccessorDeclarationSyntax[] { }),
                expressionBody: default,
                initializer: Initializer());

        public TupleElementSyntax ToTupleElement() => TupleElement(type: TypeSyntax(), identifier: TupleNameIdentifier(Name));

        public ExpressionSyntax? ExpressionSyntax => Value?.ExpressionSyntax;
        public ExpressionSyntax? DefaultValueSyntax() => ExpressionSyntax ?? Value switch
        {
            _ when Value?.IsLiteralExpression == true => Value.LiteralExpression,
            _ => default
        };

        public TypeSyntax TypeSyntax() => Type.TypeSyntax();

        public EqualsValueClauseSyntax? Initializer() => DefaultValueSyntax() switch
        {
            ExpressionSyntax expression => EqualsValueClause(expression),
            _ => default
        };

        private SyntaxToken TupleNameIdentifier(string? name) => name == default || new Regex("Item+[1-9]+[0-9]*").IsMatch(name) ? default : Identifier(name);
    }
}
