using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record AssignmentExpression(IExpression Left, IExpression Right, SyntaxKind Kind) : Expression<AssignmentExpressionSyntax>(Left.Get_Type())
{
    public override AssignmentExpressionSyntax Syntax() => AssignmentExpression(Kind, Left.Syntax(), Right.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Left;
        yield return Right;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        var result = CodeModelFactory.BinaryExpression(Left, GetOperationType(Kind), Right).Evaluate(context);
        context.SetValue(Left, result);
        return result;
    }

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
}

public record SimpleAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.SimpleAssignmentExpression) { }
public record AddAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.AddAssignmentExpression) { }
public record SubtractAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.SubtractAssignmentExpression) { }
public record MultiplyAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.MultiplyAssignmentExpression) { }
public record DivideAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.DivideAssignmentExpression) { }
public record ModuloAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.ModuloAssignmentExpression) { }
public record AndAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.AndAssignmentExpression) { }
public record ExclusiveOrAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.ExclusiveOrAssignmentExpression) { }
public record OrAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.OrAssignmentExpression) { }
public record LeftShiftAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.LeftShiftAssignmentExpression) { }
public record RightShiftAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.RightShiftAssignmentExpression) { }
public record CoalesceAssignmentExpression(IExpression Left, IExpression Right) : AssignmentExpression(Left, Right, SyntaxKind.CoalesceAssignmentExpression) { }
