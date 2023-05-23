using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IStatement : ICodeModel, IMember
{
    new StatementSyntax Syntax();
    bool EndsInBreak();
    void Evaluate(ICodeModelExecutionContext context);
}

public interface IStatement<T> : IStatement, ICodeModel<T> where T : StatementSyntax
{
    new T Syntax();
}

public abstract record AbstractStatement<T>(string? Name = null, Modifier Modifier = Modifier.None)
    : NamedCodeModel<T>(Name ?? "_AbstractStatement"), IStatement<T> where T : StatementSyntax
{
    StatementSyntax IStatement.Syntax() => Syntax();
    public virtual bool EndsInBreak() => false;
    public abstract void Evaluate(ICodeModelExecutionContext context);

    //public Modifier Modifier => Modifier.None;
    public bool IsStatic => false;

    MemberDeclarationSyntax IMember.Syntax() => new GlobalStatement(this).Syntax();
    public virtual MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None) => throw new System.NotImplementedException();
    public virtual TypeSyntax TypeSyntax() => throw new System.NotImplementedException();
    public virtual IType Get_Type() => TypeShorthands.VoidType;

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
