using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member;

public record ParameterList(List<Parameter> Parameters)
    : CodeModel<ParameterListSyntax>(), IToParameterListConvertible
{
    public static ParameterList Create(IEnumerable<Parameter>? parameters = default)
        => new(List(parameters));
    //public static ParameterList Create(IToParameterListConvertible? parameters = default)
    //    => new(parameters is null ? new(List<Parameter>()) : parameters.ToParameterList());

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var parameter in Parameters) yield return parameter;
    }

    public override ParameterListSyntax Syntax()
        => SyntaxFactory.ParameterList(SeparatedList(Parameters.Select(x => x.Syntax())));

    public ParameterList ToParameterList() => this;
}
