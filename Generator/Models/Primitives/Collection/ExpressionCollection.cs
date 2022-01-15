using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record ExpressionCollection(List<IExpression> Values) : Expression<ArrayCreationExpressionSyntax>(Type(FindBaseType(Values), isMulti: true))
{
    public ExpressionCollection(IEnumerable<IExpression>? values = null) : this(List(values)) { }
    public ExpressionCollection(string commaSeparatedValues) : this(commaSeparatedValues.Trim().Split(',').Select(x => new LiteralExpression(x))) { }
    public ExpressionCollection(EnumDeclarationSyntax declaration) : this(declaration.Members.Select(x => new LiteralExpression(x.Identifier.ToString()))) { }

    public EnumDeclarationSyntax ToEnum(string name, bool isFlags = false, bool hasNoneValue = false) => EnumDeclaration(
            attributeLists: default,
            modifiers: TokenList(Token(SyntaxKind.PublicKeyword)),
            identifier: Identifier(name),
            baseList: default,
            members: SeparatedList(Values.Select((x, index) => x.ToEnumValue(isFlags ? hasNoneValue && index == 0 ? 0 : (int)Math.Pow(2, index - (hasNoneValue ? 1 : 0)) : null)))
        );

    public ArgumentListSyntax ToArguments() => ArgumentListCustom(Values.Select(x => x.ToArgument()));
    public TypeSyntax BaseType() => BaseTType().Syntax();
    public virtual IType BaseTType() => FindBaseType(Values);
    public ArrayCreationExpressionSyntax ToArrayInitialization() => ArrayInitializationCustom(BaseTType().TypeSyntaxNonMultiWrapped(), Values.Select(x => x.Syntax()));
    public ObjectCreationExpressionSyntax ToListInitialization() => ListInitializationCustom(BaseType(), Values.Select(x => x.Syntax()));

    public override ArrayCreationExpressionSyntax Syntax() => ToArrayInitialization();

    public static IType FindBaseType(IEnumerable<IExpression> expressions)
        => expressions.Select(x => x.Type).Distinct().Count() is 1
        ? expressions.First().Type : Type(typeof(object));

    public override IEnumerable<ICodeModel> Children() => Values;
}


