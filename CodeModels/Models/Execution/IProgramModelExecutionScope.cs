namespace CodeModels.Models;

public interface IProgramModelExecutionScope
{
    bool IsBottomScope();
    bool HasIdentifier(string identifier);
    bool HasIdentifier(IdentifierExpression identifier);
    IExpression GetValue(string identifier);
    IExpression GetValue(IdentifierExpression identifier);
    void DefineVariable(string identifier, IExpression? value = null);
    void DefineVariable(IdentifierExpression identifier, IExpression? value = null);
    void SetValue(string identifier, IExpression value);
    void SetValue(IdentifierExpression identifier, IExpression value);
    IExpression ExecuteMethod(string identifier, params IExpression[] parameters);
    object ExecuteMethodPlain(string identifier, params object?[] parameters);
    void Throw(IExpression value);
    bool HasThis();
    IExpression This();
}
