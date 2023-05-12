using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IToRecordConvertible
{
    RecordDeclarationSyntax ToRecord(string? name = null, Modifier? modifiers = null);
}
