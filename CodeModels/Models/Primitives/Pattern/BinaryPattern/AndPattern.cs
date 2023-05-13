using Microsoft.CodeAnalysis.CSharp;

namespace CodeModels.Models;

public record AndPattern(IPattern Left, IPattern Right) 
    : BinaryPattern(SyntaxKind.AndPattern, Left, Right)
{
}