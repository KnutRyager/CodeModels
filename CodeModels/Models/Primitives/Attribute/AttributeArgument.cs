using System.Collections.Generic;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record AttributeArgument(IExpression Expression, string? Name)
    : CodeModel<AttributeArgumentSyntax>, IToAttributeArgumentConvertible
{
    public static AttributeArgument Create(IExpression expression, string? name = null)
        => new(expression, name);

    public override AttributeArgumentSyntax Syntax()
        => AttributeArgument(null, Name is null ? null : CodeModelFactory.NameColon(Name).Syntax(), Expression.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public AttributeArgument ToAttributeArgument() => this;
    public AttributeArgumentList ToAttributeArgumentList() => CodeModelFactory.AttributeArgs(this);
}