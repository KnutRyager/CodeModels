using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IToTupleElementConvertible
{
    TupleElementSyntax ToTupleElement();
}
