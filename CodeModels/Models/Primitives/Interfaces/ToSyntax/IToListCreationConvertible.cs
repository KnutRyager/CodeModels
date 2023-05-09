using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IToListCreationConvertible
{
    /// <summary>
    /// Create an <see cref="ObjectCreationExpression"/> for list.
    /// </summary>
    ObjectCreationExpressionSyntax ToListInitialization();
}
