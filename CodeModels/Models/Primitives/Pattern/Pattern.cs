using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public abstract record Pattern<T>() : CodeModel<T>, IPattern<T>
    where T : PatternSyntax
{
    public SimpleNameSyntax NameSyntax() => throw new System.NotImplementedException();

    public IExpression Evaluate(ICodeModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }

    public object? EvaluatePlain(ICodeModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }

    public IdentifierExpression Identifier()
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

    public IdentifierNameSyntax IdentifierNameSyntax()
    {
        throw new System.NotImplementedException();
    }

    public SyntaxToken IdentifierSyntax()
    {
        throw new System.NotImplementedException();
    }

    public IdentifierExpression ToIdentifierExpression()
    {
        throw new System.NotImplementedException();
    }
}