using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Models;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

public class Property
{
    public string Name { get; set; }
    public string Type { get; set; }

    public Property(string name, string type)
    {
        Name = name;
        Type = type;
    }

    public Property(ITypeSymbol typeSymbol)
    {
        Type = typeSymbol.ToString();
    }

    public Property(ISymbol typeSymbol)
    {
        Type = typeSymbol.ToString();
    }
}