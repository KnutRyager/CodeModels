using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record LiteralExpression(IType Type, object? Value, string? SerializedValue) : Expression<LiteralExpressionSyntax>(Type)
{
    public static LiteralExpression Create(IType type, object? value = null, string? SerializedValue = null)
        => new(type, value, SerializedValue ?? "missing_json_serializer");
    public static LiteralExpression Create(object? value)
        => Create(TypeFromReflection.Create(value?.GetType() ?? typeof(object)), value);
    public static LiteralExpression Create(AbstractType type, string serializedValue)
        => Create(type, null, serializedValue);
    public static LiteralExpression Create(EnumMemberDeclarationSyntax value)
        => Create(TypeFromReflection.Create(typeof(string)), value.Identifier);

    public override LiteralExpressionSyntax LiteralSyntax() => ReferenceEquals(this, CodeModelFactory.VoidValue) ? null! : LiteralExpressionCustom(Value);
    public override object? LiteralValue() => Value;
    public override LiteralExpressionSyntax Syntax() => LiteralSyntax();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context) => this;
}
