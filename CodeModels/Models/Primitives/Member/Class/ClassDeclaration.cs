using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member;

public abstract record ClassDeclaration(string Name,
    List<IType> GenericParameters,
    List<TypeParameterConstraintClause> ConstraintClauses,
    List<IBaseType> BaseTypeList,
    List<IMember> Members,
    IClassDeclaration? Parent = null,
    Namespace? Namespace = null,
    Modifier Modifier = Modifier.Public)
    : TypeDeclaration<ClassDeclarationSyntax>(
        Name: Name,
        GenericParameters: GenericParameters,
        ConstraintClauses: ConstraintClauses,
        BaseTypeList: BaseTypeList,
        Members: Members,
        Namespace: Namespace,
        TopLevelModifier: Modifier,
        MemberModifier: Modifier.None,
        ReflectedType: null),
    INamedValueCollection<IFieldOrProperty>,
    IMember, IClassDeclaration
{
    public static ClassDeclaration Create(string name,
    IEnumerable<IType>? genericParameters = null,
    IEnumerable<TypeParameterConstraintClause>? constraintClauses = null,
    IEnumerable<IBaseType>? baseTypeList = null,
    IEnumerable<IMember>? members = null,
    Namespace? @namespace = null,
    Modifier? modifier = null)
    {
        var declaration = new ClassDeclarationImp(name, List(genericParameters), List(constraintClauses), List(baseTypeList), List(members), @namespace, modifier ?? Modifier.Public);
        declaration.InitOwner();
        return declaration;
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

    //public ClassDeclarationSyntax ToClass(string? name = null, Modifier? modifiers = null, Modifier memberModifiers = Modifier.Public) => ClassDeclarationCustom(
    //        attributeLists: default,
    //        modifiers: (modifiers ?? TopLevelModifier).Syntax(),
    //        identifier: ToIdentifier(),
    //        typeParameterList: default,
    //        baseList: default,
    //        constraintClauses: default,
    //        members: ToMembers(memberModifiers));

    //public RecordDeclarationSyntax ToRecord(string? name = null, Modifier? modifiers = null) => RecordDeclarationCustom(
    //        attributeLists: default,
    //        modifiers: (modifiers ?? TopLevelModifier).Syntax(),
    //        identifier: name != null ? Identifier(name) : ToIdentifier(),
    //        typeParameterList: default,
    //        parameterList: ToParameters(),
    //        baseList: default,
    //        constraintClauses: default,
    //        members: default);

    public ParameterListSyntax ToParameters() => ParameterListCustom(GetProperties().Select(x => x.ToParameter()));
    public ArgumentListSyntax ToArguments() => ArgumentListCustom(GetProperties().Select(x => ExpressionUtils.ExpressionOrVoid(x.Value).ToArgument()));
    public InitializerExpressionSyntax ToInitializer() => InitializerExpression(SyntaxKind.ObjectCreationExpression, SeparatedList(GetPropertiesAndFields().Select(x => ExpressionUtils.ExpressionOrVoid(x.Value).Syntax())));
    public List<IMember> Ordered() => Members.OrderBy(x => x.Modifier, new ModifierComparer()).ToList();
    public List<Property> FilterValues() => GetProperties().Where(x => x.Value != null).ToList();
    public List<IExpression> ToExpressions() => GetProperties().Select(x => ExpressionUtils.ExpressionOrVoid(x.Value)).ToList();
    public ExpressionCollection ToValueCollection() => new(FilterValues().Select(x => x.Value ?? throw new Exception($"Property '{x}' contains no value.")), Get_Type());
    public ArrayCreationExpressionSyntax ToArrayCreationSyntax() => ToValueCollection().Syntax();
    //public override LiteralExpressionSyntax? LiteralSyntax() => ToValueCollection().LiteralSyntax;
    public SeparatedSyntaxList<ExpressionSyntax> SyntaxList() => SeparatedList(GetPropertiesAndFields().Select(x => x.ExpressionSyntax!));
    //public override object? LiteralValue() => ToValueCollection().LiteralValue();

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var member in Members) yield return member;
    }

    public void Evaluate(ICodeModelExecutionContext context) => context.AddMember(Namespace?.Name, this);

    public IType BaseType() => CodeModelFactory.QuickType(Name);

    public NamedValueCollection ToNamedValues() => throw new NotImplementedException();

    public List<IType> ConvertToList() => ToNamedValues().ConvertToList();
    public List<AbstractProperty> AsList(AbstractProperty? typeSpecifier = null) => ToNamedValues().AsList(typeSpecifier);
    public ITypeCollection ToTypeCollection() => ToNamedValues().ToTypeCollection();

    public override ClassDeclarationSyntax Syntax() => ToClass();
    MemberDeclarationSyntax IMember.Syntax() => Syntax();

    public override IInstantiatedObject CreateInstance()
    {
        var scope = CreateInstanceScope();
        var instance = new InstantiatedObject(this, scope, GetStaticScope(), Parent?.CreateInstanceScope());
        scope.SetThis(instance);
        InitInstanceScope(scope);
        return instance;
    }

    public CodeModelExecutionScope CreateInstanceScope(bool init = false)
    {
        var scope = new CodeModelExecutionScope();
        if (init) InitInstanceScope(scope);
        return scope;
    }

    private record ClassDeclarationImp(string Name,
    List<IType> GenericParameters,
    List<TypeParameterConstraintClause> ConstraintClauses,
    List<IBaseType> BaseTypeList,
    List<IMember> Members,
    Namespace? Namespace,
    Modifier Modifier = Modifier.Public)
    : ClassDeclaration(
        Name: Name,
        GenericParameters: GenericParameters,
        ConstraintClauses: ConstraintClauses,
        BaseTypeList: BaseTypeList,
        Members: Members,
        Namespace: Namespace,
        Modifier: Modifier);
}

