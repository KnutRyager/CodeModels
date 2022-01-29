using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record IdentifierExpression(string Name, IType? Type = null) : Expression<IdentifierNameSyntax>(Type ?? TypeShorthands.NullType)
{
    public override IdentifierNameSyntax Syntax() => Syntax(Name ?? Type.Name);
    public IdentifierNameSyntax Syntax(string name) => IdentifierName(name);
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context) => context.GetValue(this);
}
