using CodeModels.Execution;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IStatement : ICodeModel, IMember
{
    new StatementSyntax Syntax();
    bool EndsInBreak();
    void Evaluate(IProgramModelExecutionContext context);
}

public interface IStatement<T> : IStatement, ICodeModel<T> where T : StatementSyntax
{
    new T Syntax();
}

public abstract record AbstractStatement<T>(string? Name = null)
    : NamedCodeModel<T>(Name ?? "_AbstractStatement"), IStatement<T> where T : StatementSyntax
{

    StatementSyntax IStatement.Syntax() => Syntax();
    public virtual bool EndsInBreak() => false;
    public abstract void Evaluate(IProgramModelExecutionContext context);


    public string Name => $"Statement";
    public Modifier Modifier => Modifier.None;
    public bool IsStatic => false;

    public SimpleNameSyntax NameSyntax() => throw new System.NotImplementedException();

    MemberDeclarationSyntax IMember.Syntax() => throw new System.NotImplementedException();
    public MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None) => throw new System.NotImplementedException();
    public TypeSyntax TypeSyntax() => throw new System.NotImplementedException();
    public virtual IType Get_Type() => throw new System.NotImplementedException();

    public ICodeModel Render(Namespace @namespace)
    {
        throw new System.NotImplementedException();
    }

    public IType ToType()
    {
        throw new System.NotImplementedException();
    }

    public IExpression ToExpression()
    {
        throw new System.NotImplementedException();
    }

    public ParameterSyntax ToParameter()
    {
        throw new System.NotImplementedException();
    }

    public TupleElementSyntax ToTupleElement()
    {
        throw new System.NotImplementedException();
    }
}
