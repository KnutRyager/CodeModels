namespace CodeAnalyzation.Models;

public interface IProgramModelExecutionScope
{
    bool IsBottomScope();
    bool HasIdentifier(string identifier);
    bool HasIdentifier(IdentifierExpression identifier);
    object? GetValue(string identifier);
    object? GetValue(IdentifierExpression identifier);
    void DefineVariable(string identifier);
    void DefineVariable(IdentifierExpression identifier);
    void SetValue(string identifier, IExpression value);
    void SetValue(IdentifierExpression identifier, IExpression value);
    void Throw(IExpression value);
}
