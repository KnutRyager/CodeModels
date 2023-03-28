using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IToSimpleNameConvertible
{
     SimpleNameSyntax NameSyntax { get; }
}
