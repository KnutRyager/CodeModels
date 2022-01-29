using System;

namespace CodeAnalyzation.Models;

public interface IProgramModelExecutionContext 
{
    IExpression GetValue(string identifier);
    IExpression GetValue(IdentifierExpression identifier);
    void DefineVariable(string identifier);
    void SetValue(string identifier, IExpression valueExpression, bool allowDefine = false);
    void SetValue(IdentifierExpression identifier, IExpression value, bool allowDefine = false);
    void SetValue(IExpression identifier, IExpression value, bool allowDefine = false);
    void SetBreak();
    bool HandleBreak();
    void SetContinue();
    bool HandleContinue();
    void SetReturn(IExpression Value);
    bool HandleReturn();
    void Throw(IExpression value);
    public void Throw(Exception exception);
    bool HandleThrow();
    void EnterScope(object owner);
    void EnterScope();
    void ExitScope(object owner);
    void ExitScope();
}
    