using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Models;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Invocation;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;
using Common.Reflection;
using Microsoft.CodeAnalysis;

namespace CodeModels.Factory;

public static class CodeModelsFromReflection
{
    public static Namespace Namespace(Type type) => new(type.Namespace);
    public static TypeFromReflection Type(Type type) => TypeFromReflection.Create(type);
    public static ConstructorFromReflection Constructor(ConstructorInfo info) => new(info);
    public static MethodFromReflection Method(MethodInfo info) => new(info);
    public static FieldFromReflection Field(FieldInfo info) => new(info);
    public static PropertyExpressionFromReflection Property(PropertyInfo info) => new(info);

    public static IBaseTypeDeclaration MetodHolder(Type type) => type switch
    {
        { IsInterface: true } => Interface(type),
        { IsEnum: true } => Enum(type),
        _ when ReflectionUtil.IsStatic(type) => StaticClass(type),
        _ => InstanceClass(type)
    };

    public static StaticClassFromReflection StaticClass(Type type) => new(type);
    public static InstanceClassFromReflection InstanceClass(Type type) => new(type);
    public static InterfaceFromReflection Interface(Type type) => new(type);
    public static EnumFromReflection Enum(Type type) => new(type);

    public static List<IMethod> Methods(Type type) => type.GetMethods().Select(x => new MethodFromReflection(x)).ToList<IMethod>();

    public static LiteralExpression Literal(object value) => LiteralExpression.Create(value);

    public static NamedValueCollection NamedValues(Type type) => new(type);

    public static PropertyExpressionFromReflection PropertyAccess(PropertyInfo property, IExpression caller)
        => PropertyExpressionFromReflection.Create(property, caller);
    public static PropertyExpressionFromReflection PropertyAccess(Type type, string name, IExpression caller)
        => PropertyAccess(type.GetProperty(name), caller);
    public static PropertyExpressionFromReflection PropertyAccess<T>(System.Linq.Expressions.Expression<Func<T, object>> expression, IExpression caller)
        => PropertyAccess(ExpressionReflectionUtil.GetPropertyInfo(expression), caller);
    //public static PropertyExpressionFromReflection PropertyAccess<T>(System.Linq.Expressions.Expression<Action<T>> expression, IExpression caller)
    //    => PropertyAccess(ExpressionReflectionUtil.GetPropertyInfo(expression), caller);
    //public static PropertyExpressionFromReflection PropertyAccess<T>(System.Linq.Expressions.Expression<Func<T, Delegate>> expression, IExpression caller)
    //    => PropertyAccess(ExpressionReflectionUtil.GetPropertyInfo(expression), caller);
    //public static PropertyExpressionFromReflection PropertyAccess(System.Linq.Expressions.Expression<Action<object>> expression, IExpression caller)
    //    => PropertyAccess(ExpressionReflectionUtil.GetPropertyInfo(expression), caller);

    public static ILambdaExpression GetModel<T>(System.Linq.Expressions.Expression<Func<T, object>> expression)
        => CodeModelsFromExpression.GetModel(expression);
    public static IExpression GetModel(System.Linq.Expressions.Expression<Func<object, object?>> expression)
        => CodeModelsFromExpression.GetExpression(expression);

    public static IdentifierExpression GetIdentifier<T>(System.Linq.Expressions.Expression<Func<T, object>> expression)
        => CodeModelFactory.Identifier(ExpressionReflectionUtil.GetConstant(expression.Body) is object o
            ? o is Enum e ? e.ToString() : o.GetType() is Type type ? type.Name : throw new NotImplementedException()
            : throw new NotImplementedException());
    public static IdentifierExpression GetIdentifier(System.Linq.Expressions.Expression<Func<object, object>> expression)
        => CodeModelFactory.Identifier(ExpressionReflectionUtil.GetConstant(expression.Body)?.GetType() is Type type ? type.Name : throw new NotImplementedException());
    private static string GetName(object o) => o switch
    {

        _ => throw new NotImplementedException($"Unhandled: {o}")
    };

