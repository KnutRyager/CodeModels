using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record TypeCollection(List<IType> Types) : CodeModel<TypeParameterListSyntax>
{
    public TypeCollection(IEnumerable<IType>? values = null) : this(List(values)) { }

    public ExpressionCollection ToExpressions() => new(Types.Select(x => CodeModelFactory.Literal(x.Name)));

    public override TypeParameterListSyntax Syntax() => TypeParameterList(SeparatedList(Types.Select(x => x.ToTypeParameter())));

    public override IEnumerable<ICodeModel> Children() => Types;
}


