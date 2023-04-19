using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace CodeAnalyzation.Models;

public record PatternExpression(IPattern Pattern, IExpression Rhs, IType Type)
    : IExpression
{
    public bool IsLiteralExpression => throw new System.NotImplementedException();

    public LiteralExpressionSyntax? LiteralSyntax => throw new System.NotImplementedException();

    public object? LiteralValue => throw new System.NotImplementedException();

    public SimpleNameSyntax NameSyntax => throw new System.NotImplementedException();

    public ExpressionStatement AsStatement()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<ICodeModel> Children()
    {
        throw new System.NotImplementedException();
    }

    public string Code()
    {
        throw new System.NotImplementedException();
    }

    public ISet<IType> Dependencies(ISet<IType>? set = null)
    {
        throw new System.NotImplementedException();
    }

    public IExpression Evaluate(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }

    public object? EvaluatePlain(IProgramModelExecutionContext context)
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

    public ExpressionSyntax Syntax()
    {
        throw new System.NotImplementedException();
    }

    public ArgumentSyntax ToArgument()
    {
        throw new System.NotImplementedException();
    }

    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null)
    {
        throw new System.NotImplementedException();
    }

    ExpressionOrPatternSyntax IExpressionOrPattern.Syntax()
    {
        throw new System.NotImplementedException();
    }

    CSharpSyntaxNode ICodeModel.Syntax()
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