    public static InvocationFromReflection Invocation(MethodInfo method, IExpression caller, IEnumerable<IExpression>? arguments = null)
        => InvocationFromReflection.Create(method, caller, arguments);
    public static InvocationFromReflection Invocation(Type type, string name, IExpression caller, IEnumerable<IExpression>? arguments = null)
        => Invocation(ReflectionUtil.GetMethodInfo(type, name,
            arguments is null ? Array.Empty<Type>() : arguments.Select(x => x.Get_Type()?.ReflectedType ?? throw new ArgumentException($"No ReflectedType for '{x}'")).ToArray())!,
            caller, arguments);
    public static InvocationFromReflection Invocation<TIn, TOut>(System.Linq.Expressions.Expression<Func<TIn, TOut>> expression, IExpression caller, IEnumerable<IExpression>? arguments = null)
        => Invocation(ExpressionReflectionUtil.GetMethodInfo(expression), caller, arguments);
    public static InvocationFromReflection Invocation<T>(System.Linq.Expressions.Expression<Action<T>> expression, IExpression caller, IEnumerable<IExpression>? arguments = null)
        => Invocation(ExpressionReflectionUtil.GetMethodInfo(expression), caller, arguments);
    public static InvocationFromReflection Invocation<T>(System.Linq.Expressions.Expression<Func<T, Delegate>> expression, IExpression caller, IEnumerable<IExpression>? arguments = null)
        => Invocation(ExpressionReflectionUtil.GetMethodInfo(expression), caller, arguments);
    public static InvocationFromReflection Invocation(System.Linq.Expressions.Expression<Action<object>> expression, IExpression caller, IEnumerable<IExpression>? arguments = null)
        => Invocation(ExpressionReflectionUtil.GetMethodInfo(expression), caller, arguments);

    public static Parameter Param(ParameterInfo parameter)
        => CodeModelFactory.Param(parameter.Name, Type(parameter.ParameterType), CodeModelFactory.Literal(parameter.DefaultValue));
    public static ParameterList ParamList(IEnumerable<ParameterInfo> parameters)
        => CodeModelFactory.ParamList(parameters.Select(Param));
    public static ParameterList ParamList(MethodInfo method)
        => ParamList(method.GetParameters());
    public static ParameterList ParamList(ConstructorInfo constructor)
        => ParamList(constructor.GetParameters());
    // TODO: Match properties of Attribute with constructor call
    public static Models.Primitives.Attribute.Attribute Attr(System.Attribute attribute)
        => CodeModelFactory.Attribute(attribute.GetType().Name);
    public static AttributeListList AttributesList(IEnumerable<System.Attribute> attributes)
        => CodeModelFactory.AttributesList(attributes.Select(Attr));
    public static AttributeListList AttributesList(MethodInfo method)
        => AttributesList(method.GetCustomAttributes());

    //public static InvocationFromReflection Invocation2<TIn, TOut>(System.Linq.Expressions.Expression<Func<TIn, TOut>> expression, IExpression caller, IEnumerable<IExpression>? arguments = null)
    //    => GetModel(expression, caller, arguments);
    //=> Invocation(GetModel(expression), caller, arguments);
    //public static InvocationFromReflection GetModel(Expression expression) => expression switch
    ////public static InvocationFromReflection GetModel(Expression expression, IExpression caller, IEnumerable<IExpression>? arguments = null) => expression switch
    //{
    //    LambdaExpression lammbda => CodeModelFactory.Lambda(),
    //    System.Linq.Expressions.BinaryExpression e => null,
    //    _ => throw new NotImplementedException()
    //};
    //=> Invocation(ExpressionReflectionUtil.GetModel(expression), caller, arguments);

    //public static ICodeModel GetModel(LambdaExpression expression)
    //    => CodeModelFactory.Lambda(expression.Parameters.Select(x => GetModel(x)), Type(expression.Type), GetModel());
}
