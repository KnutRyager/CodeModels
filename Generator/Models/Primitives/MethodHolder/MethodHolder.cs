using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public abstract record MethodHolder<T>(string Name, PropertyCollection Properties, List<IMethod> Methods,
            Namespace? Namespace = null, Modifier TopLevelModifier = Modifier.None,
            Modifier MemberModifier = Modifier.None, bool IsStatic = false, Type? Type = null) : CodeModel<T>, IMethodHolder<T> where T : BaseTypeDeclarationSyntax
    {
        public MethodHolder(string name, PropertyCollection? properties = null, IEnumerable<IMethod>? methods = null,
            Namespace? @namespace = null, Modifier topLevelModifier = Modifier.None,
            Modifier memberModifier = Modifier.None, Type? type = null)
            : this(name, properties ?? new PropertyCollection(name: name), methods?.ToList() ?? new(), @namespace, topLevelModifier, memberModifier, Type: type)
        {
            foreach (var property in Properties.Properties) property.Owner = this;
        }

        public IMethodHolder AddProperty(Property property)
        {
            Properties.Properties.Add(property);
            if (property.Owner is not null) throw new ArgumentException($"Adding already owned property '{property}' to '{Name}'.");
            property.Owner = this;
            return this;
        }

        public IMethodHolder AddProperty(Type type, string name) => AddProperty(new TypeFromReflection(type), name);
        public IMethodHolder AddProperty(ITypeSymbol type, string name) => AddProperty(new TypeFromSymbol(type), name);
        public IMethodHolder AddProperty(AbstractType type, string name) => AddProperty(new(type, name));

        public List<Property> GetReadonlyProperties() => Properties.Properties.Where(x => x.Modifier.IsWritable()).ToList();
        public SyntaxList<MemberDeclarationSyntax> MethodsSyntax() => List<MemberDeclarationSyntax>(Methods.Select(x => x.ToMethodSyntax(MemberModifier)));
        public List<IMember> Members() => Properties.Ordered().Concat<IMember>(Methods).ToList(); // TODO: MemberModifier
        public SyntaxList<MemberDeclarationSyntax> MembersSyntax() => List(Properties.ToMembers(MemberModifier).Concat(MethodsSyntax()));

        public RecordDeclarationSyntax ToRecord() => RecordDeclarationCustom(
                attributeLists: default,
                modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Syntax(),
                identifier: Identifier(Name),
                typeParameterList: default,
                parameterList: Properties.ToParameters(),
                baseList: default,
                constraintClauses: default,
                members: MethodsSyntax());

        public ClassDeclarationSyntax ToClass() => ClassDeclarationCustom(
                attributeLists: default,
                modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Syntax(),
                identifier: Identifier(Name),
                typeParameterList: default,
                baseList: default,
                constraintClauses: default,
                members: MembersSyntax());

        public StructDeclarationSyntax ToStruct() => StructDeclarationCustom(
                attributeLists: default,
                modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Syntax(),
                identifier: Identifier(Name),
                typeParameterList: default,
                baseList: default,
                constraintClauses: default,
                members: MembersSyntax());

        public InterfaceDeclarationSyntax ToInterface() => InterfaceDeclarationCustom(
                attributeLists: default,
                modifiers: TopLevelModifier.Syntax(),
                identifier: Identifier(Name),
                typeParameterList: default,
                baseList: default,
                constraintClauses: default,
                members: List(Members().Where(x => x.Modifier.HasFlag(Modifier.Public)).Select(x => x.ToMemberSyntax(removeModifier: Modifier.Public))));

        public IMember this[string name] => (IMember)Properties[name] ?? Methods.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentException($"No member '{name}' found in {Name}.");
        public Property GetProperty(string name) => Properties[name];
        public IMethod GetMethod(string name) => Methods.First(x => x.Name == name);

        BaseTypeDeclarationSyntax IMethodHolder.Syntax() => Syntax();
    }
}