using Microsoft.CodeAnalysis.CSharp;

namespace CodeModels.Models;

public record OrPattern(IPattern Left, IPattern Right) 
    : BinaryPattern(SyntaxKind.OrPattern, Left, Right)
{
}