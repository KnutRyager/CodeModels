using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;
using Common.DataStructures;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.AbstractCodeModels.Member;

public abstract record AbstractBaseTypeDeclaration<T, TSyntax>(string Name, NamedValueCollection Properties, List<IMethod> Methods,
        Namespace? Namespace, Modifier TopLevelModifier,
        Modifier MemberModifier, Type? ReflectedType)
    : AbstractAbstractCodeModel<T, TSyntax>(),
    IBaseTypeDeclaration<TSyntax>,
    IAbstractCodeModel<T, TSyntax> where T : ICodeModel<TSyntax>
    where TSyntax : BaseTypeDeclarationSyntax
{
    public AbstractBaseTypeDeclaration(string name, NamedValueCollection? properties = null, IEnumerable<IMethod>? methods = null,
        Namespace? @namespace = null, Modifier topLevelModifier = Modifier.Public,
        Modifier memberModifier = Modifier.Public, Type? type = null)
        : this(name, NamedValues(properties), List(methods), @namespace, topLevelModifier, memberModifier, ReflectedType: type)
    {
        foreach (var property in Properties.Properties) property.Owner = this;
    }

    public IBaseTypeDeclaration AddProperty(AbstractProperty property)
    {
        Properties.Properties.Add(property);
        if (property.Owner is not null) throw new ArgumentException($"Adding already owned property '{property}' to '{Name}'.");
        property.Owner = this;
        return this;
    }

    public IBaseTypeDeclaration AddProperty(Type type, string name) => AddProperty(TypeFromReflection.Create(type), name);
    public IBaseTypeDeclaration AddProperty(ITypeSymbol type, string name) => AddProperty(TypeFromSymbol.Create(type), name);
    public IBaseTypeDeclaration AddProperty(AbstractType type, string name) => AddProperty(new(type, name));
    public IType Get_Type() => Type(Name);
    public TypeSyntax TypeSyntax() => Get_Type().Syntax();

    public List<AbstractProperty> GetReadonlyProperties() => Properties.Properties.Where(x => x.Modifier.IsWritable()).ToList();
    public SyntaxList<MemberDeclarationSyntax> MethodsSyntax() => SyntaxFactory.List<MemberDeclarationSyntax>(Methods.Select(x => x.ToMethodSyntax(MemberModifier)));
    public List<IMember> Members() => Properties.Ordered(MemberModifier).Concat<IMember>(Methods).ToList();
    public SyntaxList<MemberDeclarationSyntax> MembersSyntax() => SyntaxFactory.List(Properties.ToMembers(MemberModifier).Concat(MethodsSyntax()));

    public RecordDeclarationSyntax ToRecord(string? name = null, Modifier? modifiers = null) => RecordDeclarationCustom(
            attributeLists: default,
            modifiers: modifiers?.Syntax() ?? Modifier.Public.SetModifiers(TopLevelModifier).Syntax(),
            identifier: name is not null ? Identifier(name) : IdentifierSyntax(),
            typeParameterList: default,
            parameterList: Properties.ToParameters(),
            baseList: default,
            constraintClauses: default,
            members: MethodsSyntax());

    public ClassDeclarationSyntax ToClass(string? name = null, Modifier? modifiers = null, Modifier memberModifiers = Modifier.Public) => ClassDeclarationCustom(
            attributeLists: default,
            modifiers: modifiers?.Syntax() ?? Modifier.Public.SetModifiers(TopLevelModifier).Syntax(),
            identifier: name is not null ? Identifier(name) : IdentifierSyntax(),
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

    public EnumDeclarationSyntax ToEnum() => EnumDeclaration(
            attributeLists: default,
            modifiers: TopLevelModifier.Syntax(),
            identifier: Identifier(Name),
            baseList: default,
            members: SeparatedList(GetEnumMembers().Select(x => x.EnumSyntax())));

    public List<IEnumMember> GetEnumMembers() => Members().Select(x => (x as IEnumMember)!).Where(x => x is not null).ToList();

    public IMember this[string name] => (IMember)Properties[name] ?? Methods.FirstOrDefault(x => ((IMember)x).Name == name) ?? throw new ArgumentException($"No member '{name}' found in {Name}.");
    public AbstractProperty GetProperty(string name) => Properties[name];
    public IMethod GetMethod(string name) => Methods.First(x => ((IMember)x).Name == name);

    public bool IsStatic => TopLevelModifier.HasFlag(Modifier.Static);

    public Modifier Modifier => throw new NotImplementedException();

    //List<IMember> IBaseTypeDeclaration.Members => Members.Select(x => x as IMember).ToList();
    List<IMember> IBaseTypeDeclaration.Members => Members();

    public EqualityList<IType> GenericTypes => throw new NotImplementedException();

    public bool IsLiteralExpression => throw new NotImplementedException();

    public bool IsMulti => throw new NotImplementedException();

    public bool Required => throw new NotImplementedException();

    public string TypeName => throw new NotImplementedException();

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var property in Properties.Properties) yield return property;
        foreach (var method in Methods) yield return method;
    }

    MemberDeclarationSyntax IMember.Syntax() => ToClass();

    public MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => (this with { TopLevelModifier = Modifier.SetModifiers(modifier).SetFlags(removeModifier, false) }).Syntax() as TSyntax;

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

    public Parameter ToParameter()
    {
        throw new NotImplementedException();
    }

    public ParameterSyntax ToParameterSyntax()
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

    public abstract IInstantiatedObject CreateInstance();

    public IBaseTypeDeclaration AddMember(IMember member)
    {
        throw new NotImplementedException();
    }

    public List<IMember> AllMembers()
    {
        throw new NotImplementedException();
    }

    public void Deconstruct(out string Name, out List<IMember> Members, out Namespace? Namespace, out Modifier TopLevelModifier, out Modifier MemberModifier, out Type? ReflectedType)
    {
        throw new NotImplementedException();
    }

    public Constructor GetConstructor()
    {
        throw new NotImplementedException();
    }

    public List<Constructor> GetConstructors()
    {
        throw new NotImplementedException();
    }

    public IField GetField(string name)
    {
        throw new NotImplementedException();
    }

    public List<IField> GetFields()
    {
        throw new NotImplementedException();
    }

    public IMember GetMember(string name)
    {
        throw new NotImplementedException();
    }

    public List<Property> GetProperties()
    {
        throw new NotImplementedException();
    }

    public List<IFieldOrProperty> GetPropertiesAndFields()
    {
        throw new NotImplementedException();
    }

    Property IBaseTypeDeclaration.GetProperty(string name)
    {
        throw new NotImplementedException();
    }

    public List<IMember> GetReadonlyMembers()
    {
        throw new NotImplementedException();
    }

    public IList<ICodeModelExecutionScope> GetScopes(ICodeModelExecutionContext context)
    {
        throw new NotImplementedException();
    }

    public CodeModelExecutionScope GetStaticScope()
    {
        throw new NotImplementedException();
    }

    public LiteralExpressionSyntax? LiteralSyntax()
    {
        throw new NotImplementedException();
    }

    public object? LiteralValue()
    {
        throw new NotImplementedException();
    }

    public List<IMember> Ordered(Modifier modifier = Modifier.None)
    {
        throw new NotImplementedException();
    }

    public TupleTypeSyntax ToTupleType()
    {
        throw new NotImplementedException();
    }

    public IField? TryGetField(string name)
    {
        throw new NotImplementedException();
    }

    public IMember TryGetMember(string name)
    {
        throw new NotImplementedException();
    }

    public Property? TryGetProperty(string name)
    {
        throw new NotImplementedException();
    }

    TSyntax IBaseTypeDeclaration<TSyntax>.Syntax()
    {
        throw new NotImplementedException();
    }

    public IdentifierExpression ToIdentifierExpression()
    {
        throw new NotImplementedException();
    }

    public IdentifierNameSyntax IdentifierNameSyntax()
    {
        throw new NotImplementedException();
    }

    public SyntaxToken IdentifierSyntax() => Identifier(Name);

    public SimpleNameSyntax NameSyntax()
    {
        throw new NotImplementedException();
    }

    BaseTypeDeclarationSyntax IBaseTypeDeclaration.Syntax()
    {
        throw new NotImplementedException();
    }
}
