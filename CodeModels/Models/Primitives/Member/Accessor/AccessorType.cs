using System;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models;

public enum AccessorType
{
    None,
    Get,
    Set,
    Init,
    Add,
    Remove,
    Unknown,
}

public static class AccessorTypeExtensions
{
    public static SyntaxKind ToSyntax(this AccessorType accessorType) => accessorType switch
    {
        AccessorType.Get => SyntaxKind.GetAccessorDeclaration,
        AccessorType.Set => SyntaxKind.SetAccessorDeclaration,
        AccessorType.Init => SyntaxKind.InitAccessorDeclaration,
        AccessorType.Add => SyntaxKind.AddAccessorDeclaration,
        AccessorType.Remove => SyntaxKind.RecordDeclaration,
        AccessorType.Unknown => SyntaxKind.UnknownAccessorDeclaration,
        _ => throw new NotImplementedException($"No mapping for '{accessorType}'.")
    };

    public static AccessorType FromSyntax(SyntaxKind syntaxKind) => syntaxKind switch
    {
        SyntaxKind.GetAccessorDeclaration => AccessorType.Get,
        SyntaxKind.SetAccessorDeclaration => AccessorType.Set,
        SyntaxKind.InitAccessorDeclaration => AccessorType.Init,
        SyntaxKind.AddAccessorDeclaration => AccessorType.Add,
        SyntaxKind.RecordDeclaration => AccessorType.Remove,
        SyntaxKind.UnknownAccessorDeclaration => AccessorType.Unknown,
        _ => throw new NotImplementedException($"No mapping for '{syntaxKind}'.")
    };

    public static string GetBackingFieldName(this AccessorType accessorType, string name) => accessorType switch
    {
        AccessorType.Get or AccessorType.Set or AccessorType.Init => $"<{name}>k__BackingField",
        _ => throw new NotImplementedException($"No backing field name for '{accessorType}'.")
    };

    public static string GetBackingMethodName(this AccessorType accessorType, string name) => accessorType switch
    {
        AccessorType.Get => $"get_{name}",
        AccessorType.Set or AccessorType.Init => $"set_{name}",
        _ => throw new NotImplementedException($"No backing method name for '{accessorType}'.")
    };
}