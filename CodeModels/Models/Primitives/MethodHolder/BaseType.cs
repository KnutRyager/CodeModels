using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataStructures;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using CodeModels.Factory;
using CodeModels.Execution.Scope;
using CodeModels.Execution.Context;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public abstract record BaseType<T>(string Name,
    List<IMember> Members,
    Namespace? Namespace,
    Modifier TopLevelModifier,
    Modifier MemberModifier,
    Type? ReflectedType)
    : NamedCodeModel<T>(Name),
    ITypeDeclaration<T>,
    IScopeHolder
    where T : TypeDeclarationSyntax

{
    private CodeModelExecutionScope? _staticScope;
    private List<Field>? _fields;
    private List<PropertyModel>? _properties;
    private List<Method>? _methods;
    private List<Constructor>? _constructors;

    public BaseType(string name, IEnumerable<IMember>? members = null,
        Namespace? @namespace = null, Modifier topLevelModifier = Modifier.Public,
        Modifier memberModifier = Modifier.Public, Type? type = null)
        : this(name, List(members), @namespace, topLevelModifier, memberModifier, ReflectedType: type)
    {
        InitOwner();
    }

    protected void InitOwner()
    {
        InitMembers();
        foreach (var member in Members)
        {
            if (member is IFieldOrProperty fieldOrProperty)
                fieldOrProperty.Owner = this;
        }
    }

    private void InitMembers()
    {
        _fields ??= new List<Field>();
        _properties ??= new List<PropertyModel>();
        _methods ??= new List<Method>();
        _constructors ??= new List<Constructor>();
        var newMembers = new List<IMember>();
        foreach (var member in Members)
        {
            InitMember(member, newMembers);
        }
        Members.AddRange(newMembers);
        foreach (var method in _methods)
        {
            if (((IMember)method).IsStatic) continue;
            //scope.DefineVariable(((INamedValue)method).Name);
            //scope.SetValue(((INamedValue)method).Name, method);
        }
    }

    private IMember InitMember(IMember member, List<IMember>? newMembers = null)
    {
        newMembers ??= Members;
        switch (member)
        {
            case Field field:
                _fields!.Add(field);
                break;
            case PropertyModel property:
                {
                    _properties!.Add(property);
                    var getter = property.GetAccessor;
                    var hasGetter = getter?.Body is not null || getter?.ExpressionBody is not null;
                    var setter = property.SetAccessor ?? property.InitAccessor;
                    var hasSetter = setter?.Body is not null || setter?.ExpressionBody is not null;
                    if (!(hasGetter && hasSetter) && getter is not null)
                    {
                        var backingField = CodeModelFactory.Field(property.Type,
                            getter.Type.GetBackingFieldName(member.Name), property.Value,
                            member.IsStatic ? PropertyAndFieldTypes.PrivateStaticField : PropertyAndFieldTypes.PrivateField);

                        if (!Members.Any(x => x.Name == backingField.Name))
                        {
                            _fields!.Add(backingField);
                            newMembers.Add(backingField);
                        }
                    }
                    var getterMethod = getter?.GetMethod(property.Name);
                    var setterMethod = setter?.GetMethod(property.Name);
                    if (getterMethod is not null && !_methods.Any(x => ((INamed)x).Name == getterMethod.Name))
                    {
                        _methods!.Add(getterMethod);
                    }
                    if (setterMethod is not null && !_methods.Any(x => ((INamed)x).Name == setterMethod.Name))
                    {
                        _methods!.Add(setterMethod);
                    }
                    break;
                }
            case Method method:
                _methods!.Add(method);
                break;
            case Constructor constructor:
                constructor.Owner = this;
                //constructor = constructor with { Owner = this };
                member = constructor;
                _constructors!.Add(constructor);
                break;
        }
        return member;
    }

    protected void InitInstanceScope(CodeModelExecutionScope scope)
    {
        foreach (var field in GetFields())
        {
            scope.DefineVariable(field.Name, field.ValueOrDefault());
        }
        foreach (var property in GetProperties())
        {
            if (property.GetBackingField() is Field backingField)
                scope.DefineAlias(backingField.Name, property.Name);
            // TODO: Methods?
        }
    }

    public CodeModelExecutionScope GetStaticScope()
    {
        if (_staticScope is null)
        {
            _staticScope = new CodeModelExecutionScope(aliases: GetAliases());
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

    private Dictionary<string, string> GetAliases()
    {
        var aliases = new Dictionary<string, string>();
        foreach (var property in GetProperties())
        {
            if (property.GetBackingField() is Field backingField)
                aliases.Add(backingField.Name, property.Name);
            // TODO: Methods?
        }
        return aliases;
    }

    public ITypeDeclaration AddMember(IMember member)
    {
        if (member is Constructor c)
        {
            c.Owner = this;
            //member = c with { Owner = this };
        }
        Members.Add(member);
        if (member is IFieldOrProperty fieldOrProperty)
        {
            if (fieldOrProperty.Owner is not null) throw new ArgumentException($"Adding already owned property '{member}' to '{Name}'.");
            fieldOrProperty.Owner = this;
        }
        InitMember(member);
        return this;
    }

    public ITypeDeclaration AddProperty(Type type, string name) => AddProperty(new TypeFromReflection(type), name);
    public ITypeDeclaration AddProperty(ITypeSymbol type, string name) => AddProperty(new TypeFromSymbol(type), name);
    public ITypeDeclaration AddProperty(AbstractType type, string name) => AddMember(CodeModelFactory.PropertyModel(name, type));
    public IType Get_Type() => Type(this);
    public TypeSyntax TypeSyntax() => Get_Type().Syntax();

    public List<IMember> AllMembers() => Members.OrderBy(x => x.Modifier, new ModifierComparer()).Concat<IMember>(Methods()).ToList();
    public List<IMember> GetReadonlyMembers() => Members.Where(x => x.Modifier.IsWritable()).ToList();
    public SyntaxList<MemberDeclarationSyntax> MethodsSyntax() => SyntaxFactory.List<MemberDeclarationSyntax>(Methods().Select(x => x.ToMethodSyntax(MemberModifier)));
    public List<Constructor> GetConstructors() => _constructors is { Count: > 0 } ? _constructors : new List<Constructor>() { CodeModelFactory.ConstructorFull(this, CodeModelFactory.NamedValues(), CodeModelFactory.Block()) };
    public List<Field> GetFields() => _fields ??= new List<Field>();
    public List<PropertyModel> GetProperties() => _properties ??= new List<PropertyModel>();
    public List<IFieldOrProperty> GetPropertiesAndFields() => GetFields().Concat<IFieldOrProperty>(GetProperties()).ToList();
    public List<Method> Methods()
    {
        if (_methods is null)
        {
            InitMembers();
        }
        return _methods!;
    }
    public SyntaxList<MemberDeclarationSyntax> MembersSyntax() => SyntaxFactory.List(Members.Select(x => x.Syntax()).Concat(MethodsSyntax()));

    public virtual IMember GetMember(string name) => AllMembers().First(x => x.Name == name);
    public virtual IMember TryGetMember(string name) => AllMembers().FirstOrDefault(x => x.Name == name);
    public virtual Field GetField(string name) => GetFields().First(x => x.Name == name);
    public virtual Field? TryGetField(string name) => GetFields().FirstOrDefault(x => x.Name == name);
    public virtual PropertyModel GetProperty(string name) => GetProperties().First(x => x.Name == name);
    public virtual PropertyModel? TryGetProperty(string name) => GetProperties().FirstOrDefault(x => x.Name == name);
    public virtual Constructor GetConstructor() => GetConstructors().First();


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

    public List<IMember> Ordered(Modifier modifier = Modifier.None) => Members.OrderBy(x => x.Modifier, new ModifierComparer()).ToList();

    public virtual IMember this[string name] => Members.FirstOrDefault(x => x.Name == name) ?? Methods().FirstOrDefault(x => ((IMember)x).Name == name) ?? throw new ArgumentException($"No member '{name}' found in {Name}.");
    public IMethod GetMethod(string name) => Methods().First(x => ((IMember)x).Name == name);

    public bool IsStatic => TopLevelModifier.HasFlag(Modifier.Static);

    public Modifier Modifier => Modifier.None;

    public string TypeName => Name;

    public bool Required => false;

    public bool IsMulti => false;

    public EqualityList<IType> GenericTypes => new EqualityList<IType>();

    public bool IsLiteralExpression => false;

    public LiteralExpressionSyntax? LiteralSyntax() => null;

    public object? LiteralValue() => null;

    TypeDeclarationSyntax ITypeDeclaration.Syntax() => Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var member in Members) yield return member;
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

    public abstract InstantiatedObject CreateInstance();

    BaseTypeDeclarationSyntax IBaseTypeDeclaration.Syntax() => Syntax();

    public IList<ICodeModelExecutionScope> GetScopes(ICodeModelExecutionContext context)
        => new[] { GetStaticScope() };
}
