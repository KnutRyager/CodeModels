using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IToArrayCreationConvertible
{
    /// <summary>
    /// Create an <see cref="ArrayCreationExpressionSyntax"/>.
    /// </summary>
    ArrayCreationExpressionSyntax ToArrayInitialization();
}
