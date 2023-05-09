using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public abstract record VariableDesignation<T>()
    : CodeModel<T>, IVariableDesignation<T>
    where T : VariableDesignationSyntax
{
    VariableDesignationSyntax IVariableDesignation.Syntax()
        => Syntax();
}