using System;
using Microsoft.CodeAnalysis.CSharp;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace CodeModels.Models;

public static class OperationTypeExtensions
{
    public static bool IsLanguageOperator(this OperationType operation)
        => operation.IsUnaryOperator() || operation.IsBinaryOperator() || operation.IsTernaryOperator() || operation.IsAnyArgOperator();

    public static bool IsUnaryOperator(this OperationType operation)
        => operation >= OperationType.Parenthesis && operation <= OperationType.UnarySubtractAfter;

    public static bool IsBinaryOperator(this OperationType operation)
        => operation >= OperationType.Plus && operation <= OperationType.Coalesce;

    public static bool IsTernaryOperator(this OperationType operation)
        => operation >= OperationType.Ternary && operation <= OperationType.Ternary;

    public static bool IsAnyArgOperator(this OperationType operation)
        => operation >= OperationType.Bracket && operation <= OperationType.With;

    public static string UnaryOperatorName(this OperationType operation) => operation switch
    {
        OperationType.Not => "!",
        OperationType.Complement => "~",
        OperationType.UnaryAdd => "+",
        OperationType.UnarySubtract => "-",
        OperationType.UnaryAddBefore => "++<>",
        OperationType.UnaryAddAfter => "<>++",
        OperationType.UnarySubtractBefore => "--<>",
        OperationType.UnarySubtractAfter => "<>--",
        OperationType.Default => "default",
        OperationType.Typeof => "typeof",
        OperationType.Sizeof => "sizeof",
        OperationType.Ref => "ref",
        OperationType.AddressOf => "addressof",
        OperationType.PointerIndirection => "*",
        OperationType.Index => "index",
        OperationType.SuppressNullableWarning => "!",
        _ => throw new NotImplementedException()
    };

