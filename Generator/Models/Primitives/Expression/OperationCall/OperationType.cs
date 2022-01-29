namespace CodeAnalyzation.Models;

public enum OperationType
{
    None,
    Method,
    Property,
    Field,
    Constructor,
    Implementation,
    Inheritance,
    Pipeline,
    Identity,
    Provider,
    DefaultParameter,
    // Unary operations
    Parenthesis,
    Cast,
    Default,
    Typeof,
    Sizeof,
    Ref,
    AddressOf,
    PointerIndirection,
    Index,
    SuppressNullableWarning,
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
    ConditionalAccess,
    Assignment,
    Dot,
    Is,
    As,
    Coalesce,
    // Ternary operations
    Ternary,
    // Any arg operations
    Bracket,
    Invocation,
    // IDK
    With,
}
