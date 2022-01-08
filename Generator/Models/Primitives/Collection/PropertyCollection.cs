using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeAnalyzation.Collectors;
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
        public PropertyCollection(IEnumerable<PropertyInfo> properties) : this(properties.Select(x => new PropertyFromReflection(x))) { }
        public PropertyCollection(IEnumerable<FieldInfo> fields) : this(fields.Select(x => new PropertyFromField(x))) { }
        public PropertyCollection(Type type) : this(type.GetProperties(), type.GetFields()) { }
        public PropertyCollection(IEnumerable<PropertyInfo> properties, IEnumerable<FieldInfo> fields)
            : this(properties.Select(x => new PropertyFromReflection(x)).ToList<Property>().Concat(fields.Select(x => new PropertyFromField(x)))) { }
        public PropertyCollection(ClassDeclarationSyntax declaration) : this(new PropertyVisiter().GetEntries(declaration.SyntaxTree).Select(x => new Property(x)), declaration.Identifier.ToString()) { }
        public PropertyCollection(RecordDeclarationSyntax declaration) : this(new ParameterVisiter().GetEntries(declaration.SyntaxTree).Select(x => new Property(x)), declaration.Identifier.ToString()) { }
        public PropertyCollection(TupleTypeSyntax declaration) : this(new TupleElementVisiter().GetEntries(declaration.SyntaxTree).Select(x => new Property(x))) { }
        public PropertyCollection(MethodDeclarationSyntax declaration) : this(declaration.ParameterList) { }
        public PropertyCollection(ConstructorDeclarationSyntax declaration) : this(declaration.ParameterList) { }
        public PropertyCollection(ParameterListSyntax parameters) : this(parameters.Parameters.Select(x => new Property(x))) { }
        public PropertyCollection(IEnumerable<ParameterInfo> parameters) : this(parameters.Select(x => new PropertyFromParameter(x))) { }

        public ClassDeclarationSyntax ToClass(string? name = null, Modifier modifiers = Modifier.Public, Modifier memberModifiers = Modifier.Public) => ClassDeclarationCustom(
                attributeLists: default,
                modifiers: modifiers.Syntax(),
                identifier: Identifier(name ?? Name ?? throw new ArgumentException("No identifier")),
                typeParameterList: default,
                baseList: default,
                constraintClauses: default,
                members: ToMembers(memberModifiers)
            );

        public RecordDeclarationSyntax ToRecord(string? name = null, Modifier modifiers = Modifier.Public) => RecordDeclarationCustom(
                attributeLists: default,
                modifiers: modifiers.Syntax(),
                identifier: name != null ? Identifier(name) : Identifier,
                typeParameterList: default,
                parameterList: ToParameters(),
                baseList: default,
                constraintClauses: default,
                members: default);

        public TupleTypeSyntax ToTuple() => TupleType(SeparatedList(Properties.Select(x => x.ToTupleElement())));
        public ParameterListSyntax ToParameters() => ParameterListCustom(Properties.Select(x => x.ToParameter()));
        public List<Property> Ordered(Modifier modifier = Modifier.None) => Properties.OrderBy(x => x, new PropertyComparer(modifier)).ToList();
        public SyntaxList<MemberDeclarationSyntax> ToMembers(Modifier modifier = Modifier.None) => List(Ordered(modifier).Select(x => x.ToMemberSyntax(modifier)));
        public List<Property> FilterValues() => Properties.Where(x => x.Value != null).ToList();
        public ExpressionCollection ToValueCollection() => new(FilterValues().Select(x => x.Value ?? throw new Exception($"Property '{x}' contains no value.")));

        public SyntaxToken Identifier => Identifier(Name ?? throw new ArgumentException("No identifier"));
        public Property this[string name] => Properties.First(x => x.Name == name);
        public Property? TryFindProperty(string name) => Properties.FirstOrDefault(x => x.Name == name);
    }
}

