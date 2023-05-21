using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Reflection;
using Common.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models.Primitives.Member;

public record MethodFromReflection(MethodInfo Method)
    : Method(
        Method.Name, 
        new NamedValueCollection(Method.GetParameters()),
        Method.GetGenericArguments().Select(x => TypeFromReflection.Create(x)).ToList<IType>(),
        new List<TypeParameterConstraintClause>(),
        TypeFromReflection.Create(Method.ReturnType),
        null)
{
    //public MethodFromReflection(IMethodSymbol symbol) : this(SemanticReflection.GetMethod(symbol) ?? Context.Get<MethodInfo>(symbol)) { }
}

public abstract record MemberFromSymbol<T, TCodeModel>(T Symbol) : IMember
    where T : ISymbol
    where TCodeModel : class, IMember
{
    public IProgramContext Context => ProgramContext.GetContext(Symbol);
    public TCodeModel Lookup => Context.Get<TCodeModel>(Symbol);
    public IMember Member => Lookup;

    public string Name => Member.Name;
    public Modifier Modifier => Member.Modifier;
    public bool IsStatic => Member.IsStatic;
    public IEnumerable<ICodeModel> Children() => Lookup.Children();
    public string Code() => Lookup.Code();
    public ISet<IType> Dependencies(ISet<IType>? set = null) => Lookup.Dependencies(set);
    public IType Get_Type() => Lookup.Get_Type();

    public virtual SimpleNameSyntax NameSyntax() => throw new NotImplementedException();

    public MemberDeclarationSyntax Syntax() => Member.Syntax();
    public MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => Lookup.SyntaxWithModifiers(modifier, removeModifier);

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

    public IType ToType()
    {
        throw new NotImplementedException();
    }

    public TypeSyntax TypeSyntax() => Lookup.TypeSyntax();
    CSharpSyntaxNode ICodeModel.Syntax() => Member.Syntax();

    public ICodeModel Render(Namespace @namespace)
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

    public SyntaxToken IdentifierSyntax()
    {
        throw new NotImplementedException();
    }
}

public abstract record MethodBaseFromSymbol<T, TCodeModel>(T Symbol) : MemberFromSymbol<T, TCodeModel>(Symbol), IMethodBase
    where T : IMethodSymbol
    where TCodeModel : class, IMethod
{
    public bool IsFamily => Lookup.IsFamily;
    public bool IsFamilyAndAssembly => Lookup.IsFamilyAndAssembly;
    public bool IsFamilyOrAssembly => Lookup.IsFamilyOrAssembly;
    public bool IsFinal => Lookup.IsFinal;
    public bool IsGenericMethod => Lookup.IsGenericMethod;
    public bool IsGenericMethodDefinition => Lookup.IsGenericMethodDefinition;
    public bool IsHideBySig => Lookup.IsHideBySig;
    public bool IsPrivate => Lookup.IsPrivate;
    public bool IsPublic => Lookup.IsPublic;
    public bool IsSecurityCritical => Lookup.IsSecurityCritical;
    public bool IsSecuritySafeCritical => Lookup.IsSecuritySafeCritical;
    public bool IsSecurityTransparent => Lookup.IsSecurityTransparent;
    public bool IsSpecialName => Lookup.IsSpecialName;
    public bool IsConstructor => Lookup.IsConstructor;
    public RuntimeMethodHandle MethodHandle => Lookup.MethodHandle;
    public bool IsAssembly => Lookup.IsAssembly;
    public bool ContainsGenericParameters => Lookup.ContainsGenericParameters;
    public bool IsAbstract => Lookup.IsAbstract;
    public MethodImplAttributes MethodImplementationFlags => Lookup.MethodImplementationFlags;
    public CallingConventions CallingConvention => Lookup.CallingConvention;
    public MethodAttributes Attributes => Lookup.Attributes;
    public IEnumerable<CustomAttributeData> CustomAttributes => Lookup.CustomAttributes;
    public ITypeInfo DeclaringType => Lookup.DeclaringType;
    public MemberTypes MemberType => Lookup.MemberType;
    public int MetadataToken => Lookup.MetadataToken;
    public Module Module => Lookup.Module;
    public ITypeInfo ReflectedType => Lookup.ReflectedType;
    public object[] GetCustomAttributes(bool inherit) => Lookup.GetCustomAttributes(inherit);
    public object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit) => Lookup.GetCustomAttributes(attributeType, inherit);
    public IList<CustomAttributeData> GetCustomAttributesData() => Lookup.GetCustomAttributesData();
    public ITypeInfo[] GetGenericArguments() => Lookup.GetGenericArguments();
    public IMethodInfo GetGenericMethodDefinition() => Lookup.GetGenericMethodDefinition();
    public MethodBody GetMethodBody() => Lookup.GetMethodBody();
    public MethodImplAttributes GetMethodImplementationFlags() => Lookup.GetMethodImplementationFlags();
    public IParameterInfo[] GetParameters() => Lookup.GetParameters();
    public object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => Lookup.Children();
    public object Invoke(object obj, object[] parameters) => Lookup.Invoke(obj, parameters);

    public IInvocation Invoke(IExpression caller, IEnumerable<IExpression> arguments)
    {
        throw new NotImplementedException();
    }

    public bool IsDefined(ITypeInfo attributeType, bool inherit) => Lookup.IsDefined(attributeType, inherit);

    BaseMethodDeclarationSyntax IMethodBase.Syntax()
    {
        throw new NotImplementedException();
    }

    BaseMethodDeclarationSyntax IMethodBase.SyntaxWithModifiers(Modifier modifier, Modifier removeModifier)
    {
        throw new NotImplementedException();
    }
}

