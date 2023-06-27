using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using Common.Reflection;

namespace CodeModels.Factory;

public static class CodeModelsFromExpression
{
    public static string GetCode(Expression expression) => GetModel(expression).Code();
    public static string GetCode<T>(System.Linq.Expressions.Expression<Func<T, Delegate>> expression) => GetModel(expression).Code();
    public static string GetCode(System.Linq.Expressions.Expression<Func<object, object?>> expression) => GetExpression(expression).Code();

    public static IExpression GetExpression(System.Linq.Expressions.Expression<Func<object, object?>> expression) => GetExpression(expression.Body);

    public static Models.ICodeModel GetModel(Expression? expression) => expression switch
    {
        BinaryExpression binaryExpression => GetModel(binaryExpression),
        BlockExpression blockExpression => GetModel(blockExpression),
        ConditionalExpression conditionalExpression => GetModel(conditionalExpression),
        ConstantExpression constantExpression => GetModel(constantExpression),
        DebugInfoExpression debugInfoExpression => GetModel(debugInfoExpression),
        DefaultExpression defaultExpression => GetModel(defaultExpression),
        DynamicExpression dynamicExpression => GetModel(dynamicExpression),
        GotoExpression gotoExpression => GetModel(gotoExpression),
        IndexExpression indexExpression => GetModel(indexExpression),
        InvocationExpression invocationExpression => GetModel(invocationExpression),
        LabelExpression labelExpression => GetModel(labelExpression),
        LambdaExpression lambdaExpression => GetModel(lambdaExpression),
        ListInitExpression listInitExpression => GetModel(listInitExpression),
        LoopExpression loopExpression => GetModel(loopExpression),
        MemberExpression memberExpression => GetModel(memberExpression),
        MemberInitExpression memberInitExpression => GetModel(memberInitExpression),
        MethodCallExpression methodCallExpression => GetModel(methodCallExpression),
        NewArrayExpression newArrayExpression => GetModel(newArrayExpression),
        NewExpression newExpression => GetModel(newExpression),
        ParameterExpression parameterExpression => GetArgModel(parameterExpression),
        RuntimeVariablesExpression runtimeVariablesExpression => GetModel(runtimeVariablesExpression),
        SwitchExpression switchExpression => GetModel(switchExpression),
        TryExpression tryExpression => GetModel(tryExpression),
        TypeBinaryExpression typeBinaryExpression => GetModel(typeBinaryExpression),
        UnaryExpression unaryExpression => GetModel(unaryExpression),
        null => CodeModelFactory.NullValue,
        _ => throw new NotImplementedException()
    };

    public static IExpression GetModel(BlockExpression expression)
        => throw new NotImplementedException();
    //=> CodeModelFactory.Block(expression.Variables.Select(GetModel));

    public static IExpression GetModel(ConditionalExpression expression)
        => CodeModelFactory.TernaryExpression(GetExpression(expression.Test), GetExpression(expression.IfTrue), GetExpression(expression.IfFalse));

    public static IExpression GetModel(ConstantExpression expression) => expression switch
    {
        _ when expression.Value is Enum e => CodeModelFactory.Identifier(e.GetType()).Access(CodeModelFactory.Identifier(e.ToString())),
        _ => CodeModelFactory.Literal(expression.Value)
    };

    public static IExpression GetModel(DebugInfoExpression expression) => throw new NotImplementedException();

    public static IExpression GetModel(DefaultExpression expression)
        => CodeModelFactory.Default(CodeModelFactory.Type(expression.Type));

    public static IExpression GetModel(DynamicExpression expression) => throw new NotImplementedException();
    public static IExpression GetModel(GotoExpression expression) => throw new NotImplementedException();
    public static IExpression GetModel(IndexExpression expression) => throw new NotImplementedException();
    public static IExpression GetModel(InvocationExpression expression) => throw new NotImplementedException();
    public static IExpression GetModel(LabelExpression expression) => throw new NotImplementedException();
    public static Models.ILambdaExpression GetModel(LambdaExpression expression)
        => CodeModelFactory.Lambda(CodeModelFactory.ParamList(expression.Parameters.Select(GetModel)),
            CodeModelFactory.Type(expression.Type),
            GetExpression(expression.Body));

    public static IExpression GetModel(ListInitExpression expression) => throw new NotImplementedException();
    public static IExpression GetModel(LoopExpression expression) => throw new NotImplementedException();

    public static IExpression GetModel(MemberExpression expression)
        => expression.Expression is ConstantExpression constantExpression && ReflectionUtil.IsStatic(constantExpression.Type)
            || expression.Expression is ParameterExpression
            ? CodeModelFactory.MemberAccess(GetExpression(expression.Expression), CodeModelFactory.Identifier(expression.Member.Name))
            : CodeModelFactory.Identifier(expression.Member.Name);

    public static IExpression GetModel(MemberInitExpression expression) => throw new NotImplementedException();
    public static IExpression GetModel(MethodCallExpression expression)
        => CodeModelsFromReflection.Invocation(expression.Method,
            expression.Object is null
            ? ReflectionUtil.IsExtension(expression.Method)
                ? GetExpression(expression.Arguments.First())
                : CodeModelFactory.Type(expression.Method.DeclaringType)
            : GetExpression(expression.Object),
            ReflectionUtil.IsExtension(expression.Method)
            ? expression.Arguments.Skip(1).Select(GetExpression)
            : expression.Arguments.Select(GetExpression));

