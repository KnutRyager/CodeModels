using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public class MethodHolder
    {
        public PropertyCollection Properties { get; set; }
        public List<Property> ValueProperties => Properties.FilterValues();
        public List<Method> Methods { get; set; }
        public string Name { get; set; }
        public Namespace? Namespace { get; set; }
        public Modifier TopLevelModifier { get; set; }
        public Modifier MemberModifier { get; set; }

        public MethodHolder(string name, PropertyCollection? properties = null, IEnumerable<Method>? methods = null,
            Namespace? @namespace = null, Modifier topLevelModifier = Modifier.None, Modifier memberModifier = Modifier.None)
        {
            Name = name;
            Properties = properties ?? new PropertyCollection(name: name);
            Namespace = @namespace;
            Methods = methods?.ToList() ?? new List<Method>();
            TopLevelModifier = topLevelModifier;
            MemberModifier = memberModifier;
        }

        public MethodHolder AddProperty(TType type, string name)
        {
            Properties.Properties.Add(new(type, name));
            return this;
        }

        public MethodHolder AddProperty(Type type, string name) => AddProperty(new TType(type), name);
        public MethodHolder AddProperty(ITypeSymbol type, string name) => AddProperty(new TType(type), name);

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
    }
}