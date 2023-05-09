using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record AttributeList(AttributeTargetSpecifier? Target, List<Attribute> Attributes) : CodeModel<AttributeListSyntax>
{
    public override AttributeListSyntax Syntax() => AttributeList(Target?.Syntax(), SeparatedList(Attributes.Select(x => x.Syntax())));
    public override IEnumerable<ICodeModel> Children()
    {
        if (Target is not null) yield return Target;
        foreach (var attribute in Attributes) yield return attribute;
    }
}

public record Attribute(string Name, AttributeArgumentList Arguments) : CodeModel<AttributeSyntax>
{
    public override AttributeSyntax Syntax() => Attribute(IdentifierName(Name), Arguments.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Arguments;
    }
}

public record AttributeArgumentList(List<AttributeArgument> Arguments) : CodeModel<AttributeArgumentListSyntax>
{
    public override AttributeArgumentListSyntax Syntax() => AttributeArgumentList(SeparatedList(Arguments.Select(x => x.Syntax())));
    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var argument in Arguments) yield return argument;
    }
}

public record AttributeArgument(IExpression Expression, string? Name = null) : CodeModel<AttributeArgumentSyntax>
{
    public override AttributeArgumentSyntax Syntax()
        => AttributeArgument(Name is null ? null : new NameEquals(Name).Syntax(), Name is null ? null : new NameColon(Name).Syntax(), Expression.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }
}

public record AttributeTargetSpecifier(string Identifier) : CodeModel<AttributeTargetSpecifierSyntax>
{
    public override AttributeTargetSpecifierSyntax Syntax() => AttributeTargetSpecifier(Identifier(Identifier));
    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();
}

public record NameEquals(string Identifier) : CodeModel<NameEqualsSyntax>
{
    public override NameEqualsSyntax Syntax() => NameEquals(IdentifierName(Identifier));
    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();
}

public record NameColon(string Name) : CodeModel<NameColonSyntax>
{
    public override NameColonSyntax Syntax() => NameColon(IdentifierName(Name));
    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();
}