    public static string BinaryOperatorName(this OperationType operation) => operation switch
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
        OperationType.Assignment => "=",
        OperationType.Dot => ".",
        OperationType.Is => "is",
        OperationType.As => "as",
        OperationType.Coalesce => "??",
        _ => throw new NotImplementedException()
    };

    public static string TernaryOperatorName(this OperationType operation) => operation switch
    {
        OperationType.Ternary => "?:",
        _ => throw new NotImplementedException()
    };

    public static string AnyArgOperatorName(this OperationType operation) => operation switch
    {
        OperationType.Bracket => "[]",
        OperationType.Invocation => "()",
        OperationType.With => "with",
        _ => throw new NotImplementedException()
    };

    public static ExpressionSyntax Syntax(this OperationType opType, IEnumerable<IExpression> inputs, IType operation, IOperationPipeline? pipeline = null) => pipeline != null ? Apply(pipeline!.OutputNode, inputs) :
      opType switch
      {
          OperationType.None => throw new NotImplementedException(),
          OperationType.Method => operation.IsStatic ? InvocationExpressionCustom(operation.Name, inputs.Select(x => x.Syntax()).ToArray()) : DottedInvocationExpressionCustom(operation.Name, inputs.Select(x => x.Syntax()).ToArray()),
          OperationType.Property => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, inputs.First().Syntax(), IdentifierName(operation.Name)),
          OperationType.Field => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, inputs.First().Syntax(), IdentifierName(operation.Name)),
          OperationType.Constructor => ConstructorCall(operation.Name, inputs.Select(x => x.Syntax()).ToArray()),
          OperationType.Cast => CastExpression(ParseTypeName(operation.Name), inputs.First().Syntax()),
          OperationType.Inheritance => inputs.First().Syntax(),
          OperationType.Identity => inputs.First().Syntax(),
          OperationType.Parenthesis => ParenthesizedExpression(inputs.First().Syntax()),
          //OperationType.Dot => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, inputs.First().Syntax(), Token(SyntaxKind.DotToken), inputs.Skip(1).First().Syntax()),
          OperationType type when type.IsUnaryOperator() => type.IsUnaryPrefix() ? PrefixUnaryExpression(type.UnarySyntaxKind(), inputs.First().Syntax()) : PostfixUnaryExpression(type.UnarySyntaxKind(), inputs.First().Syntax()),
          OperationType type when type.IsBinaryOperator() => BinaryExpression(type.BinarySyntaxKind(), inputs.First().Syntax(), inputs.Last().Syntax()),
          OperationType type when type.IsTernaryOperator() => ConditionalExpression(inputs.First().Syntax(), inputs.Skip(1).First().Syntax(), inputs.Last().Syntax()),
          OperationType.Bracket => ElementAccessExpression(inputs.First().Syntax(), BracketedArgumentList(Token(SyntaxKind.OpenBracketToken), SeparatedList(inputs.Skip(1).Select(x => x.Syntax()).Select(x => Argument(x))), Token(SyntaxKind.CloseBracketToken))),
          OperationType.Invocation => operation.IsStatic ? InvocationExpressionCustom(operation.Name, inputs.Select(x => x.Syntax()).ToArray()) : DottedInvocationExpressionCustom(operation.Name, inputs.Select(x => x.Syntax()).ToArray()),
          OperationType.With => WithExpression(inputs.First().Syntax(), InitializerExpression(SyntaxKind.WithInitializerExpression, SeparatedList(inputs.Skip(1).Select(x => x.Syntax())))),
          //OperationType.Pipeline => Apply(operation.OperationPipeline!.OutputNode, inputs),
          _ => throw new NotImplementedException(),
      };

    private static ExpressionSyntax Apply(IOperationPipelineNode node, IEnumerable<IExpression> inputs, int argIndex = 0)
    {
        if (!inputs.Any()) return new OperationCall(node).Syntax();
        var opParams = new List<ExpressionSyntax>();
        foreach (var input in node.InputNodes)
        {
            var inputCount = input.GetLeafInputCount();
            var arguments = inputs.Skip(argIndex).Take(inputCount).ToArray();
            opParams.Add(new OperationCall(input, arguments).Syntax());

            argIndex += inputCount;
        }
        opParams.AddRange(inputs.Skip(argIndex).Select(x => x.Syntax()));
        return new OperationCall(node).Syntax(opParams);
    }

    public static SyntaxKind UnarySyntaxKind(this OperationType operation) => operation switch
    {
        OperationType.Not => SyntaxKind.LogicalNotExpression,
        OperationType.Complement => SyntaxKind.BitwiseNotExpression,
        OperationType.UnaryAdd => SyntaxKind.UnaryPlusExpression,
        OperationType.UnaryAddBefore => SyntaxKind.PreIncrementExpression,
        OperationType.UnaryAddAfter => SyntaxKind.PostIncrementExpression,
        OperationType.UnarySubtract => SyntaxKind.UnaryMinusExpression,
        OperationType.UnarySubtractBefore => SyntaxKind.PreDecrementExpression,
        OperationType.UnarySubtractAfter => SyntaxKind.PostDecrementExpression,
        OperationType.Default => SyntaxKind.DefaultExpression,
        OperationType.Typeof => SyntaxKind.TypeOfExpression,
        OperationType.Sizeof => SyntaxKind.SizeOfExpression,
        OperationType.Ref => SyntaxKind.RefExpression,
        OperationType.AddressOf => SyntaxKind.AddressOfExpression,
        OperationType.PointerIndirection => SyntaxKind.PointerIndirectionExpression,
        OperationType.Index => SyntaxKind.IndexExpression,
        OperationType.SuppressNullableWarning => SyntaxKind.SuppressNullableWarningExpression,
        _ => throw new NotImplementedException()
    };

    public static bool IsUnaryPrefix(this OperationType operation) => operation switch
    {
        OperationType.Not => true,
        OperationType.Complement => true,
        OperationType.UnaryAdd => true,
        OperationType.UnaryAddBefore => true,
        OperationType.UnaryAddAfter => false,
        OperationType.UnarySubtract => true,
        OperationType.UnarySubtractBefore => true,
        OperationType.UnarySubtractAfter => false,
        OperationType.Default => true,
        OperationType.Typeof => true,
        OperationType.Sizeof => true,
        OperationType.Ref => true,
        OperationType.AddressOf => true,
        OperationType.PointerIndirection => true,
        OperationType.Index => false,
        OperationType.SuppressNullableWarning => false,
        _ => throw new NotImplementedException()
    };

    public static SyntaxKind BinarySyntaxKind(this OperationType operation) => operation switch
    {
        OperationType.Plus => SyntaxKind.AddExpression,
        OperationType.Subtract => SyntaxKind.SubtractExpression,
        OperationType.Multiply => SyntaxKind.MultiplyExpression,
        OperationType.Divide => SyntaxKind.DivideExpression,
        OperationType.Modulo => SyntaxKind.ModuloExpression,
        OperationType.Equals => SyntaxKind.EqualsEqualsToken,
        OperationType.NotEquals => SyntaxKind.NotEqualsExpression,
        OperationType.GreaterThan => SyntaxKind.GreaterThanExpression,
        OperationType.GreaterThanOrEqual => SyntaxKind.GreaterThanOrEqualExpression,
        OperationType.LessThan => SyntaxKind.LessThanExpression,
        OperationType.LessThanOrEqual => SyntaxKind.LessThanOrEqualExpression,
        OperationType.LogicalAnd => SyntaxKind.LogicalAndExpression,
        OperationType.LogicalOr => SyntaxKind.LogicalOrExpression,
        OperationType.BitwiseAnd => SyntaxKind.BitwiseAndExpression,
        OperationType.BitwiseOr => SyntaxKind.BitwiseOrExpression,
        OperationType.ExclusiveOr => SyntaxKind.ExclusiveOrExpression,
        OperationType.LeftShift => SyntaxKind.LeftShiftExpression,
        OperationType.RightShift => SyntaxKind.RightShiftExpression,
        OperationType.Assignment => SyntaxKind.SimpleAssignmentExpression,
        OperationType.Dot => SyntaxKind.SimpleMemberAccessExpression,
        OperationType.Is => SyntaxKind.IsExpression,
        OperationType.As => SyntaxKind.AsExpression,
        OperationType.Coalesce => SyntaxKind.CoalesceExpression,
        _ => throw new NotImplementedException()
    };

    public static SyntaxKind AnyArgOperatorSyntaxKind(this OperationType operation) => operation switch
    {
        OperationType.Bracket => SyntaxKind.BracketedArgumentList,
        _ => throw new NotImplementedException()
    };

    public static object? ApplyOperator(this OperationType operation, params object?[] args) => operation switch
    {
        //OperationType.Method => Invoker.Invoke(new[] { Method }, instance, args),
        //OperationType.Constructor => Invoker.InvokeStatic(Output.PrimitiveType, "new", args),
        //OperationType.Property => Property!.GetValue(instance)!,
        //OperationType.Field => Field!.GetValue(instance)!,
        //OperationType.Inheritance => args[0],
        //OperationType.Pipeline => OperationPipeline!.Apply(args),
        OperationType.Identity => args[0],
        OperationType.Implementation => args[0],
        OperationType.Provider => args[0],
        OperationType.Parenthesis => args[0],
        _ when operation.IsUnaryOperator() => operation.ApplyUnaryOperator(args),
        _ when operation.IsBinaryOperator() => operation.ApplyBinaryOperator(args),
        _ when operation.IsTernaryOperator() => operation.ApplyTernaryOperator(args),
        _ when operation.IsAnyArgOperator() => operation.ApplyAnyArgOperator(args),
        _ => throw new NotImplementedException()
    };

    public static object? ApplyUnaryOperator(this OperationType operation, params object?[] args) => operation switch
    {
        OperationType.Not => !(dynamic)args[0]!,
        OperationType.Complement => ~(dynamic)args[0]!,
        OperationType.UnaryAdd => +(dynamic)args[0]!,
        OperationType.UnaryAddBefore => (dynamic)args[0]! + 1,
        OperationType.UnaryAddAfter => (dynamic)args[0]! + 1,    // TODO: Wrong return
        OperationType.UnarySubtract => -(dynamic)args[0]!,
        OperationType.UnarySubtractBefore => (dynamic)args[0]! - 1,
        OperationType.UnarySubtractAfter => (dynamic)args[0]! - 1,    // TODO: Wrong return
        //OperationType.Cast => Convert.ChangeType(args[0], Output.PrimitiveType ?? throw new Exception("Cannot cast operation as there is no primitiveType.")),
        OperationType.Default => Default(args[0]),
        OperationType.Typeof => (args[0])?.GetType(),
        OperationType.Sizeof => throw new NotImplementedException(),
        OperationType.Ref => (dynamic)args[0]!,
        OperationType.AddressOf => throw new NotImplementedException(),
        OperationType.PointerIndirection => throw new NotImplementedException(),
        OperationType.Index => throw new NotImplementedException(),
        OperationType.SuppressNullableWarning => args[0],
        _ => throw new NotImplementedException()
    };

    public static object? Default(object? o) => o?.GetType() switch
    {
        null => null,
        _ when o.GetType().IsValueType => Activator.CreateInstance(o.GetType()),
        _ => throw new NotImplementedException()
    };

    public static object? ApplyBinaryOperator(this OperationType operation, params object?[] args) => operation switch
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
        OperationType.Assignment => (dynamic)args[1]!,
        OperationType.Is => ((dynamic)args[0]!)?.GetType().IsAssignableFrom((dynamic)args[1]!) ?? false,
        OperationType.As => ((dynamic)args[0]!)?.GetType().IsAssignableFrom((dynamic)args[1]!) ? args[0] : null,
        OperationType.Coalesce => (dynamic)args[0]! ?? (dynamic)args[1]!,
        _ => throw new NotImplementedException()
    };

    public static object? ApplyTernaryOperator(this OperationType operation, params object?[] args) => operation switch
    {
        OperationType.Ternary => (dynamic)args[0]! ? (dynamic)args[1]! : (dynamic)args[2]!,
        _ => throw new NotImplementedException()
    };

    public static object? ApplyAnyArgOperator(this OperationType operation, params object?[] args) => operation switch
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
