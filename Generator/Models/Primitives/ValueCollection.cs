using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public class ValueCollection
    {
        public List<Value> Values { get; set; }

        public ValueCollection(IEnumerable<Value> values)
        {
            Values = values.ToList();
        }

        public ValueCollection(string commaSeparatedValues) : this(commaSeparatedValues.Trim().Split(',').Select(x => Value.FromValue(x))) { }
        public ValueCollection(EnumDeclarationSyntax declaration) : this(declaration.Members.Select(x => new Value(x))) { }

        public EnumDeclarationSyntax ToEnum(string name) => EnumDeclaration(
                attributeLists: default,
                modifiers: TokenList(Token(SyntaxKind.PublicKeyword)),
                identifier: Identifier(name),
                baseList: default,
                members: SeparatedList(Values.Select(x => x.ToEnumValue()))
            );

        public ArgumentListSyntax ToArguments() => ArgumentListCustom(Values.Select(x => x.ToArgument()));
        public TypeSyntax BaseType() => Values.Select(x => x.Type).Distinct().Count() is 1 ? Values.First().Type.TypeSyntax() : new TType(typeof(object)).TypeSyntax();
        public ArrayCreationExpressionSyntax ToArrayInitialization() => ArrayInitializationCustom(BaseType(), Values.Select(x => x.Expression));
        public ObjectCreationExpressionSyntax ToListInitialization() => ListInitializationCustom(BaseType(), Values.Select(x => x.Expression));
    }
}


