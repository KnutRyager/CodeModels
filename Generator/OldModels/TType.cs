using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using System.Collections.Generic;
using System;

namespace CodeAnalyzation.Models
{

    public class TType
    {
        private IDictionary<string, SyntaxKind> _predefinedTypes = new Dictionary<string, SyntaxKind>()
        {
            { SyntaxKind.IntKeyword.ToString(), SyntaxKind.IntKeyword },
            { SyntaxKind.UIntKeyword.ToString(), SyntaxKind.UIntKeyword },
            { SyntaxKind.LongKeyword.ToString(), SyntaxKind.LongKeyword },
            { SyntaxKind.ULongKeyword.ToString(), SyntaxKind.ULongKeyword },
        };

        public string Identifier { get; set; }
        public ISymbol Symbol { get; set; }
        public bool Required { get; set; }

        public TType(string identifier, bool required = true)
        {
            Identifier = identifier;
            Required = required;
        }

        public TType(ISymbol typeSymbol, bool required = true)
        {
            Symbol = typeSymbol;
            Identifier = Symbol.Name;
            Required = required;
        }

        public static TType Parse(string code) => Parse(ParseTypeName(code));

        public static TType Parse(TypeSyntax type, bool required = true) => type switch
        {
            PredefinedTypeSyntax t => new TType(t, required),
            NullableTypeSyntax t => Parse(t.ElementType, false),
            IdentifierNameSyntax t => new TType(t.Identifier.ToString()),
            _ => throw new ArgumentException($"Unhandled {nameof(TypeSyntax)}: '{type}'.")
        };

        //SyntaxNodeExtensions.GetSemanticModel(type.SyntaxTree).GetDeclaredSymbol(type)) { }
        public TType(PredefinedTypeSyntax type, bool required = true) : this(type.Keyword.ToString(), required) { }

        public TypeSyntax TypeSyntax() => Required ? TypeSyntaxNonNullableWrapped() : NullableType(TypeSyntaxNonNullableWrapped());

        public TypeSyntax TypeSyntaxNonNullableWrapped() => Identifier switch
        {
            _ when _predefinedTypes.ContainsKey(Identifier) => PredefinedType(Token(_predefinedTypes[Identifier])),
            _ => IdentifierName(Identifier(Identifier))
        };
    }
}