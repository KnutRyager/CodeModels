using System;

namespace CodeAnalyzation.Models;

public interface IProgramModelExecutionContext 
{
    IExpression This();
    IExpression GetValue(string identifier);
    IExpression GetValue(IdentifierExpression identifier);
    void DefineVariable(string identifier);
    void SetValue(string identifier, IExpression valueExpression, bool allowDefine = false);
    void SetValue(IdentifierExpression identifier, IExpression value, bool allowDefine = false);
    void SetValue(IExpression identifier, IExpression value, bool allowDefine = false);
    IExpression ExecuteMethod(string identifier, params IExpression[] parameters);
    object ExecuteMethodPlain(string identifier, params object?[] parameters);
    void SetReturn(IExpression Value);
    void Throw(IExpression value);
    public void Throw(Exception exception);
    void EnterScope(object owner);
    void EnterScope();
    void ExitScope(object owner);
    void ExitScope();
    IExpression PreviousExpression { get; }
    IExpression SetPreviousExpression(IExpression expression);
}
