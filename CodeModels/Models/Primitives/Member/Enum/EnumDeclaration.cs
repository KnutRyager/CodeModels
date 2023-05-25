using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Scope;
using CodeModels.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models.Primitives.Member.Enum;

public abstract record EnumDeclaration(string Name,
    List<IMember> Members,
    Namespace? Namespace,
    Modifier Modifier,
    Type? ReflectedType = null)
    : BaseTypeDeclaration<EnumDeclarationSyntax>(Name,
        GenericParameters: new List<IType>(),
        new List<TypeParameterConstraintClause>(),
        new List<IBaseType>(),
        Members: Members,
        Namespace: Namespace,
        TopLevelModifier: Modifier,
        MemberModifier: Modifier.None,
        ReflectedType: ReflectedType)
{
    public IEnumerable<IEnumerable<string>>? ValueCategories { get; set; }

    public static EnumDeclaration Create(string name,
    IEnumerable<IEnumMember>? members = null,
    Namespace? @namespace = null,
    Modifier? modifier = null)
    {
        var declaration = new EnumDeclarationImp(name, List(InitMembersWithDefaultValues(members)), @namespace, modifier ?? Modifier.Public);
        declaration.InitOwner();
        return declaration;
    }

    public EnumDeclarationSyntax ToEnum() => SyntaxFactory.EnumDeclaration(
        attributeLists: default,
        modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
        identifier: SyntaxFactory.Identifier(Name),
        baseList: default,
        members: SyntaxFactory.SeparatedList(GetEnumMembers().Select(x => x.EnumSyntax())));

    public List<IEnumMember> GetEnumMembers() => Members.Select(x => (x as IEnumMember)!).Where(x => x is not null).ToList();

    public override EnumDeclarationSyntax Syntax() => ToEnum();

   public override IInstantiatedObject CreateInstance()
    {
        var scope = CreateInstanceScope();
        var instance = new InstantiatedEnum(this, scope, GetStaticScope());
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

    private static IEnumerable<IMember>? InitMembersWithDefaultValues(IEnumerable<IEnumMember>? members = null)
    {
        if (members is null) return null;
        IEnumMember? previousMember = null;
        var initializedMembers = new List<IMember>();
        foreach (var member in members)
        {
            var initializedMember = previousMember is not null && ExpressionUtils.IsNull(member.Value)
                ? EnumField(member.Name, ExpressionUtils.Add(previousMember.Value!, 1))
                : ExpressionUtils.IsNull(member.Value) ? EnumField(member.Name, 0) : member;
            initializedMembers.Add(initializedMember);
            previousMember = initializedMember;
        }
        return initializedMembers;
    }

    private record EnumDeclarationImp(string Name,
    List<IMember> Members,
    Namespace? Namespace,
    Modifier Modifier = Modifier.Public)
    : EnumDeclaration(
        Name: Name,
        Members: Members,
        Namespace: Namespace,
        Modifier: Modifier);
}
