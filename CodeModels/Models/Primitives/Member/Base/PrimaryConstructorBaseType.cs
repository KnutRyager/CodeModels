using System.Collections.Generic;
using System.Linq;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record PrimaryConstructorBaseType(IType Type, List<Argument> Arguments)
    : BaseType<PrimaryConstructorBaseTypeSyntax>(Type)
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        foreach (var argument in Arguments) yield return argument;
    }

    public override PrimaryConstructorBaseTypeSyntax Syntax()
        => SyntaxFactory.PrimaryConstructorBaseType(Type.Syntax(), 
            SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(Arguments.Select(x => x.Syntax()))));
}
