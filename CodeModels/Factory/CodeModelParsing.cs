using System;
using System.Linq;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeModels.Factory;

public static class CodeModelParsing
{
    public static Modifier ParseModifier(SyntaxTokenList syntax) => Modifier.None.SetModifiers(syntax.Select(ParseSingleModifier));
    public static Modifier ParseSingleModifier(SyntaxToken syntax) => syntax.Kind() switch
    {
        SyntaxKind.PrivateKeyword => Modifier.Private,
        SyntaxKind.ProtectedKeyword => Modifier.Protected,
        SyntaxKind.InternalKeyword => Modifier.Internal,
        SyntaxKind.PublicKeyword => Modifier.Public,
        SyntaxKind.ReadOnlyKeyword => Modifier.Readonly,
        SyntaxKind.ConstKeyword => Modifier.Const,
        SyntaxKind.AbstractKeyword => Modifier.Abstract,
        SyntaxKind.StaticKeyword => Modifier.Static,
        _ => throw new ArgumentException($"Unhandled token '{syntax}'.")
    };
    public static Modifier Modifiers(SyntaxTokenList tokenList) => ParseModifier(tokenList);

    public static IExpression ParseExpression(string code)
    {
        var parser = new CodeModelParser(code);
        var parsed = parser.Parse();
        var member = parsed.Members.First();
        return member is ExpressionStatement expression ? expression : throw new NotImplementedException();
    }

    public static ITypeDeclaration ParseTypeDeclaration(string code)
    {
        var parser = new CodeModelParser(code);
        var parsed = parser.Parse();
        var member = parsed.Members.First();
        return member is ITypeDeclaration declaration ? declaration : throw new NotImplementedException();
    }
}
