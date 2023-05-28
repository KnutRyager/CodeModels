using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member;

public record ArgumentList(List<Argument> Arguments)
    : CodeModel<ArgumentListSyntax>(), IToArgumentListConvertible
{
    public static ArgumentList Create(IEnumerable<Argument>? Arguments = default)
        => new(List(Arguments));

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var argument in Arguments) yield return argument;
    }

    public override ArgumentListSyntax Syntax()
        => SyntaxFactory.ArgumentList(SeparatedList(Arguments.Select(x => x.Syntax())));

    public ArgumentList ToArgumentList() => this;

    public List<IExpression> Evaluate(ICodeModelExecutionContext context)
        => Arguments.Select(x => x.Expression.Evaluate(context)).ToList();
    public List<object?> EvaluatePlain(ICodeModelExecutionContext context)
        => Arguments.Select(x => x.Expression.EvaluatePlain(context)).ToList();
    public BracketedArgumentListSyntax ToBracketedListSyntax()
        => BracketedArgumentList(SeparatedList(Arguments.Select(x => x.Syntax())));
}
