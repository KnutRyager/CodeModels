using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models;

public record AndPattern(IPattern Left, IPattern Right) 
    : BinaryPattern(SyntaxKind.AndPattern, Left, Right)
{
}