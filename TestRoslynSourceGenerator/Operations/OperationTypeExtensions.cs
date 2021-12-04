namespace TheEverythingAPI.DataTransformation;

public static class OperationTypeExtensions
{
    public static bool IsLanguageOperator(this OperationType operationType)
        => operationType.IsUnaryOperator() || operationType.IsBinaryOperator() || operationType.IsTernaryOperator() || operationType.IsAnyArgOperator();

    public static bool IsUnaryOperator(this OperationType operationType)
        => operationType >= OperationType.Not && operationType <= OperationType.UnarySubtractAfter;

    public static bool IsBinaryOperator(this OperationType operationType)
        => operationType >= OperationType.Plus && operationType <= OperationType.Coalesce;

    public static bool IsTernaryOperator(this OperationType operationType)
        => operationType >= OperationType.Ternary && operationType <= OperationType.Ternary;

    public static bool IsAnyArgOperator(this OperationType operationType)
        => operationType >= OperationType.Bracket && operationType <= OperationType.With;

    public static string UnaryOperatorName(this OperationType operationType) => operationType switch
    {
        OperationType.Not => "!",
        OperationType.Complement => "~",
        OperationType.UnaryAdd => "+",
        OperationType.UnarySubtract => "-",
        OperationType.UnaryAddBefore => "++<>",
        OperationType.UnaryAddAfter => "<>++",
        OperationType.UnarySubtractBefore => "--<>",
        OperationType.UnarySubtractAfter => "<>--",
        _ => throw new NotImplementedException()
    };

    public static string BinaryOperatorName(this OperationType operationType) => operationType switch
    {
        OperationType.Plus => "+",
        OperationType.Subtract => "-",
        OperationType.Multiply => "*",
        OperationType.Divide => "/",
        OperationType.Modulo => "%",
        OperationType.Equals => "==",
        OperationType.NotEquals => "!=",
        OperationType.GreaterThan => ">",
        OperationType.GreaterThanOrEqual => ">=",
        OperationType.LessThan => "<",
        OperationType.LessThanOrEqual => "<=",
        OperationType.LogicalAnd => "&&",
        OperationType.LogicalOr => "||",
        OperationType.BitwiseAnd => "&",
        OperationType.BitwiseOr => "|",
        OperationType.ExclusiveOr => "^",
        OperationType.LeftShift => "<<",
        OperationType.RightShift => ">>",
        OperationType.Is => "is",
        OperationType.As => "as",
        OperationType.Coalesce => "??",
        _ => throw new NotImplementedException()
    };

    public static string TernaryOperatorName(this OperationType operationType) => operationType switch
    {
        OperationType.Ternary => "?:",
        _ => throw new NotImplementedException()
    };

    public static string AnyArgOperatorName(this OperationType operationType) => operationType switch
    {
        OperationType.Bracket => "[]",
        OperationType.With => "with",
        _ => throw new NotImplementedException()
    };

    public static object? ApplyUnaryOperator(this OperationType operationType, params object?[] args) => operationType switch
    {
        OperationType.Not => !(dynamic)args[0]!,
        OperationType.Complement => ~(dynamic)args[0]!,
        OperationType.UnaryAdd => +(dynamic)args[0]!,
        OperationType.UnaryAddBefore => new NotImplementedException(),
        OperationType.UnaryAddAfter => new NotImplementedException(),
        OperationType.UnarySubtract => -(dynamic)args[0]!,
        OperationType.UnarySubtractBefore => new NotImplementedException(),
        OperationType.UnarySubtractAfter => new NotImplementedException(),
        _ => throw new NotImplementedException()
    };

    public static object? ApplyBinaryOperator(this OperationType operationType, params object?[] args) => operationType switch
    {
        OperationType.Plus => (dynamic)args[0]! + (dynamic)args[1]!,
        OperationType.Subtract => (dynamic)args[0]! - (dynamic)args[1]!,
        OperationType.Multiply => (dynamic)args[0]! * (dynamic)args[1]!,
        OperationType.Divide => (dynamic)args[0]! / (dynamic)args[1]!,
        OperationType.Modulo => (dynamic)args[0]! % (dynamic)args[1]!,
        OperationType.Equals => (dynamic)args[0]! == (dynamic)args[1]!,
        OperationType.NotEquals => (dynamic)args[0]! != (dynamic)args[1]!,
        OperationType.GreaterThan => (dynamic)args[0]! > (dynamic)args[1]!,
        OperationType.GreaterThanOrEqual => (dynamic)args[0]! >= (dynamic)args[1]!,
        OperationType.LessThan => (dynamic)args[0]! < (dynamic)args[1]!,
        OperationType.LessThanOrEqual => (dynamic)args[0]! <= (dynamic)args[1]!,
        OperationType.LogicalAnd => (dynamic)args[0]! && (dynamic)args[1]!,
        OperationType.LogicalOr => (dynamic)args[0]! || (dynamic)args[1]!,
        OperationType.BitwiseAnd => (dynamic)args[0]! & (dynamic)args[1]!,
        OperationType.BitwiseOr => (dynamic)args[0]! | (dynamic)args[1]!,
        OperationType.ExclusiveOr => (dynamic)args[0]! ^ (dynamic)args[1]!,
        OperationType.LeftShift => (dynamic)args[0]! << (dynamic)args[1]!,
        OperationType.RightShift => (dynamic)args[0]! >> (dynamic)args[1]!,
        OperationType.Is => ((dynamic)args[0]!)?.GetType().IsAssignableFrom((dynamic)args[1]!) ?? false,
        OperationType.As => ((dynamic)args[0]!)?.GetType().IsAssignableFrom((dynamic)args[1]!) ? args[0] : null,
        OperationType.Coalesce => (dynamic)args[0]! ?? (dynamic)args[1]!,
        _ => throw new NotImplementedException()
    };

    public static object? ApplyTernaryOperator(this OperationType operationType, params object?[] args) => operationType switch
    {
        OperationType.Ternary => (dynamic)args[0]! ? (dynamic)args[1]! : (dynamic)args[2]!,
        _ => throw new NotImplementedException()
    };

    public static object? ApplyAnyArgOperator(this OperationType operationType, params object?[] args) => operationType switch
    {
        OperationType.Bracket => args.Length switch
        {
            2 => ((dynamic)args[0]!)[(dynamic)args[1]!],
            3 => ((dynamic)args[0]!)[(dynamic)args[1]!, (dynamic)args[2]!],
            4 => ((dynamic)args[0]!)[(dynamic)args[1]!, (dynamic)args[2]!, (dynamic)args[3]!],
            5 => ((dynamic)args[0]!)[(dynamic)args[1]!, (dynamic)args[2]!, (dynamic)args[3]!, (dynamic)args[4]!],
            6 => ((dynamic)args[0]!)[(dynamic)args[1]!, (dynamic)args[2]!, (dynamic)args[3]!, (dynamic)args[4]!, (dynamic)args[5]!],
            7 => ((dynamic)args[0]!)[(dynamic)args[1]!, (dynamic)args[2]!, (dynamic)args[3]!, (dynamic)args[4]!, (dynamic)args[5]!, (dynamic)args[6]!],
            _ => throw new NotImplementedException()
        },
        OperationType.With => throw new NotImplementedException(),
        _ => throw new NotImplementedException()
    };
}
