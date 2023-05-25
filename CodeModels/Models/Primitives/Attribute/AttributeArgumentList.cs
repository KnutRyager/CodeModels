using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record AttributeArgumentList(List<AttributeArgument> Arguments) : CodeModel<AttributeArgumentListSyntax>
{
    public static AttributeArgumentList Create(IEnumerable<AttributeArgument>? arguments = null) 
        => new(List(arguments));

    public override AttributeArgumentListSyntax Syntax() => AttributeArgumentList(SeparatedList(Arguments.Select(x => x.Syntax())));
    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var argument in Arguments) yield return argument;
    }
}