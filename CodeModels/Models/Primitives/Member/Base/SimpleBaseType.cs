using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;


public record SimpleBaseType(IType Type)
    : BaseType<SimpleBaseTypeSyntax>(Type)
{
    public static SimpleBaseType Create(IType type) => new(type);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override SimpleBaseTypeSyntax Syntax() => SyntaxFactory.SimpleBaseType(Type.Syntax());
}