public record MethodFromSymbol(IMethodSymbol Symbol) : MethodBaseFromSymbol<IMethodSymbol, IMethod>(Symbol), IMethod
{
    public IParameterInfo ReturnParameter => Lookup.ReturnParameter;
    public ITypeInfo ReturnType => Lookup.ReturnType;
    public Reflection.ICustomAttributeProvider ReturnTypeCustomAttributes => Lookup.ReturnTypeCustomAttributes;

    public ITypeDeclaration? Owner { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ExpressionSyntax? ExpressionSyntax => throw new NotImplementedException();

    public IExpression Value => throw new NotImplementedException();


    public ExpressionSyntax? AccessSyntax(IExpression? instance = null)
    {
        throw new NotImplementedException();
    }

    public IExpression AccessValue(string identifier, IType? type = null, ISymbol? symbol = null)
    {
        throw new NotImplementedException();
    }

    public IExpression AccessValue(IExpression? instance = null)
    {
        throw new NotImplementedException();
    }

    public Delegate CreateDelegate(ITypeInfo delegateType) => Lookup.CreateDelegate(delegateType);
    public Delegate CreateDelegate(ITypeInfo delegateType, object target) => Lookup.CreateDelegate(delegateType, target);
    public IMethodInfo GetBaseDefinition() => Lookup.GetBaseDefinition();
    public IMethodInfo MakeGenericMethod(params ITypeInfo[] typeArguments) => Lookup.MakeGenericMethod(typeArguments);
    public MethodDeclarationSyntax ToMethodSyntax(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
         => Lookup.ToMethodSyntax(modifier, removeModifier);
    MethodDeclarationSyntax ICodeModel<MethodDeclarationSyntax>.Syntax() => (Member.Syntax() as MethodDeclarationSyntax)!;
}

public record TypeFromSymbol2(ITypeSymbol Symbol) : MemberFromSymbol<ITypeSymbol, IType>(Symbol), IType
{
    public string TypeName => Lookup.TypeName;
    public bool Required => Lookup.Required;
    public bool IsMulti => Lookup.IsMulti;
    public Type? ReflectedType => Lookup.ReflectedType;
    public EqualityList<IType> GenericTypes => Lookup.GenericTypes;
    public bool IsLiteralExpression => Lookup.IsLiteralExpression;
    public LiteralExpressionSyntax? LiteralSyntax() => Lookup.LiteralSyntax();
    public object? LiteralValue() => Lookup.LiteralValue();
    public ExpressionStatement AsStatement() => Lookup.AsStatement();
    public bool Equals(IType other, ICodeModelExecutionContext context)
        => TypeName == other.TypeName; // TODO: Check assembly
    public bool IsAssignableFrom(IType other, ICodeModelExecutionContext context)
        => ReflectedType is Type type && other.ReflectedType is Type otherType
            && type.IsAssignableFrom(otherType) || Equals(other, context); // TODO: Check for non-reflected

    public IExpression Evaluate(ICodeModelExecutionContext context) => Lookup.AsStatement();
    public object? EvaluatePlain(ICodeModelExecutionContext context) => Lookup.AsStatement();
    public IType GetGenericType(int index) => Lookup.GetGenericType(index);
    public IdentifierExpression ToIdentifierExpression() => new(Lookup.TypeName);
    public string GetMostSpecificType() => Lookup.GetMostSpecificType();
    public Type? GetReflectedType() => Lookup.GetReflectedType();
    public ArgumentSyntax ToArgument() => Lookup.ToArgument();
    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => Lookup.ToEnumValue(value);
    public IType ToMultiType() => Lookup.ToMultiType();
    public TypeParameterSyntax ToTypeParameter() => Lookup.ToTypeParameter();
    public TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type) => Lookup.TypeSyntaxMultiWrapped(type);
    public TypeSyntax TypeSyntaxNonMultiWrapped() => Lookup.TypeSyntaxNonMultiWrapped();
    public TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type) => Lookup.TypeSyntaxNullableWrapped(type);
    public TypeSyntax TypeSyntaxUnwrapped() => Lookup.TypeSyntaxUnwrapped();
    TypeSyntax IType.Syntax() => Lookup.Syntax();
    ExpressionOrPatternSyntax IExpressionOrPattern.Syntax() => Lookup.Syntax();
    TypeSyntax ICodeModel<TypeSyntax>.Syntax() => Lookup.Syntax();
    ExpressionSyntax IExpression.Syntax() => Lookup.Syntax();

    public IType PlainType()
    {
        throw new NotImplementedException();
    }

    public IType ToOptionalType()
    {
        throw new NotImplementedException();
    }
}



