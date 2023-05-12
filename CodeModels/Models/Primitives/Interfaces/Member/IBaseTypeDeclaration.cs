using System;
using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Execution.Scope;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Member;
using Common.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IBaseTypeDeclaration : IScopeHolder, ITypeModel, IMember
{
    List<IMember> Members { get; }
    EqualityList<IType> GenericTypes { get; }
    bool IsLiteralExpression { get; }
    bool IsMulti { get; }
    Modifier MemberModifier { get; }
    Namespace? Namespace { get; }
    Type? ReflectedType { get; }
    bool Required { get; }
    Modifier TopLevelModifier { get; }
    string TypeName { get; }

    List<Method> Methods();
    new BaseTypeDeclarationSyntax Syntax();
    IInstantiatedObject CreateInstance();
    IBaseTypeDeclaration AddMember(IMember member);
    IBaseTypeDeclaration AddProperty(AbstractType type, string name);
    IBaseTypeDeclaration AddProperty(ITypeSymbol type, string name);
    IBaseTypeDeclaration AddProperty(Type type, string name);
    List<IMember> AllMembers();
    Constructor GetConstructor();
    List<Constructor> GetConstructors();
    IField GetField(string name);
    List<IField> GetFields();
    IMember GetMember(string name);
    IMethod GetMethod(string name);
    List<Property> GetProperties();
    List<IFieldOrProperty> GetPropertiesAndFields();
    Property GetProperty(string name);
    List<IMember> GetReadonlyMembers();
    CodeModelExecutionScope GetStaticScope();
    LiteralExpressionSyntax? LiteralSyntax();
    object? LiteralValue();
    SyntaxList<MemberDeclarationSyntax> MembersSyntax();
    SyntaxList<MemberDeclarationSyntax> MethodsSyntax();
    List<IMember> Ordered(Modifier modifier = Modifier.None);
    ClassDeclarationSyntax ToClass();
    InterfaceDeclarationSyntax ToInterface();
    RecordDeclarationSyntax ToRecord();
    StructDeclarationSyntax ToStruct();
    TupleTypeSyntax ToTupleType();
    IField? TryGetField(string name);
    IMember TryGetMember(string name);
    Property? TryGetProperty(string name);
}

public interface IBaseTypeDeclaration<T> : ICodeModel<T>, IBaseTypeDeclaration
    where T : BaseTypeDeclarationSyntax
{
    new T Syntax();
}
