using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public abstract record ClassDeclaration(string Name,
    List<IMember> Members,
    ClassDeclaration? Parent = null,
    Namespace? Namespace = null,
    Modifier Modifier = Modifier.Public)
    : BaseType<ClassDeclarationSyntax>(
        Name: Name,
        Members: Members,
        Namespace: Namespace,
        TopLevelModifier: Modifier,
        MemberModifier: Modifier.None,
        ReflectedType: null),
    INamedValueCollection<IFieldOrProperty>,
    IMember
{
    public static ClassDeclaration Create(string name,
    IEnumerable<IMember>? members = null,
    Namespace? @namespace = null,
    Modifier? modifier = null)
    {
        var c = new ClassDeclarationImp(name, List(members), @namespace, modifier ?? Modifier.Public);
        c.InitOwner();
        return c;
    }

    //public ClassDeclaration(IEnumerable<Property>? properties = null, string? name = null, IType? specifiedType = null) : this(List(properties), name, specifiedType) { }
    //public ClassDeclaration(IEnumerable<PropertyInfo> properties) : this(properties.Select(x => new PropertyFromReflection(x))) { }
    //public ClassDeclaration(IEnumerable<FieldInfo> fields) : this(fields.Select(x => new PropertyFromField(x))) { }
    //public ClassDeclaration(Type type) : this(type.GetProperties(), type.GetFields()) { }
    //public ClassDeclaration(IEnumerable<PropertyInfo> properties, IEnumerable<FieldInfo> fields)
    //    : this(properties.Select(x => new PropertyFromReflection(x)).ToList<Property>().Concat(fields.Select(x => new PropertyFromField(x)))) { }
    //public ClassDeclaration(ClassDeclarationSyntax declaration) : this(new PropertyVisiter().GetEntries(declaration.SyntaxTree).Select(x => new Property(x)), declaration.Identifier.ToString()) { }
    //public ClassDeclaration(RecordDeclarationSyntax declaration) : this(new ParameterVisiter().GetEntries(declaration.SyntaxTree).Select(x => new Property(x)), declaration.Identifier.ToString()) { }
    //public ClassDeclaration(TupleTypeSyntax declaration) : this(new TupleElementVisiter().GetEntries(declaration.SyntaxTree).Select(x => new Property(x))) { }
    //public ClassDeclaration(MethodDeclarationSyntax declaration) : this(declaration.ParameterList) { }
    //public ClassDeclaration(ConstructorDeclarationSyntax declaration) : this(declaration.ParameterList) { }
    //public ClassDeclaration(ParameterListSyntax parameters) : this(parameters.Parameters.Select(x => new Property(x))) { }
    //public ClassDeclaration(IEnumerable<ParameterInfo> parameters) : this(parameters.Select(x => new PropertyFromParameter(x))) { }

    public ClassDeclarationSyntax ToClass(string? name = null, Modifier modifiers = Modifier.Public, Modifier memberModifiers = Modifier.Public) => ClassDeclarationCustom(
            attributeLists: default,
            modifiers: modifiers.Syntax(),
            identifier: ToIdentifier(),
            typeParameterList: default,
            baseList: default,
            constraintClauses: default,
            members: ToMembers(memberModifiers)
        );

    public RecordDeclarationSyntax ToRecord(string? name = null, Modifier modifiers = Modifier.Public) => RecordDeclarationCustom(
            attributeLists: default,
            modifiers: modifiers.Syntax(),
            identifier: name != null ? SyntaxFactory.Identifier(name) : ToIdentifier(),
            typeParameterList: default,
            parameterList: ToParameters(),
            baseList: default,
            constraintClauses: default,
            members: default);

    public ParameterListSyntax ToParameters() => ParameterListCustom(GetProperties().Select(x => x.ToParameter()));
    public ArgumentListSyntax ToArguments() => ArgumentListCustom(GetProperties().Select(x => x.Value.ToArgument()));
    public InitializerExpressionSyntax ToInitializer() => InitializerExpression(SyntaxKind.ObjectCreationExpression, SeparatedList(GetPropertiesAndFields().Select(x => x.Value.Syntax())));
    public List<IMember> Ordered() => Members.OrderBy(x => x.Modifier, new ModifierComparer()).ToList();
    public SyntaxList<MemberDeclarationSyntax> ToMembers(Modifier modifier = Modifier.None) => SyntaxFactory.List(Ordered(modifier).Select(x => x.SyntaxWithModifiers(modifier)));
    public List<PropertyModel> FilterValues() => GetProperties().Where(x => x.Value != null).ToList();
    public List<IExpression> ToExpressions() => GetProperties().Select(x => x.Value).ToList();
    public ExpressionCollection ToValueCollection() => new(FilterValues().Select(x => x.Value ?? throw new Exception($"Property '{x}' contains no value.")), Get_Type());
    public ArrayCreationExpressionSyntax ToArrayCreationSyntax() => ToValueCollection().Syntax();
    //public override LiteralExpressionSyntax? LiteralSyntax() => ToValueCollection().LiteralSyntax;
    public SeparatedSyntaxList<ExpressionSyntax> SyntaxList() => SeparatedList(GetPropertiesAndFields().Select(x => x.ExpressionSyntax!));
    //public override object? LiteralValue() => ToValueCollection().LiteralValue();

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var member in Members) yield return member;
    }

    public void Evaluate(IProgramModelExecutionContext context) => context.AddMember(Namespace?.Name, this);

    public IType BaseType() => CodeModelFactory.QuickType(Name);

    public PropertyCollection ToPropertyCollection() => throw new NotImplementedException();

    public List<IType> ConvertToList() => ToPropertyCollection().ConvertToList();
    public List<Property> AsList(Property? typeSpecifier = null) => ToPropertyCollection().AsList(typeSpecifier);
    public ITypeCollection ToTypeCollection() => ToPropertyCollection().ToTypeCollection();

    MemberDeclarationSyntax IMember.Syntax()
        => ToClass();

    public override ClassDeclarationSyntax Syntax() => Syntax();

    public override InstantiatedObject CreateInstance()
    {
        var scope = CreateInstanceScope();
        var instance = new InstantiatedObject(this, scope, GetStaticScope(), Parent?.CreateInstanceScope());
        scope.SetThis(instance);
        InitInstanceScope(scope);
        return instance;
    }

    public ProgramModelExecutionScope CreateInstanceScope(bool init = false)
    {
        var scope = new ProgramModelExecutionScope();
        if (init) InitInstanceScope(scope);
        return scope;
    }

    private record ClassDeclarationImp(string Name,
    List<IMember> Members,
    Namespace? Namespace,
    Modifier Modifier = Modifier.Public)
    : ClassDeclaration(
        Name: Name,
        Members: Members,
        Namespace: Namespace,
        Modifier: Modifier);
}

