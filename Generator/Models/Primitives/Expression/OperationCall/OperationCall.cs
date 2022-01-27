using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record OperationCall(IOperationPipeline Pipeline, IOperation? Operation, IEnumerable<IExpression> Inputs)
    : Expression<ExpressionSyntax>(Pipeline.Type)
{
    public OperationCall(IOperationPipeline pipeline, IEnumerable<IExpression> Inputs) : this(pipeline, pipeline.OutputNode.Operation, Inputs) { }
    public OperationCall(IOperationPipelineNode pipeline, IEnumerable<IExpression>? Inputs = null)
        : this(pipeline.OperationPipeline, pipeline.Operation, Inputs ?? pipeline.InputNodes.Select(x => new OperationCall(x))) { }
    public OperationCall(IOperationPipeline pipeline) : this(pipeline.OutputNode) { }

    public override ExpressionSyntax Syntax() => Pipeline is null ? Syntax(Inputs.Select(x => x.Syntax())) : Apply(Pipeline!.OutputNode);

    public ExpressionSyntax Syntax(IEnumerable<ExpressionSyntax> inputs) => Pipeline is not null ? Apply(Pipeline!.OutputNode) :
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
            OperationType type when type.IsUnaryOperator() => type.IsUnaryPrefix() ? PrefixUnaryExpression(type.UnarySyntaxKind(), inputs.First()) : PostfixUnaryExpression(type.UnarySyntaxKind(), inputs.First()),
            OperationType type when type.IsBinaryOperator() => BinaryExpression(type.BinarySyntaxKind(), inputs.First(), inputs.Last()),
            OperationType type when type.IsTernaryOperator() => ConditionalExpression(inputs.First(), inputs.Skip(1).First(), inputs.Last()),
            OperationType.Bracket => ElementAccessExpression(inputs.First(), BracketedArgumentList(Token(SyntaxKind.OpenBracketToken), SeparatedList(inputs.Skip(1).Select(x => Argument(x))), Token(SyntaxKind.CloseBracketToken))),
            OperationType.Invocation => ElementAccessExpression(inputs.First(), BracketedArgumentList(Token(SyntaxKind.OpenBracketToken), SeparatedList(inputs.Skip(1).Select(x => Argument(x))), Token(SyntaxKind.CloseBracketToken))),
            OperationType.With => WithExpression(inputs.First(), InitializerExpression(SyntaxKind.WithInitializerExpression, SeparatedList(inputs.Skip(1)))),
            OperationType.Pipeline => Apply(Operation.OperationPipeline!.OutputNode),
            _ => throw new NotImplementedException(),
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

    public override IEnumerable<ICodeModel> Children() => Inputs; // TODO: Remaining

    public override object? Evaluate(IProgramModelExecutionContext context) => throw new NotImplementedException();
}
