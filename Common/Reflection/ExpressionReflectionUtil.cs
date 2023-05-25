using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Common.Util;

namespace Common.Reflection;

public static class ExpressionReflectionUtil
{
    public static FieldInfo GetFieldInfo<T>(Expression<Func<T, object>> expression)
        => GetMemberInfo(expression.Body) as FieldInfo ?? throw new NotImplementedException();
    public static FieldInfo GetFieldInfo(Expression<Func<object, object>> expression)
        => GetMemberInfo(expression.Body) as FieldInfo ?? throw new NotImplementedException();

    public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> expression)
        => GetMemberInfo(expression.Body) as PropertyInfo ?? throw new NotImplementedException();
    public static PropertyInfo GetPropertyInfo(Expression<Func<object, object>> expression)
        => GetMemberInfo(expression.Body) as PropertyInfo ?? throw new NotImplementedException();

    public static ConstructorInfo GetConstructorInfo(Expression<Func<object, object>> expression)
        => GetMemberInfo(expression.Body) as ConstructorInfo ?? throw new NotImplementedException();

    public static MethodInfo GetMethodInfo<T1, TOut>(Expression<Func<T1, TOut>> expression)
        => GetMemberInfo(expression.Body) as MethodInfo ?? throw new NotImplementedException();
    public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        => GetMemberInfo(expression.Body) as MethodInfo ?? throw new NotImplementedException();
    public static MethodInfo GetMethodInfo(Expression<Action<object>> expression)
        => GetMemberInfo(expression.Body) as MethodInfo ?? throw new NotImplementedException();

    public static MethodInfo GetMethodInfo<T>(Expression<Func<T, Delegate>> expression)
        => GetMethodInfo<T, Delegate>(expression);
    public static MethodInfo GetMethodInfo(Expression<Func<object, Delegate>> expression)
        => GetMethodInfo<object, Delegate>(expression);

    // See: https://learn.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression?view=net-7.0
    public static MemberInfo GetMemberInfo(Expression expression) => expression switch
    {
        NewExpression newExpression => newExpression.Constructor,
        MemberExpression memberExpression => memberExpression.Member,
        UnaryExpression unaryExpression => GetMemberInfo(unaryExpression),
        MethodCallExpression methodCallExpression => methodCallExpression.Method,
        ConstantExpression constantExpression when constantExpression.Value is MethodInfo methodInfo => methodInfo,
        _ => throw new NotImplementedException()
    };
    public static MemberInfo GetMemberInfo(UnaryExpression expression) => expression.Operand switch
    {
        MethodCallExpression methodCall => GetMemberInfo(methodCall.Object),
        ConstantExpression constantExpression => constantExpression.Value as MemberInfo ?? throw new NotImplementedException(),
        _ => throw new NotImplementedException()
    };


    public static string? GetModel(Expression<Func<object, object>> expression) => GetModel(expression.Body);


    public static string? GetModel(Expression expression) => expression switch
    {
        //BinaryExpression binaryExpression => $"{GetModel(binaryExpression.Left)}{GetModel(binaryExpression.NodeType)}{GetModel(binaryExpression.Right)}",
        BlockExpression blockExpression => null,
        ConditionalExpression conditionalExpression => null,
        ConstantExpression constantExpression => null,
        DebugInfoExpression debugInfoExpression => null,
        DefaultExpression defaultExpression => null,
        DynamicExpression dynamicExpression => null,
        GotoExpression gotoExpression => null,
        IndexExpression indexExpression => null,
        InvocationExpression invocationExpression => null,
        LabelExpression labelExpression => null,
        LambdaExpression lambdaExpression => null,
        ListInitExpression listInitExpression => null,
        LoopExpression loopExpression => null,
        MemberExpression memberExpression => null,
        MemberInitExpression memberInitExpression => null,
        MethodCallExpression methodCallExpression => null,
        NewArrayExpression newArrayExpression => null,
        NewExpression newExpression => null,
        ParameterExpression parameterExpression => null,
        RuntimeVariablesExpression runtimeVariablesExpression => null,
        SwitchExpression switchExpression => null,
        TryExpression tryExpression => null,
        TypeBinaryExpression typeBinaryExpression => null,
        UnaryExpression unaryExpression => null,
        _ => throw new NotImplementedException()
    };
}