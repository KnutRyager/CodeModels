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
    void Throw(IExpression value);
    public void Throw(Exception exception);
    void EnterScope(object owner);
    void EnterScope();
    void EnterScope(IProgramModelExecutionScope scope);
    void ExitScope(object owner);
    void ExitScope();
    IProgramModelExecutionScope CaptureScope();
    IExpression PreviousExpression { get; }
    IExpression SetPreviousExpression(IExpression expression);
    string ConsoleOutput { get; }
    void ConsoleWrite(string s);
    void ConsoleWriteLine(string s);
    void IncreaseDisableSetPreviousValueLock();
    void DecreaseDisableSetPreviousValueLock();
    ClassDeclaration GetType(string name);
    void AddType(ClassDeclaration type);
}
