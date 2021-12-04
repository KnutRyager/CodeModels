using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Common.Util;

public static class ExpressionUtil
{
    public static Expression<Func<T, TProp>> MakeGetter<T, TProp>(PropertyInfo property)
        => (Expression<Func<T, TProp>>)MakeGetter(typeof(T), property);

    public static Expression<Func<T, object?>> MakeGetter<T>(PropertyInfo property)
        => (Expression<Func<T, object?>>)MakeGetter(typeof(T), property, typeof(object));

    public static LambdaExpression MakeGetter(Type type, PropertyInfo property, Type? propertyType = null)
    {
        if (propertyType == null) propertyType = property.PropertyType;
        var parameter = Parameter(type, "x");
        var memberExpression = CastIfNeeded(PropertyOrField(parameter, property.Name), property.PropertyType, propertyType);
        var lambdaType = typeof(Func<,>).MakeGenericType(type, propertyType);
        return Lambda(lambdaType, memberExpression, parameter);
    }

    public static Action<T, TProp> MakeSetter<T, TProp>(Expression<Func<T, TProp>> get)
    {
        var param = Parameter(typeof(TProp), "value");
        return Lambda<Action<T, TProp>>(Assign((MemberExpression)get.Body, param), get.Parameters[0], param).Compile();
    }

    public static Action<T, object?> MakeSetter<T>(PropertyInfo property)
        => (Action<T, object?>)MakeSetter(typeof(T), property, typeof(object));

    public static Delegate MakeSetter(Type type, PropertyInfo property, Type? propertyType = null)
    {
        if (propertyType == null) propertyType = property.PropertyType;
        var parameter = Parameter(type, "x");
        MemberExpression member = PropertyOrField(parameter, property.Name);
        var param = Parameter(propertyType, "value");
        var lambdaType = typeof(Action<,>).MakeGenericType(type, propertyType);
        var casted = CastIfNeeded(param, propertyType, property.PropertyType);
        return Lambda(lambdaType, Assign(member, casted), parameter, param).Compile();
    }

    public static PropertyInfo GetPropertyFromExpression<T, TReturn>(Expression<Func<T, TReturn>> GetPropertyLambda) => (PropertyInfo)(GetPropertyLambda.Body switch
    {
        UnaryExpression e => e switch
        {
            _ when e.Operand is MemberExpression expression => expression.Member,
            _ => throw new ArgumentException()
        },
        MemberExpression e => e.Member,
        _ => throw new ArgumentException()
    });

    public static MethodInfo GetMethodFromExpression<T, TReturn>(Expression<Func<T, TReturn>> methodCall) => methodCall.Body switch
    {
        _ when methodCall.Body is MethodCallExpression method => method.Method,
        _ => throw new ArgumentException()
    };

    public static string GetPropertyNameFromExpression<T, TReturn>(Expression<Func<T, TReturn>> GetPropertyLambda) => GetPropertyFromExpression(GetPropertyLambda).Name;

    public static Expression<Func<T, bool>> LambdaMethodPredicate<T, TMethod, TProperty>(TMethod instance, Expression<Func<TMethod, bool>> methodCall, Expression<Func<T, TProperty>> property)
        => LambdaMethodPredicate<T, TMethod>(instance, GetMethodFromExpression(methodCall), GetPropertyFromExpression(property));

    private static Expression<Func<T, bool>> LambdaMethodPredicate<T, TMethod>(TMethod instance, MethodInfo method, PropertyInfo property)
    {
        var xParameter = Parameter(typeof(T), "x");
        var propertyAccess = Property(xParameter, property);
        var predicate = Call(Constant(instance), method, propertyAccess);
        return (Expression<Func<T, bool>>)Lambda(predicate, xParameter);
    }

    public static Expression<Func<T, bool>> LambdaEquals<T, TValue>(TValue value, Expression<Func<T, TValue>> property)
        => LambdaEquals<T, TValue>(value, GetPropertyFromExpression(property));

    private static Expression<Func<T, bool>> LambdaEquals<T, TValue>(TValue value, PropertyInfo property)
    {
        var xParameter = Parameter(typeof(T), "x");
        var propertyAccess = Property(xParameter, property);
        var eq = Equal(Constant(value), propertyAccess);
        return (Expression<Func<T, bool>>)Lambda(eq, xParameter);
    }

    private static Expression CastIfNeeded(Expression exp, Type type, Type? optionalOverride = null)
        => optionalOverride == type ? exp : optionalOverride == null ? exp : Convert(exp, optionalOverride);
}
