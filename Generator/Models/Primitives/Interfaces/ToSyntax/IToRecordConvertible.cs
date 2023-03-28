using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IToRecordConvertible
{
    RecordDeclarationSyntax ToRecord(string? name = null, Modifier modifiers = Modifier.Public);
}
