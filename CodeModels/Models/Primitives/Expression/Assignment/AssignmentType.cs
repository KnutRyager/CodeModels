using System;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeModels.Models;

public enum AssignmentType
{
    Simple,
    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,
    And,
    ExclusiveOr,
    Or,
    LeftShift,
    RightShift,
    Coalesce
}

public static class AssignmentTypeExtensions
{
    public static OperationType GetOperationType(this AssignmentType kind) => kind switch
    {
        AssignmentType.Simple => OperationType.Assignment,
        AssignmentType.Add => OperationType.Plus,
        AssignmentType.Subtract => OperationType.Subtract,
        AssignmentType.Multiply => OperationType.Multiply,
        AssignmentType.Divide => OperationType.Divide,
        AssignmentType.Modulo => OperationType.Modulo,
        AssignmentType.And => OperationType.LogicalAnd,
        AssignmentType.ExclusiveOr => OperationType.ExclusiveOr,
        AssignmentType.Or => OperationType.LogicalOr,
        AssignmentType.LeftShift => OperationType.LeftShift,
        AssignmentType.RightShift => OperationType.RightShift,
        AssignmentType.Coalesce => OperationType.Coalesce,
        _ => throw new NotImplementedException($"{kind}")
    };

    public static OperationType GetOperationType(SyntaxKind kind) => kind switch
    {
        SyntaxKind.SimpleAssignmentExpression => OperationType.Assignment,
        SyntaxKind.AddAssignmentExpression => OperationType.Plus,
        SyntaxKind.SubtractAssignmentExpression => OperationType.Subtract,
        SyntaxKind.MultiplyAssignmentExpression => OperationType.Multiply,
        SyntaxKind.DivideAssignmentExpression => OperationType.Divide,
        SyntaxKind.ModuloAssignmentExpression => OperationType.Modulo,
        SyntaxKind.AndAssignmentExpression => OperationType.LogicalAnd,
        SyntaxKind.ExclusiveOrAssignmentExpression => OperationType.ExclusiveOr,
        SyntaxKind.OrAssignmentExpression => OperationType.LogicalOr,
        SyntaxKind.LeftShiftAssignmentExpression => OperationType.LeftShift,
        SyntaxKind.RightShiftAssignmentExpression => OperationType.RightShift,
        SyntaxKind.CoalesceAssignmentExpression => OperationType.Coalesce,
        _ => throw new NotImplementedException($"{kind}")
    };

    public static AssignmentType GetAssignmentType(SyntaxKind kind) => kind switch
    {
        SyntaxKind.SimpleAssignmentExpression => AssignmentType.Simple,
        SyntaxKind.AddAssignmentExpression => AssignmentType.Add,
        SyntaxKind.SubtractAssignmentExpression => AssignmentType.Subtract,
        SyntaxKind.MultiplyAssignmentExpression => AssignmentType.Multiply,
        SyntaxKind.DivideAssignmentExpression => AssignmentType.Divide,
        SyntaxKind.ModuloAssignmentExpression => AssignmentType.Modulo,
        SyntaxKind.AndAssignmentExpression => AssignmentType.And,
        SyntaxKind.ExclusiveOrAssignmentExpression => AssignmentType.ExclusiveOr,
        SyntaxKind.OrAssignmentExpression => AssignmentType.Or,
        SyntaxKind.LeftShiftAssignmentExpression => AssignmentType.LeftShift,
        SyntaxKind.RightShiftAssignmentExpression => AssignmentType.RightShift,
        SyntaxKind.CoalesceAssignmentExpression => AssignmentType.Coalesce,
        _ => throw new NotImplementedException($"{kind}")
    };

    public static SyntaxKind Syntax(this AssignmentType assignmentType) => assignmentType switch
    {
        AssignmentType.Simple => SyntaxKind.SimpleAssignmentExpression,
        AssignmentType.Add => SyntaxKind.AddAssignmentExpression,
        AssignmentType.Subtract => SyntaxKind.SubtractAssignmentExpression,
        AssignmentType.Multiply => SyntaxKind.MultiplyAssignmentExpression,
        AssignmentType.Divide => SyntaxKind.DivideAssignmentExpression,
        AssignmentType.Modulo => SyntaxKind.ModuloAssignmentExpression,
        AssignmentType.And => SyntaxKind.AndAssignmentExpression,
        AssignmentType.ExclusiveOr => SyntaxKind.ExclusiveOrAssignmentExpression,
        AssignmentType.Or => SyntaxKind.OrAssignmentExpression,
        AssignmentType.LeftShift => SyntaxKind.LeftShiftAssignmentExpression,
        AssignmentType.RightShift => SyntaxKind.RightShiftAssignmentExpression,
        AssignmentType.Coalesce => SyntaxKind.CoalesceAssignmentExpression,
        _ => throw new NotImplementedException($"{assignmentType}")
    };
}
