using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IVariableDesignation : ICodeModel
{
    new VariableDesignationSyntax Syntax();
}

public interface IVariableDesignation<T> : IVariableDesignation, ICodeModel<T>
    where T : VariableDesignationSyntax
{
    new T Syntax();
}