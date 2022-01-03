using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public record ExpressionCollection
    {
        public List<IExpression> Values { get; set; }

        public ExpressionCollection(IEnumerable<IExpression> values)
        {
            Values = values.ToList();
        }

        public ExpressionCollection(string commaSeparatedValues) : this(commaSeparatedValues.Trim().Split(',').Select(x => new LiteralExpression(x))) { }
        public ExpressionCollection(EnumDeclarationSyntax declaration) : this(declaration.Members.Select(x => new LiteralExpression(x))) { }

        public EnumDeclarationSyntax ToEnum(string name, bool isFlags = false, bool hasNoneValue = false) => EnumDeclaration(
                attributeLists: default,
                modifiers: TokenList(Token(SyntaxKind.PublicKeyword)),
                identifier: Identifier(name),
                baseList: default,
                members: SeparatedList(Values.Select((x, index) => x.ToEnumValue(isFlags ? hasNoneValue && index == 0 ? 0 : (int)Math.Pow(2, index - (hasNoneValue ? 1 : 0)) : null)))
            );

        public ArgumentListSyntax ToArguments() => ArgumentListCustom(Values.Select(x => x.ToArgument()));
        public TypeSyntax BaseType() => BaseTType().TypeSyntax();
        public virtual IType BaseTType() => Values.Select(x => x.Type).Distinct().Count() is 1 ? Values.First().Type : new TypeFromReflection(typeof(object));
        public ArrayCreationExpressionSyntax ToArrayInitialization() => ArrayInitializationCustom(BaseTType().TypeSyntaxNonMultiWrapped(), Values.Select(x => x.Syntax()));
        public ObjectCreationExpressionSyntax ToListInitialization() => ListInitializationCustom(BaseType(), Values.Select(x => x.Syntax()));
    }
}


