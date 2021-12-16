using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    // Ordering: https://stackoverflow.com/questions/150479/order-of-items-in-classes-fields-properties-constructors-methods
    public enum PropertyType
    {
        PublicConst,
        PrivateConst,
        PublicStaticReadonlyField,
        PublicStaticField,
        PublicField,
        PrivateStaticReadonly,
        PrivateStaticField,
        PrivateReadonlyField,
        PrivateField,
        PublicStaticReadonly,
        PublicReadonly,
        PublicReadWrite,
        PublicReadPrivateWrite,
        PrivateProperty,
    }

    public static class PropertyTypeExtensions
    {
        public static SyntaxTokenList Modifiers(this PropertyType type) => type switch
        {
            PropertyType.PublicConst => TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ConstKeyword)),
            PropertyType.PrivateConst => TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ConstKeyword)),
            PropertyType.PublicStaticField => TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)),
            PropertyType.PrivateStaticField => TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)),
            PropertyType.PublicStaticReadonlyField => TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.ReadOnlyKeyword)),
            PropertyType.PrivateStaticReadonly => TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.ReadOnlyKeyword)),
            PropertyType.PublicReadWrite => TokenList(Token(SyntaxKind.PublicKeyword)),
            PropertyType.PublicReadPrivateWrite => TokenList(Token(SyntaxKind.PublicKeyword)),
            PropertyType.PublicReadonly => TokenList(Token(SyntaxKind.PublicKeyword)),
            PropertyType.PrivateField => TokenList(Token(SyntaxKind.PrivateKeyword)),
            PropertyType.PrivateReadonlyField => TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)),
            PropertyType.PrivateProperty => TokenList(Token(SyntaxKind.PrivateKeyword)),
            _ => throw new ArgumentException($"Unhandled PropertyType '{type}'.")
        };

        public static bool IsWritable(this PropertyType type) => type switch
        {
            PropertyType.PublicConst => false,
            PropertyType.PrivateConst => false,
            PropertyType.PublicStaticField => true,
            PropertyType.PrivateStaticField => true,
            PropertyType.PublicStaticReadonlyField => false,
            PropertyType.PublicStaticReadonly => false,
            PropertyType.PrivateStaticReadonly => false,
            PropertyType.PublicReadWrite => true,
            PropertyType.PublicReadPrivateWrite => true,
            PropertyType.PublicReadonly => false,
            PropertyType.PrivateField => true,
            PropertyType.PrivateReadonlyField => false,
            PropertyType.PrivateProperty => true,
            _ => throw new ArgumentException($"Unhandled PropertyType '{type}'.")
        };

        public static bool IsField(this PropertyType type) => type switch
        {
            PropertyType.PublicConst => true,
            PropertyType.PrivateConst => true,
            PropertyType.PublicStaticField => true,
            PropertyType.PrivateStaticField => true,
            PropertyType.PublicStaticReadonlyField => true,
            PropertyType.PublicStaticReadonly => false,
            PropertyType.PrivateStaticReadonly => false,
            PropertyType.PublicReadWrite => false,
            PropertyType.PublicReadPrivateWrite => false,
            PropertyType.PublicReadonly => false,
            PropertyType.PrivateField => true,
            PropertyType.PrivateReadonlyField => true,
            PropertyType.PrivateProperty => false,
            _ => throw new ArgumentException($"Unhandled PropertyType '{type}'.")
        };
    }
}
