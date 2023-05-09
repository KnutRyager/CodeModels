using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models;

public record OrPattern(IPattern Left, IPattern Right) 
    : BinaryPattern(SyntaxKind.OrPattern, Left, Right)
{
}