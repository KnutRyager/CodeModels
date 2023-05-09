using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public static class TypeShorthands
{
    public static readonly IType NullType = CodeModelFactory.QuickType("null");
    public static readonly IType VarType = CodeModelFactory.QuickType("var");
    public static readonly IType VoidType = CodeModelFactory.QuickType("void");

    public static readonly IDictionary<string, SyntaxKind> PredefinedTypes = new Dictionary<string, SyntaxKind>()
        {
            { SyntaxKind.ByteKeyword.ToString(), SyntaxKind.ByteKeyword },
            { SyntaxKind.SByteKeyword.ToString(), SyntaxKind.SByteKeyword },
            { SyntaxKind.ShortKeyword.ToString(), SyntaxKind.ShortKeyword },
            { SyntaxKind.UShortKeyword.ToString(), SyntaxKind.UShortKeyword },
            { SyntaxKind.IntKeyword.ToString(), SyntaxKind.IntKeyword },
            { SyntaxKind.UIntKeyword.ToString(), SyntaxKind.UIntKeyword },
            { SyntaxKind.LongKeyword.ToString(), SyntaxKind.LongKeyword },
            { SyntaxKind.ULongKeyword.ToString(), SyntaxKind.ULongKeyword },
            { SyntaxKind.FloatKeyword.ToString(), SyntaxKind.FloatKeyword },
            { SyntaxKind.DoubleKeyword.ToString(), SyntaxKind.DoubleKeyword },
            { SyntaxKind.DecimalKeyword.ToString(), SyntaxKind.DecimalKeyword },
            { SyntaxKind.StringKeyword.ToString(), SyntaxKind.StringKeyword },
            { SyntaxKind.BoolKeyword.ToString(), SyntaxKind.BoolKeyword },
            { SyntaxKind.VoidKeyword.ToString(), SyntaxKind.VoidKeyword },
        };
}
