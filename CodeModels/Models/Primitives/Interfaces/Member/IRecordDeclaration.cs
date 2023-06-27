using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IRecordDeclaration
    : IClassOrRecordDeclaration<RecordDeclaration, RecordDeclarationSyntax>
{
}