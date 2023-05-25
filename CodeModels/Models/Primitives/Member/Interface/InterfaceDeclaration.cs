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

public abstract record InterfaceDeclaration(string Name,
    List<IType> GenericParameters,
    List<TypeParameterConstraintClause> ConstraintClauses,
    List<IBaseType> BaseTypeList,
    List<IMember> Members,
    Namespace? Namespace = null,
    Modifier Modifier = Modifier.Public)
    : TypeDeclaration<InterfaceDeclarationSyntax>(
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
    IMember, IInterfaceDeclaration
{
    public static InterfaceDeclaration Create(string name,
    IEnumerable<IType>? genericParameters = null,
    IEnumerable<TypeParameterConstraintClause>? constraintClauses = null,
    IEnumerable<IBaseType>? baseTypeList = null,
    IEnumerable<IMember>? members = null,
    Namespace? @namespace = null,
    Modifier? modifier = null)
    {
        var declaration = new InterfaceDeclarationImp(name, List(genericParameters), List(constraintClauses), List(baseTypeList), List(members), @namespace, modifier ?? Modifier.Public);
        declaration.InitOwner();
        return declaration;
    }

    public ParameterListSyntax ToParameters() => ParameterListCustom(GetProperties().Select(x => x.ToParameter()));
    public ArgumentListSyntax ToArguments() => ArgumentListCustom(GetProperties().Select(x => ExpressionUtils.ExpressionOrVoid(x.Value).ToArgument()));
    public InitializerExpressionSyntax ToInitializer() => InitializerExpression(SyntaxKind.ObjectCreationExpression, SeparatedList(GetPropertiesAndFields().Select(x => ExpressionUtils.ExpressionOrVoid(x.Value).Syntax())));
    public List<IMember> Ordered() => Members.OrderBy(x => x.Modifier, new ModifierComparer()).ToList();
    public List<Property> FilterValues() => GetProperties().Where(x => x.Value != null).ToList();
    public List<IExpression> ToExpressions() => GetProperties().Select(x => ExpressionUtils.ExpressionOrVoid(x.Value)).ToList();
    public ExpressionCollection ToValueCollection() => AbstractCodeModelFactory.Expressions(FilterValues().Select(x => x.Value ?? throw new Exception($"Property '{x}' contains no value.")), Get_Type());
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

    public override InterfaceDeclarationSyntax Syntax() => ToInterface();
    MemberDeclarationSyntax IMember.Syntax() => Syntax();

    public override IInstantiatedObject CreateInstance()
    {
        var scope = CreateInstanceScope();
        var instance = new InstantiatedInterface(this, scope, GetStaticScope());
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

    private record InterfaceDeclarationImp(string Name,
    List<IType> GenericParameters,
    List<TypeParameterConstraintClause> ConstraintClauses,
    List<IBaseType> BaseTypeList,
    List<IMember> Members,
    Namespace? Namespace,
    Modifier Modifier = Modifier.Public)
    : InterfaceDeclaration(
        Name: Name,
        GenericParameters: GenericParameters,
        ConstraintClauses: ConstraintClauses,
        BaseTypeList: BaseTypeList,
        Members: Members,
        Namespace: Namespace,
        Modifier: Modifier);
}

