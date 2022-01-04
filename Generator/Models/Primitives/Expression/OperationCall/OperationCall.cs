﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.DataTransformation;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using CodeAnalyzation.Models;

namespace CodeAnalyzation.Models
{
    public record OperationCall(IOperationPipeline Pipeline, IOperation? Operation, IEnumerable<IExpression> Inputs) 
        : Expression<ExpressionSyntax>(Pipeline.Type)
    {
        public OperationCall(IOperationPipeline pipeline, IEnumerable<IExpression> Inputs) : this(pipeline, pipeline.OutputNode.Operation, Inputs) { }
        public OperationCall(IOperationPipelineNode pipeline, IEnumerable<IExpression>? Inputs = null)
            : this(pipeline.OperationPipeline, pipeline.Operation, Inputs ?? pipeline.InputNodes.Select(x => new OperationCall(x))) { }
        public OperationCall(IOperationPipeline pipeline) : this(pipeline.OutputNode) { }

        public override ExpressionSyntax Syntax() => Pipeline != null ? Apply(Pipeline!.OutputNode) : Syntax(Inputs.Select(x => x.Syntax()));

        public ExpressionSyntax Syntax(IEnumerable<ExpressionSyntax> inputs) => Pipeline != null ? Apply(Pipeline!.OutputNode) :
            Operation!.OperationType switch
            {
                OperationType.None => throw new NotImplementedException(),
                OperationType.Method => Operation.IsStatic ? InvocationExpressionCustom(Operation.Name, inputs) : DottedInvocationExpressionCustom(Operation.Name, inputs),
                OperationType.Property => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, inputs.First(), IdentifierName(Operation.Name)),
                OperationType.Field => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, inputs.First(), IdentifierName(Operation.Name)),
                OperationType.Constructor => ConstructorCall(Operation.Output.Name, inputs),
                OperationType.Cast => CastExpression(ParseTypeName(Operation.Output.Name), inputs.First()),
                OperationType.Inheritance => inputs.First(),
                OperationType.Identity => inputs.First(),
                OperationType.Parenthesis => ParenthesizedExpression(inputs.First()),
                OperationType type when type.IsUnaryOperator() => GetUnaryIsPrefix(type) ? PrefixUnaryExpression(GetUnarySyntaxKind(type), inputs.First()) : PostfixUnaryExpression(GetUnarySyntaxKind(type), inputs.First()),
                OperationType type when type.IsBinaryOperator() => BinaryExpression(GetBinarySyntaxKind(type), inputs.First(), inputs.Last()),
                OperationType type when type.IsTernaryOperator() => ConditionalExpression(inputs.First(), inputs.Skip(1).First(), inputs.Last()),
                OperationType.Bracket => ElementAccessExpression(inputs.First(), BracketedArgumentList(Token(SyntaxKind.OpenBracketToken), SeparatedList(inputs.Skip(1).Select(x => Argument(x))), Token(SyntaxKind.CloseBracketToken))),
                OperationType.With => WithExpression(inputs.First(), InitializerExpression(SyntaxKind.WithInitializerExpression, SeparatedList(inputs.Skip(1)))),
                OperationType.Pipeline => Apply(Operation.OperationPipeline!.OutputNode),
                _ => throw new NotImplementedException(),
            };

        public SyntaxKind GetUnarySyntaxKind(OperationType operationType) => operationType switch
        {
            OperationType.Not => SyntaxKind.LogicalNotExpression,
            OperationType.Complement => SyntaxKind.BitwiseNotExpression,
            OperationType.UnaryAdd => SyntaxKind.UnaryPlusExpression,
            OperationType.UnaryAddBefore => SyntaxKind.PreIncrementExpression,
            OperationType.UnaryAddAfter => SyntaxKind.PostIncrementExpression,
            OperationType.UnarySubtract => SyntaxKind.UnaryMinusExpression,
            OperationType.UnarySubtractBefore => SyntaxKind.PreDecrementExpression,
            OperationType.UnarySubtractAfter => SyntaxKind.PostDecrementExpression,
            _ => throw new NotImplementedException()
        };

        public bool GetUnaryIsPrefix(OperationType operationType) => operationType switch
        {
            OperationType.Not => true,
            OperationType.Complement => true,
            OperationType.UnaryAdd => true,
            OperationType.UnaryAddBefore => true,
            OperationType.UnaryAddAfter => false,
            OperationType.UnarySubtract => true,
            OperationType.UnarySubtractBefore => true,
            OperationType.UnarySubtractAfter => false,
            _ => throw new NotImplementedException()
        };

        public SyntaxKind GetBinarySyntaxKind(OperationType operationType) => operationType switch
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
            OperationType.Is => SyntaxKind.IsExpression,
            OperationType.As => SyntaxKind.AsExpression,
            OperationType.Coalesce => SyntaxKind.CoalesceExpression,
            _ => throw new NotImplementedException()
        };

        public SyntaxKind AnyArgOperatorSyntaxKind(OperationType operationType) => operationType switch
        {
            OperationType.Bracket => SyntaxKind.BracketedArgumentList,
            _ => throw new NotImplementedException()
        };

        private ExpressionSyntax Apply(IOperationPipelineNode node, int argIndex = 0)
        {
            if (!Inputs.Any()) return new OperationCall(node).Syntax();
            var opParams = new List<ExpressionSyntax>();
            foreach (var input in node.InputNodes)
            {
                var inputCount = input.GetLeafInputCount();
                var arguments = Inputs.Skip(argIndex).Take(inputCount).ToArray();
                opParams.Add(new OperationCall(input, arguments).Syntax());

                argIndex += inputCount;
            }
            opParams.AddRange(Inputs.Skip(argIndex).Select(x => x.Syntax()));
            return new OperationCall(node).Syntax(opParams);
        }
    }
}