using System.Collections.Generic;
using Common.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record Constructor(string Name, PropertyCollection Parameters, Block? Statements, IExpression? ExpressionBody = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
    : MemberModel<ConstructorDeclarationSyntax>(Name, Type(Name), Attributes ?? new List<AttributeList>(), Modifier)
{
    public Constructor(string name, PropertyCollection parameters, Block body, Modifier modifier = Modifier.Public)
        : this(name, parameters, body, null, modifier) { }
    public Constructor(string name, PropertyCollection parameters, IExpression? body = null, Modifier modifier = Modifier.Public)
        : this(name, parameters, null, body, modifier) { }

    public ConstructorDeclarationSyntax ToConstructorSyntax(Modifier modifiers = Modifier.None, Modifier removeModifier = Modifier.None)
        => ConstructorDeclarationCustom(
        attributeLists: List<AttributeListSyntax>(),
        modifiers: Modifier.SetModifiers(modifiers).SetFlags(removeModifier, false).Syntax(),
        identifier: Identifier(Name),
        parameterList: Parameters.ToParameters(),
        body: Statements?.Syntax(),
        initializer: null,
        expressionBody: ExpressionBody is null ? null : ArrowExpressionClause(ExpressionBody.Syntax()));

    public override ConstructorDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => ToConstructorSyntax(modifier, removeModifier);

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var property in Parameters.Properties) yield return property;
        if (Statements is not null) yield return Statements;
        if (ExpressionBody is not null) yield return ExpressionBody;
    }
}
