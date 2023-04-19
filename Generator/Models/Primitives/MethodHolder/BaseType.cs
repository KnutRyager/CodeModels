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

public abstract record BaseType<T>(string Name,
    List<IFieldOrProperty> Members,
    List<IMethod> Methods,
    Namespace? Namespace,
    Modifier TopLevelModifier,
    Modifier MemberModifier,
    Type? ReflectedType)
    : NamedCodeModel<T>(Name),
    IMethodHolder<T> where T : BaseTypeDeclarationSyntax
{
    private ProgramModelExecutionScope? _staticScope;

    public BaseType(string name, IEnumerable<IFieldOrProperty>? members = null, IEnumerable<IMethod>? methods = null,
        Namespace? @namespace = null, Modifier topLevelModifier = Modifier.Public,
        Modifier memberModifier = Modifier.Public, Type? type = null)
        : this(name, List(members), List(methods), @namespace, topLevelModifier, memberModifier, ReflectedType: type)
    {
        InitOwner();
    }

    protected void InitOwner()
    {
        InitMembers();
        foreach (var property in Members) property.Owner = this;
        //foreach (var method in Methods) method.Owner = this;
    }

    private void InitMembers()
    {
        var newMembers = new List<IFieldOrProperty>();
        foreach (var member in Members)
        {
            if (member is PropertyModel property)
            {
                var getter = property.GetAccessor;
                var hasGetter = getter?.Body is not null || getter?.ExpressionBody is not null;
                var setter = property.SetAccessor ?? property.InitAccessor;
                var hasSetter = setter?.Body is not null || setter?.ExpressionBody is not null;
                if (!(hasGetter && hasSetter) && getter is not null)
                {
                    var backingField = CodeModelFactory.FieldModel(property.Type,
                        getter.Type.GetBackingFieldName(member.Name), property.Value,
                        member.IsStatic ? PropertyAndFieldTypes.PrivateStaticField : PropertyAndFieldTypes.PrivateField);

                    if (!Members.Any(x => x.Name == backingField.Name))
                    {
                        newMembers.Add(backingField);
                    }
                }
                var getterMethod = getter?.GetMethod(property.Name);
                var setterMethod = setter?.GetMethod(property.Name);
                if (getterMethod is not null && !Methods.Any(x => ((INamedValue)x).Name == getterMethod.Name))
                {
                    Methods.Add(getterMethod);
                }
                if (setterMethod is not null && !Methods.Any(x => ((INamedValue)x).Name == setterMethod.Name))
                {
                    Methods.Add(setterMethod);
                }
            }
        }
        Members.AddRange(newMembers);
        foreach (var method in Methods)
        {
            if (((IMember)method).IsStatic) continue;
            //scope.DefineVariable(((INamedValue)method).Name);
            //scope.SetValue(((INamedValue)method).Name, method);
        }
    }

    protected void InitInstanceScope(ProgramModelExecutionScope scope)
    {
        foreach (var field in GetFields())
        {
            scope.DefineVariable(field.Name, field.ValueOrDefault());
        }
    }

    public ProgramModelExecutionScope GetStaticScope()
    {
        if (_staticScope is null)
        {
            _staticScope = new ProgramModelExecutionScope();
            foreach (var field in GetFields())
            {
                if (field.IsStatic)
                {
                    _staticScope.DefineVariable(field.Name, field.ValueOrDefault());
                }
            }

        }
        return _staticScope;
    }

    public IMethodHolder AddProperty(IFieldOrProperty property)
    {
        Members.Add(property);
        if (property.Owner is not null) throw new ArgumentException($"Adding already owned property '{property}' to '{Name}'.");
        property.Owner = this;
        return this;
    }

    public IMethodHolder AddProperty(Type type, string name) => AddProperty(new TypeFromReflection(type), name);
    public IMethodHolder AddProperty(ITypeSymbol type, string name) => AddProperty(new TypeFromSymbol(type), name);
    public IMethodHolder AddProperty(AbstractType type, string name) => AddProperty(CodeModelFactory.PropertyModel(name, type));
    public IType Get_Type() => Type(Name);
    public TypeSyntax TypeSyntax() => Get_Type().Syntax();

    public List<IMember> AllMembers() => Members.OrderBy(x => x.Modifier, new ModifierComparer()).Concat<IMember>(Methods).ToList();
    public List<IFieldOrProperty> GetReadonlyMembers() => Members.Where(x => x.Modifier.IsWritable()).ToList();
    public SyntaxList<MemberDeclarationSyntax> MethodsSyntax() => SyntaxFactory.List<MemberDeclarationSyntax>(Methods.Select(x => x.ToMethodSyntax(MemberModifier)));
    public List<FieldModel> GetFields() => Members.Where(x => x is FieldModel).Select(x => (x as FieldModel)!).ToList();
    public List<PropertyModel> GetProperties() => Members.Where(x => x is PropertyModel).Select(x => (x as PropertyModel)!).ToList();
    public SyntaxList<MemberDeclarationSyntax> MembersSyntax() => SyntaxFactory.List(Members.Select(x => x.Syntax()).Concat(MethodsSyntax()));

    public virtual IMember GetMember(string name) => AllMembers().First(x => x.Name == name);
    public virtual IMember TryGetMember(string name) => AllMembers().FirstOrDefault(x => x.Name == name);
    public virtual FieldModel GetField(string name) => GetFields().First(x => x.Name == name);
    public virtual FieldModel? TryGetField(string name) => GetFields().FirstOrDefault(x => x.Name == name);
    public virtual PropertyModel GetPropery(string name) => GetProperties().First(x => x.Name == name);
    public virtual PropertyModel? TryGetPropery(string name) => GetProperties().FirstOrDefault(x => x.Name == name);


    public RecordDeclarationSyntax ToRecord() => RecordDeclarationCustom(
            attributeLists: default,
            modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Syntax(),
            identifier: ToIdentifier(),
            typeParameterList: default,
            parameterList: ParameterList(SeparatedList(GetProperties().Select(x => x.ToParameter()))),
            baseList: default,
            constraintClauses: default,
            members: MethodsSyntax());

    public ClassDeclarationSyntax ToClass() => ClassDeclarationCustom(
            attributeLists: default,
            modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Syntax(),
            identifier: ToIdentifier(),
            typeParameterList: default,
            baseList: default,
            constraintClauses: default,
            members: MembersSyntax());

    public TupleTypeSyntax ToTupleType() => TupleType(
        SeparatedList(GetFields().Select(x => x.ToTupleElement())));

    public StructDeclarationSyntax ToStruct() => StructDeclarationCustom(
            attributeLists: default,
            modifiers: Modifier.Public.SetModifiers(TopLevelModifier).Syntax(),
            identifier: ToIdentifier(),
            typeParameterList: default,
            baseList: default,
            constraintClauses: default,
            members: MembersSyntax());

    public InterfaceDeclarationSyntax ToInterface() => InterfaceDeclarationCustom(
            attributeLists: default,
            modifiers: TopLevelModifier.Syntax(),
            identifier: ToIdentifier(),
            typeParameterList: default,
            baseList: default,
            constraintClauses: default,
            members: SyntaxFactory.List(Members.Where(x => x.Modifier.HasFlag(Modifier.Public)).Select(x => x.SyntaxWithModifiers(removeModifier: Modifier.Public))));

    public List<IFieldOrProperty> Ordered(Modifier modifier = Modifier.None) => Members.OrderBy(x => x.Modifier, new ModifierComparer()).ToList();

    public virtual IMember this[string name] => Members.FirstOrDefault(x => x.Name == name) as IMember ?? Methods.FirstOrDefault(x => ((IMember)x).Name == name) ?? throw new ArgumentException($"No member '{name}' found in {Name}.");
    public IFieldOrProperty GetProperty(string name) => Members.First(x => x.Name == name);
    public IMethod GetMethod(string name) => Methods.First(x => ((IMember)x).Name == name);

    public bool IsStatic => TopLevelModifier.HasFlag(Modifier.Static);

    public Modifier Modifier => Modifier.None;

    BaseTypeDeclarationSyntax IMethodHolder.Syntax() => Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var member in Members) yield return member;
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

    public IExpression ToExpression() => ToIdentifierExpression();

    public ParameterSyntax ToParameter()
    {
        throw new NotImplementedException();
    }

    public TupleElementSyntax ToTupleElement()
    {
        throw new NotImplementedException();
    }
}
