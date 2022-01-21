namespace CodeAnalyzation.Models;

public interface IProgramModelExecutionContext 
{
    object? GetValue(string identifier);
    object? GetValue(IdentifierExpression identifier);
    void SetValue(string identifier, IExpression valueExpression, bool allowDefine = false);
    void SetValue(IdentifierExpression identifier, IExpression value, bool allowDefine = false);
    void Throw(IExpression value);
    void EnterScope(object owner);
    void EnterScope();
    void ExitScope(object owner);
    void ExitScope();
}
