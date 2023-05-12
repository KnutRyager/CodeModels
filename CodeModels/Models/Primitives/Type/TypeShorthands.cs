using System.Collections.Generic;
using CodeModels.Factory;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeModels.Models;

public static class TypeShorthands
{
    public static readonly IType NullType = CodeModelFactory.QuickType("null");
    public static readonly IType VarType = CodeModelFactory.QuickType("var");
    public static readonly IType VoidType = CodeModelFactory.QuickType("void");
    public static readonly IType Byte = CodeModelFactory.Type<byte>();
    public static readonly IType BSyte = CodeModelFactory.Type<sbyte>();
    public static readonly IType Short = CodeModelFactory.Type<short>();
    public static readonly IType UShort = CodeModelFactory.Type<ushort>();
    public static readonly IType Int = CodeModelFactory.Type<int>();
    public static readonly IType UInt = CodeModelFactory.Type<uint>();
    public static readonly IType Long = CodeModelFactory.Type<long>();
    public static readonly IType ULong = CodeModelFactory.Type<ulong>();
    public static readonly IType Float = CodeModelFactory.Type<float>();
    public static readonly IType Double = CodeModelFactory.Type<double>();
    public static readonly IType Decimal = CodeModelFactory.Type<decimal>();
    public static readonly IType String = CodeModelFactory.Type<string>();
    public static readonly IType Bool = CodeModelFactory.Type<bool>();

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
