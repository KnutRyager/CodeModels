using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;


public record SimpleBaseType(IType Type)
    : BaseType<SimpleBaseTypeSyntax>(Type)
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override SimpleBaseTypeSyntax Syntax() => SyntaxFactory.SimpleBaseType(Type.Syntax());
}