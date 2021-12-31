using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeAnalyzation.Collectors;
using CodeAnalyzation.Parsing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public record PropertyCollection(List<Property> Properties, string? Name = null)
    {
        public PropertyCollection(IEnumerable<Property>? properties = null, string? name = null) : this(properties?.ToList() ?? new(), name) { }

        public PropertyCollection(ClassDeclarationSyntax declaration) : this(new PropertyVisiter().GetValues(declaration.SyntaxTree).Select(x => new Property(x)), declaration.Identifier.ToString()) { }
        public PropertyCollection(RecordDeclarationSyntax declaration) : this(new ParameterVisiter().GetValues(declaration.SyntaxTree).Select(x => new Property(x)), declaration.Identifier.ToString()) { }
        public PropertyCollection(TupleTypeSyntax declaration) : this(new TupleElementVisiter().GetValues(declaration.SyntaxTree).Select(x => new Property(x))) { }
        public PropertyCollection(MethodDeclarationSyntax declaration) : this(declaration.ParameterList) { }
        public PropertyCollection(ParameterListSyntax parameters) : this(parameters.Parameters.Select(x => new Property(x))) { }
        public PropertyCollection(IEnumerable<ParameterInfo> parameters) : this(parameters.Select(x => new PropertyFromParameter(x))) { }

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
        public List<Property> Ordered(Modifier modifier = Modifier.None) => Properties.OrderBy(x => x, new PropertyComparer()).ToList();
        public SyntaxList<MemberDeclarationSyntax> ToMembers(Modifier modifier = Modifier.None) => List(Ordered().Select(x => x.ToMemberSyntax(modifier)));
        public List<Property> FilterValues() => Properties.Where(x => x.Value != null).ToList();
        public ExpressionCollection ToValueCollection() => new(FilterValues().Select(x => x.Value ?? throw new Exception($"Property '{x}' contains no value.")));

        public SyntaxToken Identifier => Identifier(Name ?? throw new ArgumentException("No identifier"));
        public Property this[string name] => Properties.First(x => x.Name == name);
        public Property? TryFindProperty(string name) => Properties.FirstOrDefault(x => x.Name == name);
    }
}

