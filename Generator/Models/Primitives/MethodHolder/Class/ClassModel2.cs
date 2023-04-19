using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeAnalyzation.Collectors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public abstract record ClassModel2(string Name,
    List<IFieldOrProperty> Members,
    List<IMethod> Methods,
    ClassModel2? Parent = null,
    IType? SpecifiedType = null)
    : BaseType<ClassDeclarationSyntax>(
        Name: Name,
        Members: Members,
        Methods: Methods,
        Namespace: null,
        TopLevelModifier: Modifier.None,
        MemberModifier: Modifier.None,
        ReflectedType: null),
    INamedValueCollection<IFieldOrProperty>,
    IMember
{
    public static ClassModel2 Create(string name,
    IEnumerable<IFieldOrProperty>? members = null,
    IEnumerable<IMethod>? methods = null,
    IType? specifiedType = null)
    {
        var c = new ClassModel2Imp(name, List(members), List(methods), specifiedType);
        c.InitOwner();
        return c;
    }

    //public ClassModel2(IEnumerable<Property>? properties = null, string? name = null, IType? specifiedType = null) : this(List(properties), name, specifiedType) { }
    //public ClassModel2(IEnumerable<PropertyInfo> properties) : this(properties.Select(x => new PropertyFromReflection(x))) { }
    //public ClassModel2(IEnumerable<FieldInfo> fields) : this(fields.Select(x => new PropertyFromField(x))) { }
    //public ClassModel2(Type type) : this(type.GetProperties(), type.GetFields()) { }
    //public ClassModel2(IEnumerable<PropertyInfo> properties, IEnumerable<FieldInfo> fields)
    //    : this(properties.Select(x => new PropertyFromReflection(x)).ToList<Property>().Concat(fields.Select(x => new PropertyFromField(x)))) { }
    //public ClassModel2(ClassDeclarationSyntax declaration) : this(new PropertyVisiter().GetEntries(declaration.SyntaxTree).Select(x => new Property(x)), declaration.Identifier.ToString()) { }
    //public ClassModel2(RecordDeclarationSyntax declaration) : this(new ParameterVisiter().GetEntries(declaration.SyntaxTree).Select(x => new Property(x)), declaration.Identifier.ToString()) { }
    //public ClassModel2(TupleTypeSyntax declaration) : this(new TupleElementVisiter().GetEntries(declaration.SyntaxTree).Select(x => new Property(x))) { }
    //public ClassModel2(MethodDeclarationSyntax declaration) : this(declaration.ParameterList) { }
    //public ClassModel2(ConstructorDeclarationSyntax declaration) : this(declaration.ParameterList) { }
    //public ClassModel2(ParameterListSyntax parameters) : this(parameters.Parameters.Select(x => new Property(x))) { }
    //public ClassModel2(IEnumerable<ParameterInfo> parameters) : this(parameters.Select(x => new PropertyFromParameter(x))) { }

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
    public InitializerExpressionSyntax ToInitializer() => InitializerExpression(SyntaxKind.ObjectCreationExpression, SeparatedList(Members.Select(x => x.Value.Syntax())));
    public List<IFieldOrProperty> Ordered() => Members.OrderBy(x => x.Modifier, new ModifierComparer()).ToList();
    public SyntaxList<MemberDeclarationSyntax> ToMembers(Modifier modifier = Modifier.None) => SyntaxFactory.List(Ordered(modifier).Select(x => x.SyntaxWithModifiers(modifier)));
    public List<PropertyModel> FilterValues() => GetProperties().Where(x => x.Value != null).ToList();
    public List<IExpression> ToExpressions() => GetProperties().Select(x => x.Value).ToList();
    public ExpressionCollection ToValueCollection() => new(FilterValues().Select(x => x.Value ?? throw new Exception($"Property '{x}' contains no value.")), Get_Type());
    public ArrayCreationExpressionSyntax ToArrayCreationSyntax() => ToValueCollection().Syntax();
    //public override LiteralExpressionSyntax? LiteralSyntax => ToValueCollection().LiteralSyntax;
    public SeparatedSyntaxList<ExpressionSyntax> SyntaxList() => SeparatedList(Members.Select(x => x.ExpressionSyntax!));
    //public override object? LiteralValue => ToValueCollection().LiteralValue;

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var member in Members) yield return member;
        foreach (var method in Methods) yield return method;
    }

    //public override IExpression Evaluate(IProgramModelExecutionContext context)
    //    => Literal(ToExpressions().Select(x => x.EvaluatePlain(context)).ToArray());

    public IType BaseType()
        => new QuickType(Name);

    public PropertyCollection ToPropertyCollection() => throw new NotImplementedException();

    public List<IType> ConvertToList() => ToPropertyCollection().ConvertToList();
    public List<Property> AsList(Property? typeSpecifier = null) => ToPropertyCollection().AsList(typeSpecifier);
    public ITypeCollection ToTypeCollection() => ToPropertyCollection().ToTypeCollection();

    MemberDeclarationSyntax IMember.Syntax()
        => ToClass();

    public override ClassDeclarationSyntax Syntax() => Syntax();

    public InstantiatedObject CreateInstance()
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

    private record ClassModel2Imp(string Name,
    List<IFieldOrProperty> Members,
    List<IMethod> Methods,
    IType? SpecifiedType = null)
    : ClassModel2(
        Name: Name,
        Members: Members,
        Methods: Methods,
        SpecifiedType: SpecifiedType);
}

