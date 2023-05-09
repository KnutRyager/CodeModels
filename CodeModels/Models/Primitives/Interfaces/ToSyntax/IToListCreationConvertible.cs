using CodeModels.Models.Primitives.Expression.Instantiation;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IToListCreationConvertible
{
    /// <summary>
    /// Create an <see cref="ObjectCreationExpression"/> for list.
    /// </summary>
    ObjectCreationExpressionSyntax ToListInitialization();
}
