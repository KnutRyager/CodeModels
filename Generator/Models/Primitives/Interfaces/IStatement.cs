using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

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

public abstract record AbstractStatement<T>() : CodeModel<T>, IStatement<T> where T : StatementSyntax
{

    StatementSyntax IStatement.Syntax() => Syntax();
    public virtual bool EndsInBreak() => false;
    public abstract void Evaluate(IProgramModelExecutionContext context);


    public string Name => $"Statement";
    public Modifier Modifier => throw new System.NotImplementedException();
    public bool IsStatic => throw new System.NotImplementedException();
    MemberDeclarationSyntax IMember.Syntax() => throw new System.NotImplementedException();
    public MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None) => throw new System.NotImplementedException();
    public TypeSyntax TypeSyntax() => throw new System.NotImplementedException();
    public IType Get_Type() => throw new System.NotImplementedException();
}
