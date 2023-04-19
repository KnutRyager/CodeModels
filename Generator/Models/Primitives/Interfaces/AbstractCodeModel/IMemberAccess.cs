namespace CodeAnalyzation.Models;

public interface IMemberAccess
{
    IMethodHolder? Owner { get; }
    IExpression? Instance { get; }
}
