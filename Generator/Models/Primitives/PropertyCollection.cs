using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models
{
    public class PropertyCollection
    {
        public List<Property> Properties { get; set; }
        public string? Name { get; set; }

        public PropertyCollection(IEnumerable<Property>? properties = null, string? name = null)
        {
            Properties = properties?.ToList() ?? new List<Property>();
            Name = name;
        }

        public PropertyCollection(ClassDeclarationSyntax declaration) : this(new PropertyVisiter().GetValues(declaration.SyntaxTree).Select(x => new Property(x)), declaration.Identifier.ToString()) { }
        public PropertyCollection(RecordDeclarationSyntax declaration) : this(new ParameterVisiter().GetValues(declaration.SyntaxTree).Select(x => new Property(x)), declaration.Identifier.ToString()) { }
        public PropertyCollection(TupleTypeSyntax declaration) : this(new TupleElementVisiter().GetValues(declaration.SyntaxTree).Select(x => new Property(x))) { }
        public PropertyCollection(MethodDeclarationSyntax declaration) : this(declaration.ParameterList) { }
        public PropertyCollection(ParameterListSyntax parameters) : this(parameters.Parameters.Select(x => new Property(x))) { }

        public static PropertyCollection Parse(string code) => code.Parse().Members.FirstOrDefault() switch
        {
            ClassDeclarationSyntax declaration => new PropertyCollection(declaration),
            RecordDeclarationSyntax declaration => new PropertyCollection(declaration),
            GlobalStatementSyntax statement => Parse(statement),
            _ => throw new ArgumentException($"Can't parse {nameof(PropertyCollection)} from '{code}'.")
        };

        public static PropertyCollection Parse(GlobalStatementSyntax statement) => statement.Statement switch
        {
            ExpressionStatementSyntax expression => Parse(expression.Expression),
            _ => throw new ArgumentException($"Can't parse {nameof(PropertyCollection)} from '{statement}'.")
        };

        public static PropertyCollection Parse(ExpressionSyntax expression) => expression switch
        {
            TupleExpressionSyntax declaration => Parse(declaration.Arguments),
            TupleTypeSyntax declaration => new PropertyCollection(declaration),
            _ => throw new ArgumentException($"Can't parse {nameof(PropertyCollection)} from '{expression}'.")
        };

        public static PropertyCollection Parse(IEnumerable<ArgumentSyntax> arguments) => new(arguments.Select(x => Property.Parse(x)));

        public ClassDeclarationSyntax ToClass(string? name = null) => ClassDeclarationCustom(
                attributeLists: default,
                modifiers: TokenList(Token(SyntaxKind.PublicKeyword)),
                identifier: Identifier(name ?? Name ?? throw new ArgumentException("No identifier")),
                typeParameterList: default,
                baseList: default,
                constraintClauses: default,
                members: ToMembers()
            );

        public RecordDeclarationSyntax ToRecord(string? name = null) => RecordDeclarationCustom(
                attributeLists: default,
                modifiers: TokenList(Token(SyntaxKind.PublicKeyword)),
                identifier: name != null ? Identifier(name) : Identifier,
                typeParameterList: default,
                parameterList: ToParameters(),
                baseList: default,
                constraintClauses: default,
                members: default);

        public TupleTypeSyntax ToTuple() => TupleType(SeparatedList(Properties.Select(x => x.ToTupleElement())));
        public ParameterListSyntax ToParameters() => ParameterListCustom(Properties.Select(x => x.ToParameter()));
        public SyntaxList<MemberDeclarationSyntax> ToMembers(Modifier modifier = Modifier.None) => List(Properties.OrderBy(x => x, new PropertyComparer()).Select(x => x.ToProperty(modifier)));
        public List<Property> FilterValues() => Properties.Where(x => x.Value != null).ToList();
        public ValueCollection ToValueCollection() => new(FilterValues().Select(x => x.Value?.Value ?? throw new Exception($"Property '{x}' contains no value.")));

        public SyntaxToken Identifier => Identifier(Name ?? throw new ArgumentException("No identifier"));

    }
}

