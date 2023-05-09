using System;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public abstract record BaseTypeDeclaration<T>(string Name, PropertyCollection Properties, List<IMethod> Methods,
        Namespace? Namespace, Modifier TopLevelModifier,
        Modifier MemberModifier, Type? ReflectedType)
    : NamedCodeModel<T>(Name),
    IBaseTypeDeclaration<T> where T : BaseTypeDeclarationSyntax
{
    public BaseTypeDeclaration(string name, PropertyCollection? properties = null, IEnumerable<IMethod>? methods = null,
        Namespace? @namespace = null, Modifier topLevelModifier = Modifier.Public,
        Modifier memberModifier = Modifier.Public, Type? type = null)
        : this(name, PropertyCollection(properties), List(methods), @namespace, topLevelModifier, memberModifier, ReflectedType: type)
    {
        foreach (var property in Properties.Properties) property.Owner = this;
    }

    public IBaseTypeDeclaration AddProperty(Property property)
    {
        Properties.Properties.Add(property);
        if (property.Owner is not null) throw new ArgumentException($"Adding already owned property '{property}' to '{Name}'.");
        property.Owner = this;
        return this;
    }

    public IBaseTypeDeclaration AddProperty(Type type, string name) => AddProperty(new TypeFromReflection(type), name);
    public IBaseTypeDeclaration AddProperty(ITypeSymbol type, string name) => AddProperty(new TypeFromSymbol(type), name);
    public IBaseTypeDeclaration AddProperty(AbstractType type, string name) => AddProperty(new(type, name));
    public IType Get_Type() => Type(Name);
    public TypeSyntax TypeSyntax() => Get_Type().Syntax();

    public List<Property> GetReadonlyProperties() => Properties.Properties.Where(x => x.Modifier.IsWritable()).ToList();
    public SyntaxList<MemberDeclarationSyntax> MethodsSyntax() => SyntaxFactory.List<MemberDeclarationSyntax>(Methods.Select(x => x.ToMethodSyntax(MemberModifier)));
    public List<IMember> Members() => Properties.Ordered(MemberModifier).Concat<IMember>(Methods).ToList();
    public SyntaxList<MemberDeclarationSyntax> MembersSyntax() => SyntaxFactory.List(Properties.ToMembers(MemberModifier).Concat(MethodsSyntax()));

    public RecordDeclarationSyntax ToRecord() => RecordDeclarationCustom(
            attributeLists: default,
            modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Syntax(),
            identifier: IdentifierSyntax(),
            typeParameterList: default,
            parameterList: Properties.ToParameters(),
            baseList: default,
            constraintClauses: default,
            members: MethodsSyntax());

    public ClassDeclarationSyntax ToClass() => ClassDeclarationCustom(
            attributeLists: default,
            modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Syntax(),
            identifier: IdentifierSyntax(),
            typeParameterList: default,
            baseList: default,
            constraintClauses: default,
            members: MembersSyntax());

    public StructDeclarationSyntax ToStruct() => StructDeclarationCustom(
            attributeLists: default,
            modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Syntax(),
            identifier: IdentifierSyntax(),
            typeParameterList: default,
            baseList: default,
            constraintClauses: default,
            members: MembersSyntax());

    public InterfaceDeclarationSyntax ToInterface() => InterfaceDeclarationCustom(
            attributeLists: default,
            modifiers: TopLevelModifier.Syntax(),
            identifier: IdentifierSyntax(),
            typeParameterList: default,
            baseList: default,
            constraintClauses: default,
            members: SyntaxFactory.List(Members().Where(x => x.Modifier.HasFlag(Modifier.Public)).Select(x => x.SyntaxWithModifiers(removeModifier: Modifier.Public))));

    public IMember this[string name] => (IMember)Properties[name] ?? Methods.FirstOrDefault(x => ((IMember)x).Name == name) ?? throw new ArgumentException($"No member '{name}' found in {Name}.");
    public Property GetProperty(string name) => Properties[name];
    public IMethod GetMethod(string name) => Methods.First(x => ((IMember)x).Name == name);

    public bool IsStatic => TopLevelModifier.HasFlag(Modifier.Static);

    public Modifier Modifier => throw new NotImplementedException();

    List<IMember> IBaseTypeDeclaration.Members => throw new NotImplementedException();

    BaseTypeDeclarationSyntax IBaseTypeDeclaration.Syntax() => Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var property in Properties.Properties) yield return property;
        foreach (var method in Methods) yield return method;
    }

    MemberDeclarationSyntax IMember.Syntax() => ToClass();

    public MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => (this with { TopLevelModifier = Modifier.SetModifiers(modifier).SetFlags(removeModifier, false) }).Syntax();

    public ICodeModel Render(Namespace @namespace)
    {
        throw new NotImplementedException();
    }

    public IType ToType()
    {
        throw new NotImplementedException();
    }

    public IExpression ToExpression()
    {
        throw new NotImplementedException();
    }

    public ParameterSyntax ToParameter()
    {
        throw new NotImplementedException();
    }

    public TupleElementSyntax ToTupleElement()
    {
        throw new NotImplementedException();
    }

    List<Method> IBaseTypeDeclaration.Methods()
    {
        throw new NotImplementedException();
    }

    public abstract InstantiatedObject CreateInstance();
}
