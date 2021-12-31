using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Models;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
public static class SyntaxExtensions
{
    //[Key] public int Id  { get; set; }
    //[Key] public int? Id2  { get; set; }

    public static string Name(this PropertyDeclarationSyntax node) => node.Identifier.ValueText;
    //public static Type ReturnType(this PropertyDeclarationSyntax node) => node.Type.Type();
    //public static string Name(this TypeSyntax node) => node.;
    //public static Type Type(this TypeSyntax node) => node.;
}