    public static IExpression GetModel(NewArrayExpression expression)
    => CodeModelFactory.ObjectCreation(
        CodeModelFactory.Type(expression.Type),
        null,
        CodeModelFactory.ArrayInitializer(expression.Expressions.Select(GetExpression).ToArray()));

    public static IExpression GetModel(NewExpression expression)
        => CodeModelFactory.ObjectCreation(
            CodeModelFactory.Type(expression.Type),
            expression.Arguments.Select(GetExpression).ToArray());

    public static Parameter GetModel(ParameterExpression expression)
        => CodeModelFactory.Param(expression.Name, CodeModelFactory.Type(expression.Type));
    public static Argument GetArgModel(ParameterExpression expression)
        => CodeModelFactory.Arg(CodeModelFactory.Identifier(expression.Name));

    public static IExpression GetModel(RuntimeVariablesExpression expression) => throw new NotImplementedException();
    public static IExpression GetModel(SwitchExpression expression) => throw new NotImplementedException();
    public static IExpression GetModel(TryExpression expression) => throw new NotImplementedException();
    public static IExpression GetModel(TypeBinaryExpression expression) => throw new NotImplementedException();

    public static IExpression GetModel(UnaryExpression expression) => expression.NodeType switch
    {
        ExpressionType.Convert when expression is { Operand: UnaryExpression inner } && inner.NodeType is ExpressionType.Convert
            => CodeModelFactory.UnaryExpression(GetExpression(inner.Operand), GetUnaryExpressionType(inner.NodeType), CodeModelFactory.Type(inner.Type)),
        ExpressionType.Convert => GetExpression(expression.Operand),
        _ => CodeModelFactory.UnaryExpression(GetExpression(expression.Operand), GetUnaryExpressionType(expression.NodeType), CodeModelFactory.Type(expression.Type))
    };

    public static IExpression GetModel(BinaryExpression expression)
        => CodeModelFactory.BinaryExpression(GetExpression(expression.Left), GetBinaryExpressionType(expression.NodeType), GetExpression(expression.Right));

    public static IExpression GetExpression(Expression expression)
        => GetModel(expression) as IExpression ?? throw new NotImplementedException();

    public static Models.OperationType GetUnaryExpressionType(ExpressionType expressionType) => expressionType switch
    {
        //ExpressionType.ArrayLength => Models.OperationType.AddressOf,
        ExpressionType.Convert => Models.OperationType.Cast,
        ExpressionType.ConvertChecked => Models.OperationType.Cast,
        ExpressionType.Negate => Models.OperationType.UnarySubtract,
        ExpressionType.NegateChecked => Models.OperationType.UnarySubtract,
        ExpressionType.Not => Models.OperationType.Not,
        //ExpressionType.Quote => Models.OperationType.,
        ExpressionType.TypeAs => Models.OperationType.As,
        ExpressionType.UnaryPlus => Models.OperationType.UnaryAdd,

        _ => throw new NotImplementedException()
    };

    public static Models.OperationType GetBinaryExpressionType(ExpressionType expressionType) => expressionType switch
    {
        ExpressionType.Add => Models.OperationType.Plus,
        ExpressionType.AddChecked => Models.OperationType.Plus,
        ExpressionType.Divide => Models.OperationType.Divide,
        ExpressionType.Modulo => Models.OperationType.Modulo,
        ExpressionType.Multiply => Models.OperationType.Multiply,
        ExpressionType.MultiplyChecked => Models.OperationType.Multiply,
        ExpressionType.Power => Models.OperationType.None,
        ExpressionType.Subtract => Models.OperationType.Subtract,
        ExpressionType.SubtractChecked => Models.OperationType.Subtract,

        ExpressionType.And => Models.OperationType.LogicalAnd,
        ExpressionType.Or => Models.OperationType.LogicalOr,
        ExpressionType.ExclusiveOr => Models.OperationType.ExclusiveOr,

        ExpressionType.LeftShift => Models.OperationType.LeftShift,
        ExpressionType.RightShift => Models.OperationType.RightShift,

        ExpressionType.AndAlso => Models.OperationType.LogicalAnd,
        ExpressionType.OrElse => Models.OperationType.LogicalOr,

        ExpressionType.Equal => Models.OperationType.Equals,
        ExpressionType.NotEqual => Models.OperationType.NotEquals,
        ExpressionType.GreaterThanOrEqual => Models.OperationType.GreaterThanOrEqual,
        ExpressionType.GreaterThan => Models.OperationType.GreaterThan,
        ExpressionType.LessThan => Models.OperationType.LessThan,
        ExpressionType.LessThanOrEqual => Models.OperationType.LessThanOrEqual,

        ExpressionType.Coalesce => Models.OperationType.Coalesce,

        ExpressionType.ArrayIndex => throw new NotImplementedException(),
        //ExpressionType.ArrayIndex => Models.OperationType.arrayIndex,

        _ => throw new NotImplementedException()
    };
}
