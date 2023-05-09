using System;
using System.Collections.Generic;
using CodeModels.Execution.Scope;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;

namespace CodeModels.Execution.Context;

public interface ICodeModelExecutionContext
{
    IProgramContext? ProgramContext { get; }
    IExpression This();
    IExpression GetValue(string identifier);
    IExpression GetValue(IdentifierExpression identifier);
    ICodeModel GetValueOrMember(string identifier);
    ICodeModel GetValueOrMember(IdentifierExpression identifier);
    ICodeModel? TryGetValueOrMember(string identifier);
    ICodeModel? TryGetValueOrMember(IdentifierExpression identifier);
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
    void EnterScope(ICodeModelExecutionScope scope);
    void EnterScopes(IEnumerable<ICodeModelExecutionScope>? scope);
    void ExitScopes(IEnumerable<ICodeModelExecutionScope>? scope);
    void ExitScope(object owner);
    void ExitScope();
    ICodeModelExecutionScope CaptureScope();
    IExpression PreviousExpression { get; }
    IExpression SetPreviousExpression(IExpression expression);
    string ConsoleOutput { get; }
    void ConsoleWrite(string s);
    void ConsoleWriteLine(string s);
    void IncreaseDisableSetPreviousValueLock();
    void DecreaseDisableSetPreviousValueLock();
    IMember GetMember(string? key, string? name = null);
    IMember? TryGetMember(string? key, string? name = null);
    void AddMember(string? key, IMember type);
}
