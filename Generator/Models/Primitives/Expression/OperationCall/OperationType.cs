namespace CodeAnalyzation.Models
{
    public enum OperationType
    {
        None,
        Method,
        Property,
        Field,
        Constructor,
        Implementation,
        Inheritance,
        Cast,
        Pipeline,
        Identity,
        Provider,
        Parenthesis,
        DefaultParameter,
        // Unary operations
        Not,
        Complement,
        UnaryAdd,
        UnaryAddBefore,
        UnaryAddAfter,
        UnarySubtract,
        UnarySubtractBefore,
        UnarySubtractAfter,
        // Binary operations
        Plus,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        LogicalAnd,
        LogicalOr,
        BitwiseAnd,
        BitwiseOr,
        ExclusiveOr,
        LeftShift,
        RightShift,
        Is,
        As,
        Coalesce,
        // Ternary operations
        Ternary,
        // Any arg operations
        Bracket,
        // IDK
        With,
    }
}