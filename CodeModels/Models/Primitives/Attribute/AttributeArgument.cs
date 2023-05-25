using System.Collections.Generic;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record AttributeArgument(IExpression Expression, string? Name)
    : CodeModel<AttributeArgumentSyntax>
{
    public static AttributeArgument Create(IExpression expression, string? name = null)
        => new(expression, name);

    public override AttributeArgumentSyntax Syntax()
        => AttributeArgument(Name is null ? null : new NameEquals(Name).Syntax(), Name is null ? null : new NameColon(Name).Syntax(), Expression.Syntax());
    
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }
}