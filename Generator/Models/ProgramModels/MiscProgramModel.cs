using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models.ProgramModels;

public record MiscProgramModel(
    Namespace TopLevelNamespace,
    List<IProgramModel> Content)
    : IProgramModel
{
    public IEnumerable<ICodeModel> Children()
    {
        throw new System.NotImplementedException();
    }

    public string Code()
    {
        throw new System.NotImplementedException();
    }

    public ISet<IType> Dependencies(ISet<IType>? set = null)
    {
        throw new System.NotImplementedException();
    }

    public ICodeModel Render()
    {
        throw new System.NotImplementedException();
    }

    public CSharpSyntaxNode Syntax()
    {
        throw new System.NotImplementedException();
    }
}
