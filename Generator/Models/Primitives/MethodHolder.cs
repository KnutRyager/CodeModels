using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public class MethodHolder : IMethodHolder
    {
        public PropertyCollection Properties { get; set; }
        public List<Property> ValueProperties => Properties.FilterValues();
        public List<Method> Methods { get; set; }
        public string Name { get; set; }
        public Namespace? Namespace { get; set; }
        public Modifier TopLevelModifier { get; set; }
        public Modifier MemberModifier { get; set; }
        public bool IsStatic { get; protected set; }

        public MethodHolder(string name, PropertyCollection? properties = null, IEnumerable<Method>? methods = null,
            Namespace? @namespace = null, Modifier topLevelModifier = Modifier.None, Modifier memberModifier = Modifier.None)
        {
            Name = name;
            Properties = properties ?? new PropertyCollection(name: name);
            foreach (var property in Properties.Properties) property.Owner = this;
            Namespace = @namespace;
            Methods = methods?.ToList() ?? new List<Method>();
            TopLevelModifier = topLevelModifier;
            MemberModifier = memberModifier;
        }

        public MethodHolder AddProperty(Property property)
        {
            Properties.Properties.Add(property);
            if (property.Owner is not null) throw new ArgumentException($"Adding already owned property '{property}' to '{Name}'.");
            property.Owner = this;
            return this;
        }

        public MethodHolder AddProperty(Type type, string name) => AddProperty(new TType(type), name);
        public MethodHolder AddProperty(ITypeSymbol type, string name) => AddProperty(new TType(type), name);
        public MethodHolder AddProperty(TType type, string name) => AddProperty(new(type, name));

        public List<Property> GetReadonlyProperties() => Properties.Properties.Where(x => x.Modifier.IsWritable()).ToList();
        public SyntaxList<MemberDeclarationSyntax> GetMethods() => List<MemberDeclarationSyntax>(Methods.Select(x => x.ToMethod()));
        public SyntaxList<MemberDeclarationSyntax> Members() => List(Properties.ToMembers(MemberModifier).Concat(GetMethods()));

        public RecordDeclarationSyntax ToRecord() => RecordDeclarationCustom(
                attributeLists: default,
                modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Modifiers(),
                identifier: Identifier(Name),
                typeParameterList: default,
                parameterList: Properties.ToParameters(),
                baseList: default,
                constraintClauses: default,
                members: GetMethods());

        public ClassDeclarationSyntax ToClass() => ClassDeclarationCustom(
                attributeLists: default,
                modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Modifiers(),
                identifier: Identifier(Name),
                typeParameterList: default,
                baseList: default,
                constraintClauses: default,
                members: Members());

        public StructDeclarationSyntax ToStruct() => StructDeclarationCustom(
                attributeLists: default,
                modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Modifiers(),
                identifier: Identifier(Name),
                typeParameterList: default,
                baseList: default,
                constraintClauses: default,
                members: Members());

        public IMember this[string name] => (IMember)Properties[name] ?? Methods.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentException($"No member '{name}' found in {Name}.");
        public Property GetProperty(string name) => Properties[name];
        public Method GetMethod(string name) => Methods.First(x => x.Name == name);
    }
}