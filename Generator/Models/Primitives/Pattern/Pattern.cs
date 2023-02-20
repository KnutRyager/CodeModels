using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public abstract record Pattern<T>() : CodeModel<T>, IPattern<T>
    where T : PatternSyntax
{
    public IExpression Evaluate(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }

    public object? EvaluatePlain(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }

    public IdentifierExpression GetIdentifier()
    {
        throw new System.NotImplementedException();
    }

    public IType Get_Type()
    {
        throw new System.NotImplementedException();
    }

    PatternSyntax IPattern.Syntax() => Syntax();

    ExpressionOrPatternSyntax IExpressionOrPattern.Syntax()
    {
        throw new System.NotImplementedException();
    }
}