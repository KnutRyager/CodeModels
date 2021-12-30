using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public record QuickType(string Identifier, bool Required = true, bool IsMulti = false, TypeSyntax? Syntax = null, Type? Type = null)
        : AbstractType(Identifier, Required, IsMulti, Syntax, Type)
    {
    }
}