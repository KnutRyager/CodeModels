using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public enum MethodHolderType
    {
        PublicClass,
        PublicStaticClass,
        PublicRecord,
        PublicStruct,
        PublicRecordClass,
    }

    public static class MethodHolderTypeExtensions
    {
        public static SyntaxTokenList Modifiers(this MethodHolderType type) => type switch
        {
            MethodHolderType.PublicClass => TokenList(Token(SyntaxKind.PublicKeyword)),
            MethodHolderType.PublicStaticClass => TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)),
            MethodHolderType.PublicRecord => TokenList(Token(SyntaxKind.PublicKeyword)),
            MethodHolderType.PublicStruct => TokenList(Token(SyntaxKind.PublicKeyword)),
            MethodHolderType.PublicRecordClass => TokenList(Token(SyntaxKind.PublicKeyword)),
            _ => throw new ArgumentException($"Unhandled MethodHolderType '{type}'.")
        };

        public static bool IsWritable(this MethodHolderType type) => type switch
        {
            MethodHolderType.PublicClass => true,
            MethodHolderType.PublicStaticClass => true,
            MethodHolderType.PublicRecord => false,
            MethodHolderType.PublicStruct => true,
            MethodHolderType.PublicRecordClass => true,
            _ => throw new ArgumentException($"Unhandled MethodHolderType '{type}'.")
        };
    }
}
