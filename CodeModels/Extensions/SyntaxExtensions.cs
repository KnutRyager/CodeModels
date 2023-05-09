using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Extensions;

public static class SyntaxExtensions
{
    //[Key] public int Id  { get; set; }
    //[Key] public int? Id2  { get; set; }

    public static string Name(this PropertyDeclarationSyntax node) => node.Identifier.ValueText;
    //public static Type ReturnType(this PropertyDeclarationSyntax node) => node.Type.Type();
    //public static string Name(this TypeSyntax node) => node.;
    //public static Type Type(this TypeSyntax node) => node.;
}