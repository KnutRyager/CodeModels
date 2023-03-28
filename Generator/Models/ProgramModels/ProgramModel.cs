using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models.ProgramModels;

public abstract record ProgramModel<T>(IProgramContext? Context) : IProgramModel<T> where T : ICodeModel
{
    public abstract T Render();
    ICodeModel IProgramModel.Render() => Render();
    public string Code() => Render().Code();
    public abstract ISet<IType> Dependencies(ISet<IType>? set = null);
    public CSharpSyntaxNode Syntax() => Render().Syntax();
    public abstract IEnumerable<ICodeModel> Children();
}
